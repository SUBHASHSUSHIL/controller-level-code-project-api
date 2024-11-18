using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace VMS_Project_API.Entities
{
    public class Camera
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CameraIP { get; set; }
        public string? Area { get; set; }
        public string? Location { get; set; }
        public NVR? NVR { get; set; }
        public int NVRId { get; set; }
        public Group? Group { get; set; }
        public int GroupId { get; set; }
        public string? Brand { get; set; }
        public string? Manufacture { get; set; }
        public string? MacAddress { get; set; }
        public int? Port { get; set; }
        public int? ChannelId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? LastLive { get; set; }
        public string? RTSPURL { get; set; }
        public int? PinCode { get; set; }
        public bool? isRecording { get; set; }
        public bool? isStreaming { get; set; }
        public bool? isANPR { get; set; }
        public bool Status { get; set; } = true; 
        public DateTime? UpdateDate { get; set; }
        public DateTime RegDate { get; set; }= DateTime.Now;
    }
}
