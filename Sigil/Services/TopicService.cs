using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ITopicService
    {
        void CreateTopic(Topic top);
        void SaveTopic();
    }

    public class TopicService : ITopicService
    {
        private readonly ITopicRepository topicRepository;

        private readonly IUnitOfWork unitOfWork;

        public TopicService(IUnitOfWork unit, ITopicRepository topRepo)
        {
            unitOfWork = unit;
            topicRepository = topRepo;
        }

        public void CreateTopic(Topic top)
        {
            topicRepository.Add(top);
        }

        public void SaveTopic()
        {
            unitOfWork.Commit();
        }
    }
}