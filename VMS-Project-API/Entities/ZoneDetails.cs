using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class ZoneDetails
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public ZoneType? ZoneType { get; set; }
        public int ZoneId { get; set; }
        public string? ReMarks { get; set; }
    }
}
