using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public interface IUnitOfWork
    {
        void Commit();
    }

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