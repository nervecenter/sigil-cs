using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    /// <summary>
    /// Parent DB factory for access the DB. Inherits from IDisposable explained below
    /// </summary>
    public interface IDbFactory : IDisposable
    {
        SigilEntities Init();
    }

    /// <summary>
    /// Second DB facotry parent that also inherits from IDisposable so that we can correctly clean up the created DB accesses
    /// </summary>
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }

            isDisposed = true;
        }

        // Ovveride this to dispose custom objects
        protected virtual void DisposeCore()
        {
        }
    }


    public class DbFactory : Disposable, IDbFactory
    {
        SigilEntities dbContext;

        public SigilEntities Init()
        {
            return dbContext ?? (dbContext = new SigilEntities());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}