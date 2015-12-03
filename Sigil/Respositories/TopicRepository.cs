using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public interface ITopicRepository : IRepository<Topic>
    {
        Topic GetTopicByURL(string topicURL);
        Topic GetTopicByName(string topicName);
    }

    public class TopicRepository : RepositoryBase<Topic>, ITopicRepository
    {
        public TopicRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Topic GetTopicByName(string topicName)
        {
            return DbContext.Topics.Where(t => t.topicName == topicName).FirstOrDefault();
            
        }

        public Topic GetTopicByURL(string topicURL)
        {
            return DbContext.Topics.Where(t => t.topicURL == topicURL).FirstOrDefault();
        }

        //where we define the Topic methods created below
    }

}