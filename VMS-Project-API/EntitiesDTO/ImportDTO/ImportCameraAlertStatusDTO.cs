using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.ImportDTO
{
    public class ImportCameraAlertStatusDTO
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public bool Recording { get; set; }
        public bool ANPR { get; set; }
        public bool Snapshot { get; set; }
        public bool PersonDetection { get; set; }
        public bool FireDetection { get; set; }
        public bool AnimalDetection { get; set; }
        public bool BikeDetection { get; set; }
        public bool MaskDetection { get; set; }
        public bool UmbrelaDetection { get; set; }
        public bool BrifecaseDetection { get; set; }
        public bool GarbageDetection { get; set; }
        public bool WeaponDetection { get; set; }
        public bool WrongDetection { get; set; }
        public bool QueueDetection { get; set; }
        public bool SmokeDetection { get; set; }
    }
}
