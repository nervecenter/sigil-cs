using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Notification methods created below
    }

    public interface INotificationRepository : IRepository<Notification>
    {
        //Methods for how when we need to get Notifications
    }
}