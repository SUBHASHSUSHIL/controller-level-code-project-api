using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class LicenseCreateDTO
    {
        public string? Name { get; set; }
        public string? LicenseKey { get; set; }
        public string? ProductCode { get; set; }
        public int Days { get; set; }
        public int TotalPC { get; set; }
        public int TotalCamera { get; set; }
        public string? Description { get; set; }
    }
}
