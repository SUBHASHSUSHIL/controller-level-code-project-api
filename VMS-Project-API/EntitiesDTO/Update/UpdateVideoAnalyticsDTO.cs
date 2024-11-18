using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS_Project_API.Entities;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateVideoAnalyticsDTO
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public string? RTSPUrl { get; set; }
        public string? ObjectList { get; set; }
    }
}
