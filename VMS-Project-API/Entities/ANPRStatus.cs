using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class ANPRStatus
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public Camera? Camera { get; set; }
        public string? CameraName { get; set; }
        public string? URL { get; set; }
        public bool Status { get; set; }
        public DateTime RegDate { get; set; } = DateTime.Now;
    }
}
