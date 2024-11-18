using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.Update;
using System.Text;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LicenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<LicenseCreateDTO> licenseCreateDTOs)
        {
            if (licenseCreateDTOs == null || !licenseCreateDTOs.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var license = licenseCreateDTOs.Select(dto => new License
                {
                    Name = dto.Name,
                    LicenseKey = dto.LicenseKey,
                    ProductCode = dto.ProductCode,
                    Days = dto.Days,
                    TotalPC = dto.TotalPC,
                    TotalCamera = dto.TotalCamera,
                    Description = dto.Description
                }).ToList();

                _context.Licenses.AddRange(license);
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
                var license = await _context.Licenses.ToListAsync();
                return Ok(license);
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
                var licenses = await _context.Licenses.ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(licenses);
                    var csvContent = writer.ToString();
                    var byteArray = Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "Licenses.csv");
                }
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
                var ucp = await _context.Licenses.ToListAsync();
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
                var totalLicense = await _context.Licenses.CountAsync();
                var activeLicense = await _context.Licenses.CountAsync(c => c.Status == true);
                var inactiveLicense = await _context.Licenses.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalLicense = totalLicense,
                    ActiveLicense = activeLicense,
                    InactiveLicense = inactiveLicense
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
                var query = _context.Licenses.AsQueryable();

                var pagedLicenses = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Licenses = pagedLicenses
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
                var license = await _context.Licenses.FirstOrDefaultAsync(u => u.Id == id);
                if (license == null)
                {
                    return NotFound();
                }
                return Ok(license);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LicenseCreateDTO licenseDTO)
        {
            if (licenseDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var license = new License
                {
                    Name = licenseDTO.Name,
                    LicenseKey = licenseDTO.LicenseKey,
                    ProductCode = licenseDTO.ProductCode,
                    Days = licenseDTO.Days,
                    TotalPC = licenseDTO.TotalPC,
                    TotalCamera = licenseDTO.TotalCamera,
                    Description = licenseDTO.Description,
                    Status = true // Default value
                };

                await _context.Licenses.AddAsync(license);
                await _context.SaveChangesAsync();

                return Ok(license);
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


        private bool LicenseExists(int id)
        {
            try
            {
                return _context.Licenses.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking license existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLicense([FromBody] LicenseUpdateDTO licenseDTO)
        {
            if (licenseDTO == null || licenseDTO.Id < 1)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var license = await _context.Licenses.FindAsync(licenseDTO.Id);
                if (license == null)
                {
                    return NotFound();
                }

                license.Name = licenseDTO.Name;
                license.LicenseKey = licenseDTO.LicenseKey;
                license.ProductCode = licenseDTO.ProductCode;
                license.Days = licenseDTO.Days;
                license.TotalPC = licenseDTO.TotalPC;
                license.TotalCamera = licenseDTO.TotalCamera;
                license.Description = licenseDTO.Description;
                license.Status = licenseDTO.Status;

                _context.Entry(license).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!LicenseExists(licenseDTO.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating license: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusLicenseDTO licenseDTO)
        {
            if (licenseDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.Licenses.FindAsync(licenseDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = licenseDTO.Status;
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
        public async Task<IActionResult> DeleteLicense(int id)
        {
            try
            {
                var license = await _context.Licenses.FindAsync(id);
                if (license == null)
                {
                    return NotFound();
                }

                _context.Licenses.Remove(license);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting license: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
