using System;
using System.Collections.Generic;

namespace AWSServerlessApp.Models
{
    public partial class Notifications
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Disabled { get; set; }
        public string Subject { get; set; }
    }
}
