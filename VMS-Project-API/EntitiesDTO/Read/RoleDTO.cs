using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Read
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Status { get; set; }
    }
}
