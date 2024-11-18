using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.Update;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<ActivityLogDTO> activityLogDTOs)
        {
            if (activityLogDTOs == null || !activityLogDTOs.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var activityLog = activityLogDTOs.Select(dto => new ActivityLog
                {
                    UserId = dto.UserId,
                    ModuleName = dto.ModuleName,
                    Action = dto.Action,
                    Data = dto.Data
                }).ToList();

                _context.ActivityLogs.AddRange(activityLog);
                await _context.SaveChangesAsync();
                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export-json")]
        public async Task<IActionResult> ExportJsonData()
        {
            try
            {
                var activityLog = await _context.ActivityLogs.ToListAsync();
                return Ok(activityLog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportData()
        {
            try
            {
                var activityLogs = await _context.ActivityLogs.Include(i => i.User).ToListAsync();

                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture));
                csv.WriteRecords(activityLogs);
                var csvContent = writer.ToString();

                return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "activitylogs.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.ActivityLogs.Include(i => i.User).ToListAsync();
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
                var totalActivityLog = await _context.ActivityLogs.CountAsync();

                var result = new
                {
                    TotalActivityLog = totalActivityLog,
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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.ActivityLogs.Include(i => i.User).AsQueryable();

                var pagedRoles = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Roles = pagedRoles
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cmrs = await _context.ActivityLogs.Include(i=>i.User).FirstOrDefaultAsync(u => u.Id == id);
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

        [HttpPost]
        public async Task<IActionResult> Create(ActivityLogDTO activityLogDto)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = activityLogDto.UserId,
                    ModuleName = activityLogDto.ModuleName,
                    Action = activityLogDto.Action,
                    Data = activityLogDto.Data,
                    RegData = DateTime.Now
                };

                await _context.ActivityLogs.AddAsync(activityLog);
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
                return _context.ActivityLogs.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking role existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCMRS([FromBody] UpdateActivityLogDTO updateDto)
        {
            try
            {
                if (updateDto.Id < 1)
                {
                    return BadRequest("Invalid ID.");
                }

                var existingActivityLog = await _context.ActivityLogs.FindAsync(updateDto.Id);
                if (existingActivityLog == null)
                {
                    return NotFound("Activity Log not found.");
                }

                if (updateDto.ModuleName != null)
                    existingActivityLog.ModuleName = updateDto.ModuleName;

                if (updateDto.Action != null)
                    existingActivityLog.Action = updateDto.Action;

                if (updateDto.Data != null)
                    existingActivityLog.Data = updateDto.Data;

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
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCMRS(int id)
        {
            try
            {
                var cmrs = await _context.ActivityLogs.FindAsync(id);
                if (cmrs == null)
                {
                    return NotFound();
                }

                _context.ActivityLogs.Remove(cmrs);
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
    }
}
