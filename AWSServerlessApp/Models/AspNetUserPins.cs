using System;
using System.Collections.Generic;

namespace AWSServerlessApp.Models
{
    public partial class AspNetUserPins
    {
        public string Id { get; set; }
        public string Pin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmailAddress { get; set; }
    }
}
