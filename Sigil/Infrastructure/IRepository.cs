using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Sigil.Repository
{
    public interface IRepository<T> where T : class
    {
        // Marks an entity as new
        void Add(T entity);
        
        // Marks an entity as modified
        void Update(T entity);
        
        // Marks an entity to be removed
        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);
        
        // Get an entity by int id
        T GetById(int id);

        //T GetByName(string name);

        //T GetByURL(string URL);
        
        // Get an entity using delegate
        T Get(Expression<Func<T, bool>> where);
        
        // Gets all entities of type T
        IEnumerable<T> GetAll();
        
        // Gets entities using delegate
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    }


    public abstract class RepositoryBase<T> where T : class
    {
        #region Properties
        private SigilEntities dataContext;
        private readonly IDbSet<T> dbSet;
 
        protected IDbFactory DbFactory
        {
            get;
            private set;
        }
 
        protected SigilEntities DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }
        #endregion
 
        protected RepositoryBase(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            dbSet = DbContext.Set<T>();
        }
 
        #region Implementation
        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }
 
        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }
 
        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
 
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
        }
 
        public virtual T GetById(int id)
        {
            return dbSet.Find(id);
        }
 
        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }
 
        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }
 
        public T Get(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }
 
        #endregion
     
    }
}