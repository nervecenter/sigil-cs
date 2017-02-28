using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Repository
{
        public class OfficialResponseRepository : RepositoryBase<OfficialResponse>, IOfficialResponseRepository
        {
            public OfficialResponseRepository(IDbFactory dbFactory) : base(dbFactory) { }

            //where we define the OfficialResponse methods created below

           
        }

        public interface IOfficialResponseRepository : IRepository<OfficialResponse>
        {
            //Methods for how when we need to get OfficialResponses

            
        }
    
}