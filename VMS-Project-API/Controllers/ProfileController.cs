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

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateProfileDTO> createProfileDTOs)
        {
            if (createProfileDTOs == null || !createProfileDTOs.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var profile = createProfileDTOs.Select(dto => new Profile
                {
                    Name = dto.Name,
                    Status = dto.Status
                }).ToList();

                _context.Profiles.AddRange(profile);
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
                var profile = await _context.Profiles.ToListAsync();
                return Ok(profile);
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
                var profiles = await _context.Profiles.OrderByDescending(x => x.Id).ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(profiles);
                    var csvContent = writer.ToString();
                    var byteArray = Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "Profiles.csv");
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
                var profiles = await _context.Profiles.OrderByDescending(x => x.Id).ToListAsync();
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Count")]
        public async Task<IActionResult> GetProfileCount()
        {
            try
            {
                var totalProfiles = await _context.Profiles.CountAsync();
                var result = new { TotalProfiles = totalProfiles };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagedProfiles(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Profiles.OrderByDescending(x => x.Id).AsQueryable();

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
                    Profiles = pagedData
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
                var profile = await _context.Profiles.FindAsync(id);
                if (profile == null)
                {
                    return NotFound();
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDTO profileDTO)
        {
            if (profileDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profile = new Profile
                {
                    Name = profileDTO.Name,
                    Status = profileDTO.Status,
                    RegDate = DateTime.Now
                };

                await _context.Profiles.AddAsync(profile);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = profile.Id }, profile);
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
        public async Task<IActionResult> Update([FromBody] UpdateProfileDTO profileDTO)
        {
            if (profileDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profile = await _context.Profiles.FindAsync(profileDTO.Id);
                if (profile == null)
                {
                    return NotFound();
                }

                profile.Name = profileDTO.Name;
                profile.Status = profileDTO.Status;

                _context.Profiles.Update(profile);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(profileDTO.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusProfileDTO profileDTO)
        {
            if (profileDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profile = await _context.Profiles.FindAsync(profileDTO.Id);
                if (profile == null)
                {
                    return NotFound();
                }

                profile.Status = profileDTO.Status;
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var profile = await _context.Profiles.FindAsync(id);
                if (profile == null)
                {
                    return NotFound();
                }

                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting profile: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("Clear")]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var profiles = await _context.Profiles.ToListAsync();

                if (profiles.Any())
                {
                    _context.Profiles.RemoveRange(profiles);
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

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
