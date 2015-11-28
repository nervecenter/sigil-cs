using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

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
        IEnumerable<Notification> GetOrgNotifications(string orgURL, bool name = false);


    }

    public class NotificationService : INotificationService
    {
        private readonly IOrgRepository orgRepository;
        private readonly IProductRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public NotificationService(IUnitOfWork unit, INotificationRepository noteRepo, IOrgRepository orgRepo)
        {
            unitOfWork = unit;
            orgRepository = orgRepo;
            notificationRepository = noteRepo;
        }

        public void CreateNotification(Notification note)
        {
            notificationRepository.Add(note);
        }

        public void ArchiveNotification(Notification note)
        {
            throw new NotImplementedException();
        }

        public void DeleteNotification(Notification note)
        {
            notificationRepository.Delete(note);
        }

        public void SaveNotification()
        {
            unitOfWork.Commit();
        }

        public IEnumerable<Notification> GetUserNotifications(string userId)
        {
            return notificationRepository.GetUsersNotifications(userId) ?? new List<Notification>().AsEnumerable();
        }

        public IEnumerable<Notification> GetOrgNotifications(int orgId)
        {
            return notificationRepository.GetOrgsNotifications(orgId) ?? new List<Notification>().AsEnumerable();
        }

        public IEnumerable<Notification> GetOrgNotifications(string orgURL, bool name)
        {
            Org org;
            if (name)
            {
                org = orgRepository.GetByName(orgURL);
            }
            else
            {
                org = orgRepository.GetByURL(orgURL);
            }

            return GetOrgNotifications(org.Id);
        }
    }
}