using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ICommentService
    {
        void CreateComment(Comment comment);
        void EditComment(Comment comment, string newText);
        void SaveComment();
        void DeleteComment(Comment comment);

        Comment GetComment(int orgId, int issueId, int commentId);

        IEnumerable<Comment> GetUserComments(string userId);
        IEnumerable<Comment> GetIssueComments(int orgId, int issueId);
        IEnumerable<Comment> GetOrgComments(int orgId);

    }

    public class CommentService : ICommentService
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