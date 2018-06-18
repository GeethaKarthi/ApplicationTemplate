using System;
using System.Collections.Generic;
using System.Text;

namespace AWSServerlessApp.CustomModels
{
  public  class AspNetUserPinModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Message { get; set; }
        public string EmailAddress { get; set; }
        public string Pin { get; set; }
    }
}
