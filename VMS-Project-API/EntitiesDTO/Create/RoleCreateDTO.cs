using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class RoleCreateDTO
    {
        public string? Name { get; set; }
        public bool Status { get; set; } = true;
    }
}
