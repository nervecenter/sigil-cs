//using Sigil.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;

//namespace Sigil
//{
//    public class SigilSeedData : DropCreateDatabaseIfModelChanges<SigilEntities>
//    {
//        protected override void Seed(SigilEntities context)
//        {
//            //GetOrgs().ForEach(o => context.Orgs.Add(o));
//            //GetCats().ForEach(c => context.Products.Add(c));
//            CreateRoles().ForEach(r => context.Roles.Add(r));
//            //CreateSigilUsers().ForEach(u => )

//            //base.Seed(context);


//            context.Commit();
//            context.SaveChanges();

            
//        }

//        //This is where we can add initial data 

       
       
//    }
//}