using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateCameraAlertDTO
    {
        public int Id { get; set; }
        public string? FramePath { get; set; }
        public string? ObjectName { get; set; }
        public int? ObjectCount { get; set; }
        public char? AlertStatus { get; set; }
        public bool Status { get; set; } = true;
    }
}
