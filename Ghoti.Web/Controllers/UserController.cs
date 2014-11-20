using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.User;
using Utilities;
using PagedList;
using MongoDB.Bson;

namespace ghoti.web.Controllers
{
    public class UserController : Nancy.NancyModule
    {
        private ICrudController<User> _crudController;

        public UserController(ICrudController<User> crudController, DataInitializer.Initializer initializer)
        {
            _crudController = crudController;

            Get["/api/User/Get"] = _ => _crudController.GetAll();
            Get["/api/User/Get/{Id}"] = _ =>
            {
                return _crudController.Get(_.Id);
            };
            Put["/api/User/Put"] = _ => crudController.Put(new User()
                {
                    Id = _.Id,
                    Email = _.Email,
                    FullName = _.FullName,
                    ShortName = _.ShortName,
                    UserName = _.UserName
                });
            Get["/api/User/Delete/{Id}"] = _ => crudController.Delete(_.Id);
            Get["/api/User/Delete/0"] = _ => crudController.RemoveAll();
            Get["/api/User/Initialize"] = _ =>
            {
                initializer.InitializeAdminUser();
                initializer.InitializePlayers(10);

                return "1";
            };
        }
    }
}