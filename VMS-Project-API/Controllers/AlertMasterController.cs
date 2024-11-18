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
using VMS_Project_API.EntitiesDTO.Update;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateAlertMasterDTO> createAlertMaster)
        {
            if (createAlertMaster == null || !createAlertMaster.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var alertMaster = createAlertMaster.Select(dto => new AlertMaster
                {
                    CameraId = dto.CameraId,
                    AlertName = dto.AlertName,
                    AlertInfo = dto.AlertInfo,
                    AlertStatus = dto.AlertStatus,
                    RegDate = dto.RegDate
                }).ToList();

                _context.AlertMasters.AddRange(alertMaster);
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
                var alertMaster = await _context.AlertMasters.ToListAsync();
                return Ok(alertMaster);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export")]
        public async Task<IActionResult> ExportData()
        {
            try
            {
                var alertMasters = await _context.AlertMasters.Include(i => i.Camera).ToListAsync();

                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture));
                csv.WriteRecords(alertMasters);
                var csvContent = writer.ToString();

                return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "alertmasters.csv");
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
                var ucp = await _context.AlertMasters.Include(i => i.Camera).OrderByDescending(c => c.Camera).ToListAsync();
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
                var totalAlert = await _context.AlertMasters.CountAsync();

                var result = new
                {
                    TotalAlert = totalAlert,
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
                var query = _context.AlertMasters.Include(i => i.Camera).AsQueryable();

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
                var cmrs = await _context.AlertMasters.Include(i => i.Camera).FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<IActionResult> Create(CreateAlertMasterDTO dto)
        {
            try
            {
                var alertMaster = new AlertMaster
                {
                    CameraId = dto.CameraId,
                    AlertName = dto.AlertName,
                    AlertInfo = dto.AlertInfo,
                    AlertStatus = dto.AlertStatus,
                    RegDate = dto.RegDate
                };

                await _context.AlertMasters.AddAsync(alertMaster);
                await _context.SaveChangesAsync();
                return Ok(alertMaster);
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
                return _context.AlertMasters.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking role existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCMRS([FromBody] UpdateAlertMasterDTO dto)
        {
            try
            {
                if (dto.Id < 1)
                {
                    return BadRequest();
                }

                var alertMaster = await _context.AlertMasters.FindAsync(dto.Id);
                if (alertMaster == null)
                {
                    return NotFound();
                }

                alertMaster.CameraId = dto.CameraId;
                alertMaster.AlertName = dto.AlertName;
                alertMaster.AlertInfo = dto.AlertInfo;
                alertMaster.AlertStatus = dto.AlertStatus;
                alertMaster.RegDate = dto.RegDate;

                _context.AlertMasters.Update(alertMaster);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CMRSExists(dto.Id))
                {
                    return NotFound();
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
                var cmrs = await _context.AlertMasters.FindAsync(id);
                if (cmrs == null)
                {
                    return NotFound();
                }

                _context.AlertMasters.Remove(cmrs);
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
