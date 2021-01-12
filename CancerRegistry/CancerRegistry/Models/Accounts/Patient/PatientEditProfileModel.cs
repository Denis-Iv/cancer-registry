﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CancerRegistry.Models.Accounts.Patient
{
    public class PatientEditProfileModel
    {
        public String Id { get; set; }

        [Required(ErrorMessage = "Полето \"Име\" е задължнително.")]
        public String FirstName { get; set; }
        
        [Required(ErrorMessage = "Полето \"Фамилия\" е задължително.")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Полето \"ЕГН\" е задължително.")]
        public String EGN { get; set; }
        
        [Required(ErrorMessage = "Полето \"Телефонен номер\" е задължително."), DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public String Gender { get; set; }

        public String[] Genders = new String[] {"Мъж", "Жена"};
    }
}
