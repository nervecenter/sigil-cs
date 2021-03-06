using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Repository
{
    public interface IUnitOfWork
    {
        void Commit();
    }

    /// <summary>
    /// Basic worker that saves all changes given to database. 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private SigilEntities dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public SigilEntities DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public void Commit()
        {
            DbContext.Commit();
        }
    }
}