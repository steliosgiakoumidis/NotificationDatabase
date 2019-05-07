using NotificationDatabase.Model;
using NotificationCommon;
using NotificationCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationDatabase
{
    public class DbEntityDtoTransformer
    {
        public static Sendout SendoutDbEntityToDto(RegularSendout dbEntitiesList)
        {
            return new Sendout(dbEntitiesList.Id, dbEntitiesList.ReminderName, dbEntitiesList.StartDate, dbEntitiesList.RepetitionFrequency.RepetitionFrequencyStringToEnum(),
                dbEntitiesList.ExecutionTime.ExecutionTimeStingToEnum(), dbEntitiesList.DayOfTheWeek.DayOfTheWeekStingToEnum(),
                dbEntitiesList.LastRunAt, dbEntitiesList.Parameters, dbEntitiesList.Username, dbEntitiesList.UserGroup);
        }
        public static RegularSendout SendoutDtoToDbEntity(Sendout sendoutDto)
        {
            return new RegularSendout()
            {
                Id = sendoutDto.Id,
                StartDate = sendoutDto.StartDate,
                ReminderName = sendoutDto.ReminderName,
                RepetitionFrequency = sendoutDto.RepetitionFrequency.ToString(),
                ExecutionTime = (int)sendoutDto.ExecutionTime,
                DayOfTheWeek = sendoutDto.DayOfTheWeek.ToString(),
                LastRunAt = sendoutDto.LastRunAt,
                Parameters = sendoutDto.Parameters,
                UserGroup = sendoutDto.UserGroup,
                Username = sendoutDto.Username
            };
        }
        public static Template TemplateDbEntityToDto(Templates template)
        {
            return new Template(template.Id, template.NotificationText,
                template.NotificationName,
                template.NotificationPriority.MeansOfCommunicationStingToEnum());
        }
        public static Templates TemplateDtoToDbEntity(Template template)
        {
            return new Templates
            {
                Id = template.Id,
                NotificationName = template.NotificationName,
                NotificationText = template.NotificationText,
                NotificationPriority = template.NotificationPriority.ToString()
            };
        }

        public static User UserDbEntryToDto(Users user)
        {
            return new User(user.Id, user.Username, user.Email, user.Sms);
        }

        public static Users UserDtoToDbEntry(User user)
        {
            return new Users()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Sms = user.SMS
            };
        }

        public static UserGroup UserGroupDbEntryToDto(UserGroups userGroup)
        {
            return new UserGroup(userGroup.Id, userGroup.GroupName, userGroup.UserIds);
        }

        public static UserGroups UserGroupDtoToDbEntry(UserGroup userGroup)
        {
            return new UserGroups()
            {
                Id = userGroup.Id,
                GroupName = userGroup.GroupName,
                UserIds = userGroup.UserIds
            };
        }

    }  
}
