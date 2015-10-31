using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface INotificationService
    {
        void CreateNotification(Notification note);
        void ArchiveNotification(Notification note);
        void DeleteNotification(Notification note);
        void SaveNotification();

        IEnumerable<Notification> GetUserNotifications(string userId);
        IEnumerable<Notification> GetOrgNotifications(int orgId);
        IEnumerable<Notification> GetOrgNotifications(int orgURL, bool name);


    }

    public class NotificationService : INotificationService
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