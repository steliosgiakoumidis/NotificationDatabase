using System.Collections.Generic;
using System.Linq;
using NotificationApi.Model;

namespace NotificationApi.Utilities
{
    public class AddSendoutValidation
    {
        public static bool CheckUserTemplateAndGroupExist(List<Users> users, 
            List<UserGroups> groups, List<Templates> templates,
            RegularSendout sendout)
        {
            var userCheck = users.Select( x => x.Username).Contains(sendout.Username);
            var groupCheck = groups.Select( x => x.GroupName).Contains(sendout.UserGroup);
            var templateCheck = templates.Select( x => x.NotificationName).Contains(sendout.ReminderName);
            if(sendout.Username == null) return groupCheck && templateCheck;
            if(sendout.UserGroup == null) return userCheck && templateCheck;
            return userCheck && groupCheck && templateCheck;
        }
    }
}