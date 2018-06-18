using AWSServerlessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AWSServerlessApp
{
    public static class RoleInitializer
    {
        public static async Task Initialize(RoleManager<ApplicationRole> _roleManager)
        {
            if(!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new ApplicationRole("Admin");
                await _roleManager.CreateAsync(role);
            }
            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                var role = new ApplicationRole("Manager");
                await _roleManager.CreateAsync(role);
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new ApplicationRole("User");
                await _roleManager.CreateAsync(role);
            }
            if (!await _roleManager.RoleExistsAsync("Anonymous"))
            {
                var role = new ApplicationRole("Anonymous");
                await _roleManager.CreateAsync(role);
            }
        }
    }
}
