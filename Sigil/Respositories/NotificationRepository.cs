using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{ 
    public interface INotificationRepository : IRepository<Notification>
    {
        //Methods for how when we need to get Notifications

        IEnumerable<Notification> GetUsersNotifications(string userId);
        IEnumerable<Notification> GetOrgsNotifications(int orgId);
    }

    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public IEnumerable<Notification> GetOrgsNotifications(int orgId)
        {
            var notes = this.DbContext.Notifications.Where(i => i.To_OrgId == orgId).Select(i => i);
            return notes;
        }

        public IEnumerable<Notification> GetUsersNotifications(string userId)
        {
            var notes = this.DbContext.Notifications.Where(i => i.To_UserId == userId).Select(i => i);
            return notes;
        }

        //where we define the Notification methods created below
    }

}