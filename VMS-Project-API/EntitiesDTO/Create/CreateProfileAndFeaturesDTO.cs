using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Create
{
    public class CreateProfileAndFeaturesDTO
    {
        public int ProfileId { get; set; }
        public int FeatureId { get; set; }
        public bool Status { get; set; } = true;
    }
}
