﻿using System;
using System.Collections.Generic;

namespace NotificationApi.Model
{
    public partial class UserGroups : IModelBase
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string UserIds { get; set; }
    }
}
