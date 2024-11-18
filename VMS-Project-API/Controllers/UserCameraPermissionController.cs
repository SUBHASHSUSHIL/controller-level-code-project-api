using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.Update;
using System.Text;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserCameraPermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserCameraPermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateUserCameraPermissionDTO> createUserCameras)
        {
            if (createUserCameras == null || !createUserCameras.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var user_Camera_Permission = createUserCameras.Select(dto => new User_Camera_Permission
                {
                    UserId = dto.UserId,
                    CameraId = dto.CameraId,
                    Status = dto.Status,
                    RegDate = dto.RegDate
                }).ToList();

                _context.user_Camera_Permissions.AddRange(user_Camera_Permission);
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
                var user_Camera_Permission = await _context.user_Camera_Permissions.ToListAsync();
                return Ok(user_Camera_Permission);
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
                var permissions = await _context.user_Camera_Permissions
                    .Include(x => x.Camera)
                    .Include(x => x.User)
                    .ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(permissions);
                    var csvContent = writer.ToString();
                    var byteArray = Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "UserCameraPermissions.csv");
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
                var ucp = await _context.user_Camera_Permissions.Include(x => x.Camera).Include(x => x.User).OrderByDescending(x => x.Id).ToListAsync();
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
                var totaluser_Camera_Permissions = await _context.user_Camera_Permissions.CountAsync();
                var activeuser_Camera_Permissions = await _context.user_Camera_Permissions.CountAsync(c => c.Status == true);
                var inactiveuser_Camera_Permissions = await _context.user_Camera_Permissions.CountAsync(c => c.Status == false);

                var result = new
                {
                    Totaluser_Camera_Permission = totaluser_Camera_Permissions,
                    Activeuser_Camera_Permission = activeuser_Camera_Permissions,
                    Inactiveuser_Camera_Permission = inactiveuser_Camera_Permissions
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
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                var totalItems = await _context.user_Camera_Permissions.Include(x => x.Camera).Include(x => x.User).OrderByDescending(x => x.Id).CountAsync();

                var permissions = await _context.user_Camera_Permissions.Include(x => x.Camera).Include(x => x.User).OrderByDescending(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    Items = permissions
                };

                return Ok(response);
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
                var permission = await _context.user_Camera_Permissions.Include(x => x.Camera).Include(x => x.User).OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (permission == null)
                {
                    return NotFound();
                }
                return Ok(permission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCameraPermissionDTO permissionDto)
        {
            try
            {
                if (permissionDto == null)
                {
                    return BadRequest("Invalid data.");
                }

                var permission = new User_Camera_Permission
                {
                    UserId = permissionDto.UserId,
                    CameraId = permissionDto.CameraId,
                    Status = permissionDto.Status,
                    RegDate = permissionDto.RegDate
                };

                await _context.user_Camera_Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving changes: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        private bool UserCameraPermissionExists(int id)
        {
            try
            {
                return _context.user_Camera_Permissions.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking user camera permission existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserCameraPermission([FromBody] User_Camera_Permission permission)
        {
            try
            {
                if (permission.Id < 1)
                {
                    return BadRequest("Invalid permission ID.");
                }

                if (!UserCameraPermissionExists(permission.Id))
                {
                    return NotFound();
                }

                _context.user_Camera_Permissions.Update(permission);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"An error occurred while updating user camera permission: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateUserCameraPermissionDTO userCameraPermissionDTO)
        {
            if (userCameraPermissionDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.user_Camera_Permissions.FindAsync(userCameraPermissionDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = userCameraPermissionDTO.Status;
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
        public async Task<IActionResult> DeleteUserCameraPermission(int id)
        {
            try
            {
                var permission = await _context.user_Camera_Permissions.FindAsync(id);
                if (permission == null)
                {
                    return NotFound();
                }

                _context.user_Camera_Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting user camera permission: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
