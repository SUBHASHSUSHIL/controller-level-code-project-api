using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateCameraAlertStatusDTO
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public bool Recording { get; set; } = true;
        public bool ANPR { get; set; } = false;
        public bool Snapshot { get; set; } = false;
        public bool PersonDetection { get; set; } = false;
        public bool FireDetection { get; set; } = false;
        public bool AnimalDetection { get; set; } = false;
        public bool BikeDetection { get; set; } = false;
        public bool MaskDetection { get; set; } = false;
        public bool UmbrelaDetection { get; set; } = false;
        public bool BrifecaseDetection { get; set; } = false;
        public bool GarbageDetection { get; set; } = false;
        public bool WeaponDetection { get; set; } = false;
        public bool WrongDetection { get; set; } = false;
        public bool QueueDetection { get; set; } = false;
        public bool SmokeDetection { get; set; } = false;
    }
}
