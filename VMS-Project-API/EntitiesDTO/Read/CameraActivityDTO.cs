using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Read
{
    public class CameraActivityDTO
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Activity { get; set; }
        public bool Status { get; set; }
        public DateTime RegDate { get; set; }
    }
}
