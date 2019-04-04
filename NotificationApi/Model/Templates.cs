using System;
using System.Collections.Generic;

namespace NotificationApi.Model
{
    public partial class Templates : IModelBase
    {
        public int Id { get; set; }
        public string NotificationText { get; set; }
        public string NotificationName { get; set; }
        public string NotificationPriority { get; set; }
    }
}
