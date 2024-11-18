using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;

namespace VMS_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectionVehicleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DetectionVehicleController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet("Get")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.tbl_ReadedVehicleNoPlate.ToListAsync();
                return Ok(ucp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Count")]
        public async Task<IActionResult> GetCountStats()
        {
            try
            {
                var totalActivityLog = await _context.tbl_ReadedVehicleNoPlate.CountAsync();

                var result = new
                {
                    TotalDetection = totalActivityLog,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Pagination")]
        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            int? cameraId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? orderBy = null,
            string orderType = "asc")
        {
            try
            {
                var query = _context.tbl_ReadedVehicleNoPlate.Include(i => i.Camera).AsQueryable();

                if (cameraId.HasValue)
                {
                    query = query.Where(x => x.CameraId == cameraId.Value);
                }

                if (dateFrom.HasValue)
                {
                    query = query.Where(x => x.RegDate >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    query = query.Where(x => x.RegDate <= dateTo.Value);
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    bool isDescending = orderType.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    switch (orderBy.ToLower())
                    {
                        case "cameraid":
                            query = isDescending ? query.OrderByDescending(x => x.CameraId) : query.OrderBy(x => x.CameraId);
                            break;
                        case "regdate":
                            query = isDescending ? query.OrderByDescending(x => x.RegDate) : query.OrderBy(x => x.RegDate);
                            break;
                        default:
                            query = isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                            break;
                    }
                }
                else
                {
                    query = orderType.Equals("desc", StringComparison.OrdinalIgnoreCase)
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                }

                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CameraAlertStatuses = pagedData
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cmrs = await _context.tbl_ReadedVehicleNoPlate.FirstOrDefaultAsync(u => u.Id == id);
                if (cmrs == null)
                {
                    return NotFound();
                }
                return Ok(cmrs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("Post")]
        public async Task<IActionResult> Create(CreateVehicleDetectionDTO activityLogDto)
        {
            try
            {
                var activityLog = new ReadedVehicleNoPlate
                {
                    FramePath = activityLogDto.FramePath,
                    PlatePath = activityLogDto.PlatePath,
                    Text = activityLogDto.Text,
                    CameraId = activityLogDto.CameraId
                };

                await _context.tbl_ReadedVehicleNoPlate.AddAsync(activityLog);
                await _context.SaveChangesAsync();

                return Ok(activityLog);
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


        private bool CMRSExists(int id)
        {
            try
            {
                return _context.tbl_ReadedVehicleNoPlate.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking role existence: {ex.Message}");
            }
        }

        [HttpPut("Put")]
        public async Task<IActionResult> UpdateCMRS([FromBody] CreateVehicleDetectionDTO updateDto)
        {
            try
            {
                if (updateDto.Id < 1)
                {
                    return BadRequest("Invalid ID.");
                }

                var existingActivityLog = await _context.tbl_ReadedVehicleNoPlate.FindAsync(updateDto.Id);
                if (existingActivityLog == null)
                {
                    return NotFound("Activity Log not found.");
                }

                if (updateDto.FramePath != null)
                    existingActivityLog.FramePath = updateDto.FramePath;

                if (updateDto.PlatePath != null)
                    existingActivityLog.PlatePath = updateDto.PlatePath;

                if (updateDto.CameraId != null)
                    existingActivityLog.CameraId = updateDto.CameraId;

                if (updateDto.Text != null)
                    existingActivityLog.Text = updateDto.Text;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CMRSExists(updateDto.Id))
                {
                    return NotFound("Activity Log not found.");
                }
                else
                {
                    throw new Exception($"An error occurred while updating role: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCMRS(int id)
        {
            try
            {
                var cmrs = await _context.tbl_ReadedVehicleNoPlate.FindAsync(id);
                if (cmrs == null)
                {
                    return NotFound();
                }

                _context.tbl_ReadedVehicleNoPlate.Remove(cmrs);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting role: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearRecords()
        {
            try
            {
                var records = await _context.tbl_ReadedVehicleNoPlate.ToListAsync();

                if (records.Any())
                {
                    _context.tbl_ReadedVehicleNoPlate.RemoveRange(records);
                    await _context.SaveChangesAsync();
                    return Ok("All records have been cleared.");
                }
                else
                {
                    return NotFound("No records found to clear.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
