using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class ProfileName
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public ProfileType? ProfileType { get; set; }
        public int ProfileId { get; set; }
        public string? ReMarks { get; set; }
    }
}
