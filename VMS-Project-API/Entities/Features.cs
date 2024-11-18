using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class Features
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public Profile? Profile { get; set; }
        public int ProfileId { get; set; }
        public bool Status { get; set; }=true;
        public DateTime RegDate { get; set; }=DateTime.Now;

    }
}
