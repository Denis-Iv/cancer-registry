using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CancerRegistry.Identity;

namespace CancerRegistry.Services.Email
{
    public class EmailService
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<Boolean> SendEmailPasswordReset(String userEmail, String confirmationLink)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("teststproject1@gmail.com", "4}vwny9>By>)wFR");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("teststproject1@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));
            mailMessage.Subject = "Confirm your email";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "<a href=\"" + confirmationLink +"\">Link</a>" ;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch(Exception ex) { }
            return false;

        }
           
    }
}
