using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS_Project_API.Entities;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateNumberPlateDetectionDTO
    {
        public int CameraId { get; set; }
        public string? FramePath { get; set; }
        public string? PlatePath { get; set; }
    }
}
