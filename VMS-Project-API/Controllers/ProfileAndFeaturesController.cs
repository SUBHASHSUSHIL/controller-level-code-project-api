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
    public class ProfileAndFeaturesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileAndFeaturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateProfileAndFeaturesDTO> createProfileAnds)
        {
            if (createProfileAnds == null || !createProfileAnds.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var profileAndFeature = createProfileAnds.Select(dto => new ProfileAndFeatures
                {
                    ProfileId = dto.ProfileId,
                    FeatureId = dto.FeatureId,
                    Status = dto.Status
                }).ToList();

                _context.ProfileAndFeaturess.AddRange(profileAndFeature);
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
                var profileAndFeatures = await _context.ProfileAndFeaturess.ToListAsync();
                return Ok(profileAndFeatures);
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
                var profileAndFeaturesList = await _context.ProfileAndFeaturess
                    .Include(paf => paf.Profile)
                    .Include(paf => paf.Features)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(profileAndFeaturesList);
                    var csvContent = writer.ToString();
                    var byteArray = Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "ProfileAndFeatures.csv");
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
                var profileAndFeatures = await _context.ProfileAndFeaturess
                    .Include(paf => paf.Profile)
                    .Include(paf => paf.Features)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
                return Ok(profileAndFeatures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Count")]
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var totalProfileAndFeatures = await _context.ProfileAndFeaturess.CountAsync();
                var result = new
                {
                    TotalProfileAndFeatures = totalProfileAndFeatures,
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
                var query = _context.ProfileAndFeaturess
                    .Include(paf => paf.Profile)
                    .Include(paf => paf.Features)
                    .OrderByDescending(x => x.Id)
                    .AsQueryable();

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
                    ProfileAndFeaturesData = pagedData
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
                var profileAndFeatures = await _context.ProfileAndFeaturess
                    .Include(paf => paf.Profile)
                    .Include(paf => paf.Features)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (profileAndFeatures == null)
                {
                    return NotFound();
                }
                return Ok(profileAndFeatures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileAndFeaturesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profileAndFeatures = new ProfileAndFeatures
                {
                    ProfileId = dto.ProfileId,
                    FeatureId = dto.FeatureId,
                    Status = dto.Status,
                    RegDate = DateTime.Now
                };

                await _context.ProfileAndFeaturess.AddAsync(profileAndFeatures);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = profileAndFeatures.Id }, profileAndFeatures);
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
        public async Task<IActionResult> Update([FromBody] UpdateProfileAndFeaturesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profileAndFeatures = await _context.ProfileAndFeaturess.FindAsync(dto.Id);
                if (profileAndFeatures == null)
                {
                    return NotFound();
                }

                profileAndFeatures.ProfileId = dto.ProfileId;
                profileAndFeatures.FeatureId = dto.FeatureId;
                profileAndFeatures.Status = dto.Status;

                _context.ProfileAndFeaturess.Update(profileAndFeatures);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProfileAndFeaturess.Any(e => e.Id == dto.Id))
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
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateProfileAndFeaturesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var profileAndFeatures = await _context.ProfileAndFeaturess.FindAsync(dto.Id);
                if (profileAndFeatures == null)
                {
                    return NotFound();
                }

                profileAndFeatures.Status = dto.Status;
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
                var profileAndFeatures = await _context.ProfileAndFeaturess.FindAsync(id);
                if (profileAndFeatures == null)
                {
                    return NotFound();
                }

                _context.ProfileAndFeaturess.Remove(profileAndFeatures);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting profile and feature: {ex.Message}");
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
                var profileAndFeatures = await _context.ProfileAndFeaturess.ToListAsync();

                if (profileAndFeatures.Any())
                {
                    _context.ProfileAndFeaturess.RemoveRange(profileAndFeatures);
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
