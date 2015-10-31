using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IUserService
    {
        AspNetUser GetUser(string id);
        string GetUserDisplayName(string id);
        void SetUserRole(AspNetRole role, string id);

        IEnumerable<Comment> GetUserComments(string id);

        Tuple<IEnumerable<Issue>, IEnumerable<Comment>> GetUserVotes(string id);
    }

    public class UserService : IUserService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository userRepo, )


    }
}