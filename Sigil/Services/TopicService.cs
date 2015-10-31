using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


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
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;




    }
}