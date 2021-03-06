﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CancerRegistry.Services;

namespace CancerRegistry.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly PatientService _patientService;
        private readonly AccountService _accountService;

        public PatientDashboardController(PatientService patientService, AccountService accountService)
        {
            _patientService = patientService;
            _accountService = accountService;
        }

        public IActionResult Home()
        => View("/Views/Dashboard/Patient/PatientDashboardHome.cshtml");
        
        [HttpGet]
        public async Task<IActionResult> ActiveDiagnose()
        {
            var patientId = _accountService.GetUserId(User);
            var model = await _patientService.GetActiveDiagnose(patientId);

            return View("/Views/Dashboard/Patient/CurrentDiagnose.cshtml", model);
        }

        public async Task<IActionResult> CurrentTreatment()
        {
            var patientId = _accountService.GetUserId(User);
            var model = await _patientService.GetCurrentTreatment(patientId);
            return View("/Views/Dashboard/Patient/CurrentTreatment.cshtml", model);
        }

    }
}
