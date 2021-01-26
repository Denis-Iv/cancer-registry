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
using CancerRegistry.Models.Accounts;

namespace CancerRegistry.Services.Email
{
    public class EmailService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private AccountService accountService;

        public EmailService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<Boolean> SendEmailPasswordReset(String userEmail, String token, AccountService accountService)
        {
            this.accountService = accountService;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("teststproject1@gmail.com", "4}vwny9>By>)wFR");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("teststproject1@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));
            mailMessage.Subject = "Reset your password";
            mailMessage.IsBodyHtml = true;
            
            
            var password = GeneratePassword(true, true, true, true, 16);
            mailMessage.Body = "<p>Нова парола: " + password + "</p>";

            try
            {
                smtpClient.Send(mailMessage);
                var user = await _userManager.FindByEmailAsync(userEmail);
                await this.accountService.ResetPassword(token, user.UserName, password);
                return true;
            }
            catch(Exception ex) { }
            return false;

        }


        const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "123456789";
        const string SPECIALS = @"!@£$%^&*()#€";


        public string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial,
            int passwordSize)
        {
            char[] _password = new char[passwordSize];
            string charSet = ""; 
            System.Random _random = new Random();
            int counter;

            
            if (useLowercase) charSet += LOWER_CASE;

            if (useUppercase) charSet += UPPER_CAES;

            if (useNumbers) charSet += NUMBERS;

            if (useSpecial) charSet += SPECIALS;

            for (counter = 0; counter < passwordSize; counter++)
            {
                _password[counter] = charSet[_random.Next(charSet.Length - 1)];
            }

            return String.Join(null, _password);
        }
    }
}
