﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class LoginModel
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
    }
}
