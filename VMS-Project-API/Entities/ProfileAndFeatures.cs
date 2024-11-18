using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.Entities
{
    public class ProfileAndFeatures
    {
        public int Id { get; set; }
        public Profile? Profile { get; set; }
        public int ProfileId { get; set; }
        public Features? Features { get; set; }
        public int FeatureId { get; set; }
        public bool Status { get; set; }=true;
        public DateTime RegDate { get; set; }=DateTime.Now;
    }
}
