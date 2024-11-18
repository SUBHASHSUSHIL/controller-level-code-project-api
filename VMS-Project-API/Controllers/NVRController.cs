using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.Update;
using System.Text;
using System.Text.Json;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NVRController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NVRController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJson([FromBody] List<CreateNvrDTO> nvrs)
        {
            if (nvrs == null || nvrs.Count == 0)
            {
                return BadRequest("Invalid or empty JSON data.");
            }

            try
            {
                var nvrEntities = nvrs.Select(dto => new NVR
                {
                    Name = dto.Name,
                    NVRIP = dto.NVRIP,
                    Port = dto.Port,
                    Username = dto.Username,
                    Password = dto.Password,
                    NVRType = dto.NVRType,
                    Status = dto.Status
                }).ToList();

                await _context.NVR.AddRangeAsync(nvrEntities);
                await _context.SaveChangesAsync();

                return Ok("Data imported successfully from JSON.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export-json")]
        public async Task<IActionResult> ExportJson()
        {
            try
            {
                var nvrs = await _context.NVR.OrderByDescending(x => x.Id).ToListAsync();
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true // Pretty print JSON
                };
                var json = JsonSerializer.Serialize(nvrs, jsonOptions);
                return Ok(json);
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
                var nvr = await _context.NVR.ToListAsync();

                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture));
                csv.WriteRecords(nvr);
                var csvContent = writer.ToString();

                return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "nvr.csv");
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
                var ucp = await _context.NVR.OrderByDescending(x=>x.Id).ToListAsync();
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
                var totalNVR = await _context.NVR.CountAsync();
                var activeNVR = await _context.NVR.CountAsync(c => c.Status == true);
                var inactiveNVR = await _context.NVR.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalNVR = totalNVR,
                    ActiveNVR = activeNVR,
                    InactiveNVR = inactiveNVR
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
                var query = _context.NVR.OrderByDescending(x => x.Id).AsQueryable();

                var pagedCameraActivities = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CameraActivities = pagedCameraActivities
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
                var cas = await _context.NVR.FirstOrDefaultAsync(u => u.Id == id);
                if (cas == null)
                {
                    return NotFound();
                }
                return Ok(cas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNvrDTO createNvrDTO)
        {
            if (createNvrDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var nvr = new NVR
                {
                    Name = createNvrDTO.Name,
                    NVRIP = createNvrDTO.NVRIP,
                    Port = createNvrDTO.Port,
                    Username = createNvrDTO.Username,
                    Password = createNvrDTO.Password,
                    NVRType = createNvrDTO.NVRType,
                    Status = createNvrDTO.Status
                };

                await _context.NVR.AddAsync(nvr);
                await _context.SaveChangesAsync();
                return Ok(nvr);
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNvrDTO updateNvrDTO)
        {
            if (updateNvrDTO == null || updateNvrDTO.Id < 1)
            {
                return BadRequest();
            }

            try
            {
                var nvr = await _context.NVR.FindAsync(updateNvrDTO.Id);
                if (nvr == null)
                {
                    return NotFound();
                }

                nvr.Name = updateNvrDTO.Name;
                nvr.NVRIP = updateNvrDTO.NVRIP;
                nvr.Port = updateNvrDTO.Port;
                nvr.Username = updateNvrDTO.Username;
                nvr.Password = updateNvrDTO.Password;
                nvr.NVRType = updateNvrDTO.NVRType;
                nvr.Status = updateNvrDTO.Status;

                _context.NVR.Update(nvr);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"An error occurred while updating NVR: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusNvrDTO nvrDTO)
        {
            if (nvrDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.NVR.FindAsync(nvrDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = nvrDTO.Status;
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
                var cas = await _context.NVR.FindAsync(id);
                if (cas == null)
                {
                    return NotFound();
                }

                _context.NVR.Remove(cas);
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
    }
}
