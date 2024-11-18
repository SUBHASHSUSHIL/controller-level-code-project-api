using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }
        public string? ModuleName { get; set; }
        public string? Action { get; set; }
        public string? Data { get; set; }
        public DateTime RegData { get; set; }= DateTime.Now;
    }
}
