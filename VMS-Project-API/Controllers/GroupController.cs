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
using System.Text.Json;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJson([FromBody] List<GroupCreateDTO> groupDTOs)
        {
            if (groupDTOs == null || groupDTOs.Count == 0)
            {
                return BadRequest("Invalid or empty JSON data.");
            }

            try
            {
                var groups = groupDTOs.Select(dto => new Group
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Status = dto.Status
                }).ToList();

                await _context.Groups.AddRangeAsync(groups);
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
        public async Task<IActionResult> ExportJson()
        {
            try
            {
                var groups = await _context.Groups.OrderByDescending(x => x.Id).ToListAsync();

                var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, groups, new JsonSerializerOptions { WriteIndented = true });
                stream.Position = 0;

                return File(stream, "application/json", "Groups.json");
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
                var group = await _context.Groups.ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(group);
                    var csvContent = writer.ToString();
                    var byteArray = System.Text.Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "Group.csv");
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
                var ucp = await _context.Groups.OrderByDescending(x => x.Id).ToListAsync();
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
                var totalGroup = await _context.Groups.CountAsync();
                var activeGroup = await _context.Groups.CountAsync(c => c.Status == true);
                var inactiveGroup = await _context.Groups.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalGroup = totalGroup,
                    ActiveGroup = activeGroup,
                    InactiveGroup = inactiveGroup
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
                var query = _context.Groups.OrderByDescending(x => x.Id).AsQueryable();

                var pagedGroups = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Groups = pagedGroups
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
                var grp = await _context.Groups.OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (grp == null)
                {
                    return NotFound();
                }
                return Ok(grp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupCreateDTO groupCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var group = new Group
                {
                    Name = groupCreateDTO.Name,
                    Description = groupCreateDTO.Description,
                    Status = groupCreateDTO.Status
                };

                await _context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
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


        private bool groupExist(int id)
        {
            try
            {
                return _context.Groups.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking group existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupUpdateDTO groupUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var group = await _context.Groups.FindAsync(groupUpdateDTO.Id);
                if (group == null)
                {
                    return NotFound();
                }

                group.Name = groupUpdateDTO.Name;
                group.Description = groupUpdateDTO.Description;
                group.Status = groupUpdateDTO.Status;

                _context.Groups.Update(group);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!groupExist(groupUpdateDTO.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating group: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStats([FromBody] StatusGroupDTO groupDTO)
        {
            if (groupDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.Groups.FindAsync(groupDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = groupDTO.Status;
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var grp = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
                if (grp == null)
                {
                    return NotFound();
                }

                _context.Groups.Remove(grp);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting group: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
