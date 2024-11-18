using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateActivityLogDTO
    {
        public int Id { get; set; }
        public string? ModuleName { get; set; }
        public string? Action { get; set; }
        public string? Data { get; set; }
    }
}
