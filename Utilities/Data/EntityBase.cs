using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace Utilities.Data
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class EntityBase<TKey>: IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual TKey Id { get; set; }

        public static bool operator ==(EntityBase<TKey> x, EntityBase<TKey> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(EntityBase<TKey> x, EntityBase<TKey> y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityBase<TKey>);
        }

        public virtual bool Equals(EntityBase<TKey> other)
        {
            var result = false;
            if (other != null)
            {
                if (!ReferenceEquals(this, other))
                {
                    if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
                    {
                        var otherType = other.GetUnproxiedType();
                        var thisType = GetUnproxiedType();
                        result = thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return Equals(Id, default(int)) ? base.GetHashCode() : Id.GetHashCode();
        }

        private static bool IsTransient(EntityBase<TKey> obj)
        {
            return obj != null && Equals(obj.Id, default(TKey));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }
    }


    public class ObjectIdConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new ObjectId(reader.Value.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
            //return true;
        }


    }

    public class ListObjectIdConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize<List<ObjectId>>(reader);
            }
            else
            {
                var media = serializer.Deserialize<ObjectId>(reader);
                return new List<ObjectId>(new[] { media });
            }

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(List<ObjectId>).IsAssignableFrom(objectType);
            //return true;
        }


    }

    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [JsonConverter(typeof(ObjectIdConverter))]
        public virtual ObjectId Id { get; set; }

        public static bool operator ==(EntityBase x, EntityBase y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(EntityBase x, EntityBase y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityBase);
        }

        public virtual bool Equals(EntityBase other)
        {
            var result = false;
            if (other != null)
            {
                if (!ReferenceEquals(this, other))
                {
                    if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
                    {
                        var otherType = other.GetUnproxiedType();
                        var thisType = GetUnproxiedType();
                        result = thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return Equals(Id, default(int)) ? base.GetHashCode() : Id.GetHashCode();
        }

        private static bool IsTransient(EntityBase obj)
        {
            return obj != null && Equals(obj.Id, default(object));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }
    }
}