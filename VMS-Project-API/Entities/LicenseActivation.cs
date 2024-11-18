using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class LicenseActivation
    {
        [Key]
        public int Id { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }
        public License? License { get; set; }
        public int LicenseId { get; set; }
        public string? MachineIP { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Status { get; set; } = true;
        public DateTime RegDate { get; set; } = DateTime.Now;
    }
}
