using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;

namespace Sigil.Controllers
{
    public class NotificationController : Controller
    {
        private static SigilDBDataContext dc = new SigilDBDataContext();

        public static void Notification_Check(string text, string user)
        {
            string to_user = null;
            string to_org = null;
            var text_words = text.Split(' ');
            foreach (string t in text_words)
            {
                if (t[0] == '@')
                {
                    to_user = t.Remove(0, 1);

                }

            }

            if (to_user != null || to_org != null)
            {
                var to_user_id = from users in dc.AspNetUsers
                                 where users.DisplayName == to_user
                                 select users.Id;

                var to_org_id = from org in dc.Orgs
                                where org.orgURL == to_org
                                select org.Id;

                Notification note = new Notification();
                try
                {
                    note.From_UserId = user;
                    note.To_UserId = to_user_id.Cast<string>().First();
                    //note.To_OrgId = Convert.ToInt32(to_org_id.ToString());
                    note.createTime = DateTime.UtcNow;
                    dc.Notifications.InsertOnSubmit(note);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    ErrorHandler.Log_Error(note, e, dc);
                    //Console.WriteLine("Could not unvote on issue %s:\n%s", note.Id, e.Message);
                }
            }
        }
    }
}