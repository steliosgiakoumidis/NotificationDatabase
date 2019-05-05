using System.Collections.Generic;
using System.Linq;
using NotificationDatabase.Model;
using NotificationCommon.Models;

namespace NotificationDatabase.Utilities
{
    public class AddSendoutValidation
    {
        public static bool CheckUserTemplateAndGroupExist(List<User> users, 
            List<UserGroup> groups, List<Template> templates,
            Sendout sendout)
        {
            var userExists = users.Select( x => x.Username).Contains(sendout.Username);
            var groupExists = groups.Select( x => x.GroupName).Contains(sendout.UserGroup);
            var templateExists = templates.Select( x => x.NotificationName).Contains(sendout.ReminderName);
            if (sendout.UserGroup != null &&
                groupExists &&
                templateExists) return true;
            if (sendout.Username != null &&
                userExists &&
                templateExists) return true;
            return false;
        }
    }
}