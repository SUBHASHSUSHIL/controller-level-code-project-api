using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Model
{
    public class ApplicationUser:IdentityUser
    {
        public bool Status {  get; set; }
    }
}
