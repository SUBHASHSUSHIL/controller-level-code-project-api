using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO
{
    public class CameraStatusDTO
    {
        public int Id { get; set; }
        public bool? IsRecording { get; set; }
        public bool? IsTracking { get; set; }
    }
}
