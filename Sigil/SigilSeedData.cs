using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sigil
{
    public class SigilSeedData : DropCreateDatabaseIfModelChanges<SigilEntities>
    {
        protected override void Seed(SigilEntities context)
        {
            base.Seed(context);
            context.Commit();

        }

        //This is where we can add initial data 
    }
}