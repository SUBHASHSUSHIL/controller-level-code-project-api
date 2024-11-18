using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VMS_Project_API.Entities
{
    [Index(nameof(CameraIP), IsUnique = true)]
    public class CameraIPList
    {
        [Key]
        public int Id { get; set; }
        public string? CameraIP { get; set; }
        public string? ObjectList { get; set; }
        public DateTime RegDate { get; set; }= DateTime.Now;
    }
}
