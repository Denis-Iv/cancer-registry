﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CancerRegistry.Services;
using NUnit.Framework;

namespace CancerRegistryTests
{
    [TestFixture]
    class AccountServiceIntegrationTests
    {
        private ServiceCollectionFactory _services;

        [Test]
        [TestCase("1234", "qwer")]
        public async Task LoginPatient_WhenPatientSuccessfullyLogsIn_ReturnsTrue(string username, string password)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);
            var result = await accountService.LoginUser(username, password);
            var user = await _services.UserManager.FindByNameAsync(username);
            var isPatient = await _services.UserManager.IsInRoleAsync(user, "Patient");
            
            Assert.True(isPatient);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("123", "Password98!")]
        [TestCase("5647", "Pass36!")]
        public async Task LoginPatient_WhenPatientFailsToLogIn_ReturnsFalse(string username, string password)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);
            var result = await accountService.LoginUser(username, password);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("Qnko", "Qnkov", "9505131234", "0965342312", "Password123!")]
        [TestCase("Ivaylo", "Petrov", "9302236456", "0965235132", "Password456!")]
        public async Task RegisterPatient_WhenPatientSuccessfullyRegisters_NoRegistrationErrors(
            string firstName,
            string lastName,
            string egn,
            string phoneNumber,
            string password)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);

            var result = await accountService.RegisterPatient(firstName, lastName, egn, phoneNumber, password);
            var users = _services.UserManager.Users.ToList();

            Assert.IsNull(result.Errors);
            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(users.Count > 3);
        }

        [Test]
        [TestCase("Qnko", "Qnkov", "1234", "0965342312", "Password123!")]
        [TestCase("Ivaylo", "Petrov", "5678", "0965235132", "Password456!")]
        public async Task RegisterPatient_WhenPatientFailsToRegister_WithRegistrationErrors(
            string firstName,
            string lastName,
            string egn,
            string phoneNumber,
            string password)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);

            var result = await accountService.RegisterPatient(firstName, lastName, egn, phoneNumber, password);

            Assert.IsNotNull(result.Errors);
            Assert.IsFalse(result.Succeeded);
        }
        
        [Test]
        [TestCase("1","Kamen", "Ivanov","1234","0878992254", "12/05/1999","Мъж")]
        public async Task EditPatientProfile_SuccessfullEdit_ReturnsTrue(
            string id,
            string firstName,
            string lastName,
            string egn,
            string phoneNumber,
            DateTime birthDate,
            string gender)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);

            var result = await accountService.Edit(id, firstName, lastName, egn, phoneNumber, birthDate, gender);
            
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("1","Kamen", "Ivanov", "5678", "0878992254", "12/05/1999", "Мъж")]
        public async Task EditPatientProfile_FailedEdit_ReturnsFalse(
            string id,
            string firstName,
            string lastName,
            string egn,
            string phoneNumber,
            DateTime birthDate,
            string gender)
        {
            var accountService = new AccountService(_services.UserManager, _services.SignInManager);

            var result = await accountService.Edit(id, firstName, lastName, egn, phoneNumber, birthDate, gender);

            Assert.IsFalse(result);
        }
        
        [SetUp]
        public async Task Setup()
        {
            _services = await ServiceCollectionFactory.CreateAsync();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceCollectionFactory.Destroy(_services);
        }
    }
}
