using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateCameraAlertDTO
    {
        public int CameraId { get; set; }
        public string? FramePath { get; set; }
        public string? ObjectName { get; set; }
        public int? ObjectCount { get; set; }
        public char? AlertStatus { get; set; }
        public bool Status { get; set; } = true;
    }
}
