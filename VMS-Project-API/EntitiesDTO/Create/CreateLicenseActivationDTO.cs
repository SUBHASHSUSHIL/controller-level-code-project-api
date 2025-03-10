﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateLicenseActivationDTO
    {
        public int UserId { get; set; }
        public int LicenseId { get; set; }
        public string? MachineIP { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Status { get; set; } = true;
    }
}
