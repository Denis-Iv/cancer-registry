﻿using CancerRegistry.Identity;
using CancerRegistry.Models.Accounts.Doctor;
using CancerRegistry.Models.Accounts.Patient;
using CancerRegistry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CancerRegistry.Models.Accounts;
using CancerRegistry.Services.Email;

namespace CancerRegistry.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly PatientService _patientService;
        private readonly EmailService _emailService;
        private readonly EmailHelper _emailHelper;

        public AccountController(AccountService accountService, PatientService patientService, EmailService emailService, EmailHelper emailHelper)
        {
            _accountService = accountService;
            _patientService = patientService;
            _emailService = emailService;
            _emailHelper = emailHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PatientSignInUp() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginPatient(PatientAccountWrapperModel model)
        {
            if (!ModelState.IsValid)
                return View("PatientSignInUp");

            var loginResult = await _accountService.LoginUser(model.LoginModel.EGN, model.LoginModel.Password);

            if (loginResult)
                return RedirectToAction("Home", "PatientDashboard");

            ModelState.AddModelError("", "Влизането неуспешно: грешно ЕГН или парола.");
            return View("PatientSignInUp");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DoctorSignIn() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginDoctor(DoctorLoginModel doctor)
        {
            if (!ModelState.IsValid)
                return View("DoctorSignIn");

            var loginResult = await _accountService.LoginUser(doctor.UID, doctor.Password);

            if (loginResult)
                return RedirectToAction("", "DoctorDashboard"); //Must redirect to doctor's dashboard

            ModelState.AddModelError("", "Влизането неуспешно: грешен УИН или парола.");
            return View("DoctorSignIn");
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutUser();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordByEmail([Required] string email)
        {
            var confirmationLink = await _emailHelper.GeneratePasswordReset(email, Url, Request.Scheme);
            var emailResult = await _emailService.SendEmailPasswordReset(email, confirmationLink);

            if (!emailResult)
            {
                ModelState.AddModelError("", "We couldn't sent you a password reset email link. Please try again");
                return View();
            }
            return RedirectToAction("PasswordReset");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPatient(PatientAccountWrapperModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegisterModel.TabSelected = "2";
                return View("PatientSignInUp", model);
            }
                

            var result = await _accountService.RegisterPatient(
                model.RegisterModel.FirstName,
                model.RegisterModel.LastName,
                model.RegisterModel.EGN,
                model.RegisterModel.Email,
                model.RegisterModel.PhoneNumber,
                model.RegisterModel.Password);

            if (result.Succeeded)
            {
                var user = await _accountService.GetUserByName(model.RegisterModel.EGN);
                await _patientService.AddPatient(user.Id, user.PhoneNumber);
                
                return RedirectToAction("Index", "Home");
            }           

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);
            model.RegisterModel.TabSelected = "2";
            
            return View("PatientSignInUp",  model);
        }

        [HttpGet]
        public async Task<IActionResult> PatientProfile(string id)
        {
            var patient = await _accountService.GetPatient(id);
            return View("PatientProfile", patient);
        }

        [HttpGet]
        public async Task<IActionResult> DoctorProfile(string id)
        {
            var doctor = await _accountService.GetDoctor(id);
            return View("DoctorProfile", doctor);
        }

        [HttpGet]
        public async Task<IActionResult> EditPatientProfile(string patientId)
        {
            var patient = await _accountService.GetPatient(patientId);

            var model = new PatientEditProfileModel()
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                EGN = patient.EGN,
                PhoneNumber = patient.PhoneNumber,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Genders = new string[] { "Mъж", "Жена" }
            };

            return View("EditProfilePatient", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPatientProfile(PatientEditProfileModel model)
        {
            if (!ModelState.IsValid)
                return View("EditProfilePatient", model);
            
            var result = await _accountService.EditPatient(model.Id, model.FirstName, model.LastName, model.EGN, model.PhoneNumber, model.BirthDate, model.Gender);

            if (result.Succeeded)
                return RedirectToAction("PatientProfile", "Account", new { id = model.Id });

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);
            
            return RedirectToAction("EditPatientProfile", "Account", model.Id);
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctorProfile(string doctorId)
        {
            var doctor = await _accountService.GetDoctor(doctorId);

            var model = new DoctorEditProfileModel()
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                EGN = doctor.EGN,
                PhoneNumber = doctor.PhoneNumber,
                Gender = doctor.Gender,
                BirthDate = doctor.BirthDate,
                Genders = new string[] { "Mъж", "Жена" }
            };

            return View("EditProfileDoctor", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctorProfile(DoctorEditProfileModel model)
        {
            var result = await _accountService.EditDoctor(model.Id, model.FirstName, model.LastName, model.EGN, model.PhoneNumber, model.BirthDate, model.Gender);

            if (result.Succeeded)
                return RedirectToAction("DoctorProfile", "Account", new { id = model.Id });
            
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);
            
            return RedirectToAction("EditDoctorProfile", "Account", new { doctorId = model.Id });
        }

        [HttpGet]
        public IActionResult ChangePassword(string accountId)
        {
            var model = new ChangePassword { AccountId = accountId };
            return View("ChangePassword", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ChangePassword(model.AccountId, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                var userRole = await _accountService.GetUserRole(model.AccountId);
                return RedirectToAction(userRole == "Patient" ? "PatientProfile" : "DoctorProfile", new {id = model.AccountId});
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string username)
        {
            if (!ModelState.IsValid)
                return View(username);

            var token = await _accountService.ForgotPassword(username);

            if (token == null)
            {
                ModelState.AddModelError("", "Потребителят не може да бъде намерен.");
                return View("ForgotPassword");
            }


            return RedirectToAction("PasswordReset", new { token = token, username = username });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordReset(string token, string username)
            => View(new PasswordReset() { Token = token, Username = username });

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset(PasswordReset passwordReset)
        {
            if (!ModelState.IsValid)
                return View(passwordReset);

            var pswResetResult = await _accountService.ResetPassword(
                passwordReset.Token,
                passwordReset.Username,
                passwordReset.Password);

            if (pswResetResult.Succeeded) return View("PasswordResetSuccess");
            
            foreach (var err in pswResetResult.Errors)
                ModelState.AddModelError("", err);
            return View();

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() => View();

        public IActionResult AccessDenied()
            => View();
    }
}
