using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class CameraRecordUpdateDTO
    {
        public int Id { get; set; }
        public string? RecordPath { get; set; }
        public bool Status { get; set; }
        public DateTime RegDate { get; set; }
    }
}
