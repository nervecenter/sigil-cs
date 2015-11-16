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
using Sigil.Repository;
using System.Xml.Linq;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Sigil.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
            //AutoMapperConfiguration.Configure();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterType<UserManager<ApplicationUser>>();
            //builder.RegisterType(typeof(ApplicationUserManager));//(c => HttpContext.Current.GetOwinContext().GetUserManager).As<ApplicationUserManager>();
            //builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager).As<ApplicationUserManager>();
            //Repositories
            builder.RegisterAssemblyTypes(typeof(OrgRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(IssueRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(CommentRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ErrorRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(CategoryRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ViewCountRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(VoteCountRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(SubscriptionCountRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(CommentCountRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(SubscriptionRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ImageRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(NotificationRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(OfficialResponseRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(TopicRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly).Where(o => o.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerRequest();

            //Services
            builder.RegisterAssemblyTypes(typeof(OrgService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(IssueService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(CommentService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ErrorService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(CategoryService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
           
            builder.RegisterAssemblyTypes(typeof(CountService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(SubscriptionService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ImageService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(NotificationService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
           
            builder.RegisterAssemblyTypes(typeof(TopicService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(UserService).Assembly).Where(o => o.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();

            

            IContainer container = builder.Build();
           
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}