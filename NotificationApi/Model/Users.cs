using System;
using System.Collections.Generic;

namespace NotificationApi.Model
{
    public partial class Users : IModelBase
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Sms { get; set; }
    }
}
