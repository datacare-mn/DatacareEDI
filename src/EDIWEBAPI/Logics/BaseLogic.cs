using System;
using System.Collections.Generic;
using System.Linq;
using EDIWEBAPI.Context;
using EDIWEBAPI.Entities.Interfaces;
using EDIWEBAPI.Entities.DBModel.SystemManagement;
using Microsoft.Extensions.Logging;

namespace EDIWEBAPI.Logics
{
    public class BaseLogic
    {
        public static void Insert(OracleDbContext _context, object newObject, bool saveChanges = true)
        {
            _context.Entry(newObject).State = System.Data.Entity.EntityState.Added;
            if (saveChanges)
                _context.SaveChanges();
        }

        public static void Update(OracleDbContext _context, object currentObject, bool saveChanges = true)
        {
            _context.Entry(currentObject).State = System.Data.Entity.EntityState.Modified;
            if (saveChanges)
                _context.SaveChanges();
        }

        public static void Delete(OracleDbContext _context, object currentObject, bool saveChanges = true)
        {
            _context.Entry(currentObject).State = System.Data.Entity.EntityState.Deleted;
            if (saveChanges)
                _context.SaveChanges();
        }

        public static decimal GetNewId(OracleDbContext _context, string tablename)
        {
            return Convert.ToDecimal(_context.Database.SqlQuery<decimal>($"SELECT {tablename}_seq.nextval nextval FROM dual").Single());
        }

        public static SYSTEM_ENTITY GetEntity(OracleDbContext _context, string entityName)
        {
            var entity = _context.SYSTEM_ENTITY.FirstOrDefault(a => a.ID == entityName);
            if (entity == null)
                throw new Exception($"{entityName} ТӨРЛИЙН ТОХИРГООГ ХИЙГЭЭГҮЙ БАЙНА.");
            return entity;
        }

        public static IBasicEntity GetBasicEntity(OracleDbContext _context, string entityName, int id)
        {
            var entity = GetEntity(_context, entityName);
            var type = Type.GetType(entity.CLASSNAME);
            return GetBasicEntity(_context, type, id);
        }

        public static IBasicEntity GetBasicEntity(OracleDbContext _context, Type type, int id)
        {
            return (_context.Set(type) as IEnumerable<IBasicEntity>).FirstOrDefault(a => a.ID == id);
        }

        public static List<IBasicEntity> GetBasicEntities(OracleDbContext _context, string entityName)
        {
            var entity = GetEntity(_context, entityName);
            return (_context.Set(Type.GetType(entity.CLASSNAME)) as IEnumerable<IBasicEntity>)
                .Where(a => a.ENABLED == 1)
                .OrderBy(a => a.VIEWORDER).ToList();
        }

        public static IBasicEntity AddBasicEntity(OracleDbContext _context, ILogger _log, string entityName, string json)
        {
            var type = GetEntityType(_context, _log, entityName);
            var deserialized = Deserialize(_context, type, json);

            deserialized.ID = Convert.ToInt16(GetNewId(_context, type.Name));
            deserialized.ENABLED = 1;

            Insert(_context, deserialized);
            return deserialized;
        }
        
        public static IBasicEntity SetBasicEntity(OracleDbContext _context, ILogger _log, string entityName, string json)
        {
            var type = GetEntityType(_context, _log, entityName);
            var deserialized = Deserialize(_context, type, json);
            var found = GetBasicEntity(_context, type, deserialized.ID);
            if (found == null)
                throw new Exception("NOT FOUND");

            found.NAME = deserialized.NAME;
            found.VIEWORDER = deserialized.VIEWORDER;
            found.ENABLED = deserialized.ENABLED;

            Update(_context, found);
            return deserialized;
        }

        public static void DeleteBasicEntity(OracleDbContext _context, ILogger _log, string entityName, int id)
        {
            var type = GetEntityType(_context, _log, entityName);
            var found = GetBasicEntity(_context, type, id);
            if (found == null)
                throw new Exception("NOT FOUND");

            found.ENABLED = 0;

            Update(_context, found);
        }

        public static Type GetEntityType(OracleDbContext _context, ILogger _log, string entityName)
        {
            var entity = GetEntity(_context, entityName);
            return Type.GetType(entity.CLASSNAME);
        }

        public static IBasicEntity Deserialize(OracleDbContext _context, string entityName, string json)
        {
            var entity = GetEntity(_context, entityName);
            var type = Type.GetType(entity.CLASSNAME);
            return Deserialize(_context, type, json);
        }

        public static IBasicEntity Deserialize(OracleDbContext _context, Type type, string json)
        {
            return (IBasicEntity)Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }
    }
}
