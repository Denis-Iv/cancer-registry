using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CancerRegistry.Identity;
using System.Net.Mail;

namespace CancerRegistry.Services.Email
{
    public class EmailHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailHelper(UserManager<ApplicationUser> userManager)
           => _userManager = userManager;
        public async Task<String> GeneratePasswordReset(String userEmail, IUrlHelper urlHelper, String scheme)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confirmationLink = urlHelper.Action("ResetPassword", "Email", new { token = token, email = user.Email }, scheme);

            return confirmationLink;
        }

        
    }
}
