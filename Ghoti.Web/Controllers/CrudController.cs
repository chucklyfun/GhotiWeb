using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using Utilities.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using Utilities;

namespace ghoti.web.Controllers
{
    public interface ICrudController<T>
        where T : IEntity<ObjectId>
    {
        string Get(string id);

        string Put(T t);
        
        string GetAll(int index = 1, int pageSize = 0);

        string Delete(string id);

        string RemoveAll();
    }

    public class CrudController<T> : ICrudController<T>
        where T : class, IEntity<ObjectId>
    {
        private readonly IRepository<T> _repository;

        private readonly ISerializationService _serializationService;

        public CrudController(IRepository<T> repository, ISerializationService serializationService)
        {
            _repository = repository;
            _serializationService = serializationService;
        }

        public string Get(string id)
        {
            return _serializationService.Serialize(_repository.GetById(new ObjectId(id)));
        }

        public string Put(T t)
        {
            return _serializationService.Serialize(_repository.Update(t));
        }
        
        public string GetAll(int index = 1, int pageSize = 0)
        {
            var values = _repository.AsQueryable();

            IPagedList<T> result = null;
            if (pageSize > 0)
            {
                result = values.ToPagedList(index, pageSize);
            }
            else
            {
                result = values.ToPagedList(1, values.Count());
            }
            return _serializationService.Serialize(result);
        }

        public string Delete(string id)
        {
            _repository.Delete(new ObjectId(id));
            return "1";
        }

        public string RemoveAll()
        {
            _repository.RemoveAll();
            return "1";
        }
    }
}
