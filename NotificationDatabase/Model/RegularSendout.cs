using System;
using System.Collections.Generic;

namespace NotificationDatabase.Model
{
    public partial class RegularSendout : IModelBase
    {
        public int Id { get; set; }
        public string ReminderName { get; set; }
        public DateTime StartDate { get; set; }
        public string RepetitionFrequency { get; set; }
        public int ExecutionTime { get; set; }
        public string DayOfTheWeek { get; set; }
        public DateTime? LastRunAt { get; set; }
        public string Parameters { get; set; }
        public string Username { get; set; }
        public string UserGroup { get; set; }
    }
}
