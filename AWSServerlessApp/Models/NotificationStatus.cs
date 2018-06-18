using System;
using System.Collections.Generic;

namespace AWSServerlessApp.Models
{
    public partial class NotificationStatus
    {
        public int Id { get; set; }
        public int? NotificationId { get; set; }
        public string UserId { get; set; }
        public bool? ReadStatus { get; set; }
    }
}
