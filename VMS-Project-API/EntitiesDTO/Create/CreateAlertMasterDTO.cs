using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateAlertMasterDTO
    {
        public int CameraId { get; set; }
        public string? AlertName { get; set; }
        public string? AlertInfo { get; set; }
        public char? AlertStatus { get; set; }
        public DateTime RegDate { get; set; } = DateTime.Now;
    }
}
