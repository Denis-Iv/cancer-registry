﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CancerRegistry.Models.Accounts.Patient
{
    public class PatientRegisterModel
    {
        [Required(ErrorMessage = "Полето \"Име\" е задължнително.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето \"Фамилия\" е задължително.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Полето \"ЕГН\" е задължително."), StringLength(10, MinimumLength = 10, ErrorMessage = "Въведеното ЕГН е невалидно.")]
        public string EGN { get; set; }

        [Required(ErrorMessage = "Полето \"Имейл\" е задължително.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Полето \"Телефонен номер\" е задължително."), DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна."), DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string RepeatPassoword { get; set; }
        
        public string TabSelected { get; set; }
    }
}
