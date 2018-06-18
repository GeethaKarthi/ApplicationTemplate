using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AWSServerlessApp.CustomModels
{
    public class ApplicationUserListViewModel
    {
        [Display(Name = "User Email Address")]
        public string UserEmail { get; set; }

        public List<IdentityUserRole<string>> Roles { get; set; }
    }
}
