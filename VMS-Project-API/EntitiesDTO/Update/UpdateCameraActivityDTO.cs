﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS_Project_API.EntitiesDTO.Update
{
    public class UpdateCameraActivityDTO
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public int UserId { get; set; }
        public string? Activity { get; set; }
        public bool Status { get; set; }
    }
}
