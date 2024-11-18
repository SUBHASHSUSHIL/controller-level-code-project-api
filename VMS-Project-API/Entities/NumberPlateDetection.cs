using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class NumberPlateDetection
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public Camera? Camera { get; set; }
        public string? FramePath { get; set; }
        public string? PlatePath { get; set; }
        public DateTime RegDate { get; set; }= DateTime.Now;
    }
}
