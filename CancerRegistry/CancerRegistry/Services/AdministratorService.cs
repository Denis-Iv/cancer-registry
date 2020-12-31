﻿using CancerRegistry.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CancerRegistry.Services
{
    public class AdministratorService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdministratorService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            var users = new List<ApplicationUser>(doctors);
            users.AddRange(patients);
            return users;
        }

        public async Task<bool> LoginAdmin(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            if (!isAdmin) return false;

            await _signInManager.SignOutAsync();

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);
            return signInResult.Succeeded;
        }

        public async Task<bool> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<RegistrationResult> RegisterDoctor(
            string firstName,
            string lastName,
            string egn,
            string uid)
        {
            RegistrationResult registrationResult = new RegistrationResult();
            
            var doctorPassword = string.Concat(
                char.ToUpper(
                    firstName[0]),
                    lastName.First().ToString().ToUpper() + lastName.Substring(1),
                    "-",
                    egn);

            ApplicationUser appUser = new ApplicationUser()
            {
                UserName = uid,
                FirstName = firstName,
                LastName = lastName,
                EGN = egn,
                UID = uid
            };

            var registerResult = await _userManager.CreateAsync(appUser, doctorPassword);
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Doctor");

            if (registerResult.Succeeded && roleResult.Succeeded)
                return registrationResult;

            registrationResult.Succeeded = false;
            registrationResult.Errors = new List<string>();
            foreach (var error in registerResult.Errors)
                registrationResult.Errors.Add(error.Description);

            foreach (var error in roleResult.Errors)
                registrationResult.Errors.Add(error.Description);

            return registrationResult;
        }
    }
}
