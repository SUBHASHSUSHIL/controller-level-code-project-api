using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS_Project_API.Entities;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateANPRStatus
    {
        public int CameraId { get; set; }
        public string? CameraName { get; set; }
        public string? URL { get; set; }
        public bool Status { get; set; }
    }
}
