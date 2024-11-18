using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.ImportDTO
{
    public class ImportCameraActivityDTO
    {
        public int CameraId { get; set; }
        public int UserId { get; set; }
        public string Activity { get; set; }
        public bool Status { get; set; }
    }
}
