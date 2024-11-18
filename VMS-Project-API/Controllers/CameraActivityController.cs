using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.ImportDTO;
using VMS_Project_API.EntitiesDTO.Read;
using VMS_Project_API.EntitiesDTO.Update;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CameraActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateCameraActivityDTO> cameraActivityDTO)
        {
            if (cameraActivityDTO == null || !cameraActivityDTO.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var cameraActivity = cameraActivityDTO.Select(dto => new CameraActivity
                {
                    CameraId = dto.CameraId,
                    UserId = dto.UserId,
                    Activity = dto.Activity,
                    Status = dto.Status
                }).ToList();

                _context.CameraActivities.AddRange(cameraActivity);
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
                var cameraActivitie = await _context.CameraActivities.ToListAsync();
                return Ok(cameraActivitie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export-excel")]
        public async Task<IActionResult> Export()
        {
            try
            {
                var cameraActivities = await _context.CameraActivities
                    .Include(x => x.Camera)
                    .Include(x => x.User)
                    .Select(x => new CameraActivityDTO
                    {
                        Id = x.Id,
                        CameraId = x.CameraId,
                        CameraName = x.Camera.Name,
                        UserId = x.UserId,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Activity = x.Activity,
                        Status = x.Status,
                        RegDate = x.RegDate
                    })
                    .ToListAsync();

                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Id,CameraId,CameraName,UserId,UserName,Activity,Status,RegDate");

                foreach (var activity in cameraActivities)
                {
                    csvBuilder.AppendLine($"{activity.Id},{activity.CameraId},{activity.CameraName},{activity.UserId},{activity.UserName},{activity.Activity},{activity.Status},{activity.RegDate}");
                }

                var fileContent = csvBuilder.ToString();
                var fileBytes = Encoding.UTF8.GetBytes(fileContent);
                var fileStream = new MemoryStream(fileBytes);

                return File(fileStream, "text/csv", "CameraActivities.csv");
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
                var activities = await _context.CameraActivities
                    .Include(x => x.Camera)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new CameraActivityDTO
                    {
                        Id = x.Id,
                        CameraId = x.CameraId,
                        CameraName = x.Camera.Name,
                        UserId = x.UserId,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Activity = x.Activity,
                        Status = x.Status,
                        RegDate = x.RegDate
                    })
                    .ToListAsync();

                return Ok(activities);
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
                var totalCameraActivities = await _context.CameraActivities.CountAsync();
                var activeCameraActivities = await _context.CameraActivities.CountAsync(c => c.Status == true);
                var inactiveCameraActivities = await _context.CameraActivities.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalCameraActivities = totalCameraActivities,
                    ActiveCameraActivities = activeCameraActivities,
                    InactiveCameraActivities = inactiveCameraActivities
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
        public async Task<IActionResult> Pagination(
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
                var query = _context.CameraActivities.Include(i => i.Camera).AsQueryable();

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var activity = await _context.CameraActivities
                    .Include(x => x.Camera)
                    .Include(x => x.User)
                    .Where(x => x.Id == id)
                    .Select(x => new CameraActivityDTO
                    {
                        Id = x.Id,
                        CameraId = x.CameraId,
                        CameraName = x.Camera.Name,
                        UserId = x.UserId,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Activity = x.Activity,
                        Status = x.Status,
                        RegDate = x.RegDate
                    })
                    .FirstOrDefaultAsync();

                if (activity == null)
                    return NotFound();

                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [AllowAnonymous]
        [HttpGet("Camera-List/{cameraId}")]
        public async Task<IActionResult> CameraList(int cameraId)
        {
            try
            {
                var cameraActivities = await _context.CameraActivities.Include(x => x.Camera).OrderByDescending(x => x.Id)
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (cameraActivities == null || !cameraActivities.Any())
                {
                    return NotFound($"No records found for Camera ID: {cameraId}");
                }

                return Ok(cameraActivities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCameraActivityDTO cameraActivityDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cameraActivity = new CameraActivity
                {
                    CameraId = cameraActivityDTO.CameraId,
                    UserId = cameraActivityDTO.UserId,
                    Activity = cameraActivityDTO.Activity,
                    Status = cameraActivityDTO.Status
                };

                await _context.CameraActivities.AddAsync(cameraActivity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = cameraActivity.Id }, cameraActivity);
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

        private bool CActivityExists(int id)
        {
            try
            {
                return _context.CameraActivities.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking camera activity existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCameraActivityDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cameraActivity = await _context.CameraActivities.FindAsync(updateDTO.Id);
                if (cameraActivity == null)
                {
                    return NotFound();
                }

                cameraActivity.CameraId = updateDTO.CameraId;
                cameraActivity.UserId = updateDTO.UserId;
                cameraActivity.Activity = updateDTO.Activity;
                cameraActivity.Status = updateDTO.Status;

                _context.CameraActivities.Update(cameraActivity);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.CameraActivities.Any(e => e.Id == updateDTO.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating camera activity: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusCameraActivityDTO cameraActivityDTO)
        {
            if (cameraActivityDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.CameraActivities.FindAsync(cameraActivityDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = cameraActivityDTO.Status;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while updating status: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCActivity(int id)
        {
            try
            {
                var cas = await _context.CameraActivities.FindAsync(id);
                if (cas == null)
                {
                    return NotFound();
                }

                _context.CameraActivities.Remove(cas);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting camera activity: {ex.Message}");
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
                var records = await _context.CameraActivities.ToListAsync();

                if (records.Any())
                {
                    _context.CameraActivities.RemoveRange(records);
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
        
        [HttpDelete("Clear/{cameraId}")]
        public async Task<IActionResult> ClearRecords(int cameraId)
        {
            try
            {
                var records = await _context.CameraActivities
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (records.Any())
                {
                    _context.CameraActivities.RemoveRange(records);
                    await _context.SaveChangesAsync();
                    return Ok($"All records for Camera ID: {cameraId} have been cleared.");
                }
                else
                {
                    return NotFound($"No records found for Camera ID: {cameraId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
