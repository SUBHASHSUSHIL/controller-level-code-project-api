using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;

namespace VMS_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoAnalyticController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VideoAnalyticController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.tbl_VideoAnalytic.Include(x => x.Camera).ToListAsync();
                return Ok(ucp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetActive")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var activeRecords = await _context.CameraAlertStatuss
                    .Where(x => x.Status == true)
                    .ToListAsync();

                return Ok(activeRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("Post")]
        public async Task<IActionResult> Create(CreateVideoAnalyticsDTO activityLogDto)
        {
            try
            {
                var existingRecord = await _context.tbl_VideoAnalytic
                    .FirstOrDefaultAsync(x => x.CameraIP == activityLogDto.CameraIP);

                if (existingRecord != null)
                {
                    // Agar record mil jata hai, toh update karte hain
                    existingRecord.CameraId = activityLogDto.CameraId;
                    existingRecord.RTSPUrl = activityLogDto.RTSPUrl;
                    existingRecord.ObjectList = activityLogDto.ObjectList;
                    existingRecord.Status = activityLogDto.Status;

                    _context.tbl_VideoAnalytic.Update(existingRecord);  // Record ko update karo
                    await _context.SaveChangesAsync();  // Changes save karo

                    return Ok(existingRecord);  // Updated record return karo
                }
                else
                {
                    // Agar koi record nahi milta, toh naya record insert karo
                    var newRecord = new VideoAnalytics
                    {
                        CameraId = activityLogDto.CameraId,
                        CameraIP = activityLogDto.CameraIP,
                        RTSPUrl = activityLogDto.RTSPUrl,
                        ObjectList = activityLogDto.ObjectList,
                        Status = activityLogDto.Status
                    };

                    await _context.tbl_VideoAnalytic.AddAsync(newRecord);  // Naya record add karo
                    await _context.SaveChangesAsync();  // Changes save karo

                    return Ok(newRecord);  // Naya record return karo
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving changes: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        //[HttpPost("Post")]
        //public async Task<IActionResult> Create(CreateVideoAnalyticsDTO activityLogDto)
        //{
        //    try
        //    {
        //        var activityLog = new VideoAnalytics
        //        {
        //            CameraId = activityLogDto.CameraId,
        //            CameraIP = activityLogDto.CameraIP,
        //            RTSPUrl = activityLogDto.RTSPUrl,
        //            ObjectList = activityLogDto.ObjectList,
        //            Status = activityLogDto.Status
        //        };

        //        await _context.tbl_VideoAnalytic.AddAsync(activityLog);
        //        await _context.SaveChangesAsync();

        //        return Ok(activityLog);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return StatusCode(500, $"An error occurred while saving changes: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}


    }
}
