using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class TopicRepository : RepositoryBase<Topic>, ITopicRepository
    {
        public TopicRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Topic methods created below
    }

    public interface ITopicRepository : IRepository<Topic>
    {
        //Methods for how when we need to get Topics
    }
}