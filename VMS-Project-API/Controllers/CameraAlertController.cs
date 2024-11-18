using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.Update;

namespace VMS_Project_API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CameraAlertController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraAlertController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateCameraAlertDTO> createCameraAlerts)
        {
            if (createCameraAlerts == null || !createCameraAlerts.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var cameraAlerts = createCameraAlerts.Select(dto => new CameraAlert
                {
                    CameraId = dto.CameraId,
                    FramePath = dto.FramePath,
                    ObjectName = dto.ObjectName,
                    AlertStatus = dto.AlertStatus,
                    Status = dto.Status
                }).ToList();

                _context.cameraAlerts.AddRange(cameraAlerts);
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
                var cameraAlerts = await _context.cameraAlerts.ToListAsync();
                return Ok(cameraAlerts);
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
                var alerts = await _context.cameraAlerts.Include(i => i.Camera).OrderByDescending(x => x.Id).ToListAsync();
                var csv = new StringBuilder();

                csv.AppendLine("Id,CameraId,FramePath,ObjectName,ObjectCount,AlertStatus,Status,RegDate");

                foreach (var alert in alerts)
                {
                    var line = $"{alert.Id},{alert.CameraId},{alert.FramePath},{alert.ObjectName},{alert.ObjectCount},{alert.AlertStatus},{alert.Status},{alert.RegDate}";
                    csv.AppendLine(line);
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                var stream = new MemoryStream(bytes);

                return File(stream, "text/csv", "CameraAlerts.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var alerts = await _context.cameraAlerts.Include(i => i.Camera).OrderByDescending(x => x.Id).ToListAsync();
                return Ok(alerts);
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
                var totalCameraAlert = await _context.cameraAlerts.CountAsync();
                var activeCameraAlert = await _context.cameraAlerts.CountAsync(c => c.Status == true);
                var inActiveCameraAlert = await _context.cameraAlerts.CountAsync(c => c.Status == false);

                var result = new
                { 
                    TotalCameraAlert = totalCameraAlert,
                    ActiveCameraAlert = activeCameraAlert,
                    InActiveCameraAlert = inActiveCameraAlert
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
                var query = _context.cameraAlerts.Include(i => i.Camera).AsQueryable();

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
                var alert = await _context.cameraAlerts.Include(i => i.Camera).OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (alert == null)
                {
                    return NotFound();
                }
                return Ok(alert);
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
                var alerts = await _context.cameraAlerts.Include(i => i.Camera).OrderByDescending(x => x.Id)
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (alerts == null || !alerts.Any())
                {
                    return NotFound($"No records found for Camera ID: {cameraId}");
                }

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCameraAlertDTO cameraAlertDTO)
        {
            if (cameraAlertDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraAlert = new CameraAlert
                {
                    CameraId = cameraAlertDTO.CameraId,
                    FramePath = cameraAlertDTO.FramePath,
                    ObjectName = cameraAlertDTO.ObjectName,
                    ObjectCount = cameraAlertDTO.ObjectCount,
                    AlertStatus = cameraAlertDTO.AlertStatus,
                    Status = cameraAlertDTO.Status
                };

                await _context.cameraAlerts.AddAsync(cameraAlert);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = cameraAlert.Id }, cameraAlert);
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

        private bool CameraAlertExists(int id)
        {
            try
            {
                return _context.cameraAlerts.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking camera alert existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCameraAlertDTO cameraAlertDTO)
        {
            if (cameraAlertDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraAlert = await _context.cameraAlerts.FindAsync(cameraAlertDTO.Id);
                if (cameraAlert == null)
                {
                    return NotFound();
                }

                cameraAlert.FramePath = cameraAlertDTO.FramePath;
                cameraAlert.ObjectName = cameraAlertDTO.ObjectName;
                cameraAlert.ObjectCount = cameraAlertDTO.ObjectCount;
                cameraAlert.AlertStatus = cameraAlertDTO.AlertStatus;
                cameraAlert.Status = cameraAlertDTO.Status;

                _context.cameraAlerts.Update(cameraAlert);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CameraAlertExists(cameraAlertDTO.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating camera alert: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusCameraAlertDTO cameraAlertDTO)
        {
            if (cameraAlertDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraAlert = await _context.cameraAlerts.FindAsync(cameraAlertDTO.Id);
                if (cameraAlert == null)
                {
                    return NotFound();
                }

                cameraAlert.Status = cameraAlertDTO.Status;
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

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCTD(int id)
        {
            try
            {
                var cameraAlert = await _context.cameraAlerts.FindAsync(id);
                if (cameraAlert == null)
                {
                    return NotFound();
                }

                _context.cameraAlerts.Remove(cameraAlert);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting camera alert: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearRecords()
        {
            try
            {
                var records = await _context.cameraAlerts.ToListAsync();

                if (records.Any())
                {
                    _context.cameraAlerts.RemoveRange(records);
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