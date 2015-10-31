using Autofac;
using Autofac.Integration.Mvc;
using Sigil.Mappings;
using Sigil.Models;
using Sigil.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Sigil.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
            AutoMapperConfiguration.Configure();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();

            //Repositories
            builder.RegisterAssemblyTypes(typeof(OrgRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();

            //Services
            builder.RegisterAssemblyTypes(typeof(OrgService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();


            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}