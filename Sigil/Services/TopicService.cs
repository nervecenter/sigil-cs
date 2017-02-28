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
        Topic GetTopic(int topicId);
        Topic GetTopic(string topicSTR, bool name = false);
        IEnumerable<Topic> GetAllTopics();

        void UpdateTopic(Topic top);
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

        public Topic GetTopic(int topicId)
        {
            var top = topicRepository.GetById(topicId);
            return top;
        }

        public Topic GetTopic(string topicSTR, bool name)
        {
            Topic top = default(Topic);

            if (name)
                top = topicRepository.GetTopicByName(topicSTR);
            else
                top = topicRepository.GetTopicByURL(topicSTR);

            return top;
        }

        public IEnumerable<Topic> GetAllTopics()
        {
            return topicRepository.GetAll();
        }

        public void UpdateTopic(Topic top)
        {
            topicRepository.Update(top);
            unitOfWork.Commit();
        }

        public void SaveTopic()
        {
            unitOfWork.Commit();
        }
    }
}