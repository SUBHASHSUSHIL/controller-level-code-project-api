using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateNvrDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NVRIP { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? NVRType { get; set; }
        public string? Model { get; set; }
        public bool Status { get; set; } = true;
    }
}
