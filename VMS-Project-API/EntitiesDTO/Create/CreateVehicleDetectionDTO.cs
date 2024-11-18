using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateVehicleDetectionDTO
    {
        public int Id { get; set; }
        public string? FramePath { get; set; }
        public string? PlatePath { get; set; }
        public int CameraId { get; set; }
        public string? Text { get; set; }
    }
}
