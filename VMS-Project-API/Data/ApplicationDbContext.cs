using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Read;
using VMS_Project_API.Model;

namespace VMS_Project_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        public DbSet<User_Camera_Permission> user_Camera_Permissions { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<LicenseActivation> LicenseActivations { get; set; }
        public DbSet<CameraActivity> CameraActivities { get; set; }
        public DbSet<CameraTrackingData> CameraTrackingDatas { get; set; }
        public DbSet<CameraRecord> CameraRecords { get; set; }
        public DbSet<MultCameraDTO> multCameras { get; set; }
        public DbSet<CameraAlertStatus> CameraAlertStatuss { get; set; }
        public DbSet<NVR> NVR { get; set; }
        public DbSet<CameraAlert> cameraAlerts { get; set; }
        public DbSet<AlertMaster> AlertMasters { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Features> Featuress { get; set; }
        public DbSet<ProfileAndFeatures> ProfileAndFeaturess { get; set; }
        public DbSet<ReadedVehicleNoPlate> tbl_ReadedVehicleNoPlate { get; set; }
        public DbSet<VideoAnalytics> tbl_VideoAnalytic { get; set; }
        //public async Task<VideoAnalytics> AddVideoAnalytics(VideoAnalytics videoAnalytics)
        //{
        //    var camera = await Cameras
        //        .Include(c => c.CameraIP) 
        //        .FirstOrDefaultAsync(c => c.Id == videoAnalytics.CameraId);

        //    if (camera == null || !camera.Status)
        //    {
        //        throw new ArgumentException("Camera not found or inactive.");
        //    }

        //    var cameraIPList = await tbl_CameraIPList
        //        .FirstOrDefaultAsync(c => c.CameraIP == camera.CameraIP && c.ObjectList == videoAnalytics.ObjectList);

        //    if (cameraIPList == null)
        //    {
        //        throw new ArgumentException("CameraIP or ObjectList not found in CameraIPList.");
        //    }

        //    tbl_VideoAnalytic.Add(videoAnalytics);
        //    await SaveChangesAsync();
        //    return videoAnalytics;
        //}

        public DbSet<CameraIPList> tbl_CameraIPList { get; set; }
        public DbSet<ANPRStatus> tbl_ANPRStatus { get; set; }
        public DbSet<NumberPlateDetection> tbl_NumberPlateDetection { get; set; }
    }
}
