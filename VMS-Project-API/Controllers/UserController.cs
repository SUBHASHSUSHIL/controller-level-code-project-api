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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateUserDTO> userDTOs)
        {
            if (userDTOs == null || !userDTOs.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var users = userDTOs.Select(dto => new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    MobileNo = dto.MobileNo,
                    EmailId = dto.EmailId,
                    Username = dto.Username,
                    Password = dto.Password,
                    RoleId = dto.RoleId,
                    Image = dto.Image,
                    Status = dto.Status,
                    RegDate = dto.RegDate
                }).ToList();

                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing data: {ex.Message}");
            }
        }

        //[AllowAnonymous]
        [HttpGet("export-json")]
        public async Task<IActionResult> ExportJsonData()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
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
                var users = await _context.Users.Include(x => x.Role).ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(users);
                    var csvContent = writer.ToString();
                    var byteArray = Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "Users.csv");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.Users.Include(x => x.Role).OrderByDescending(x => x.Id).ToListAsync();
                return Ok(ucp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetCountStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(c => c.Status == true);
                var inactiveUsers = await _context.Users.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = inactiveUsers
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> Index([FromQuery] int pageNumbers = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Users.Include(x => x.Role).OrderByDescending(x => x.Id).CountAsync();
                var pageNumber = (int)Math.Ceiling(totalCount / (double)pageSize);

                var users = await _context.Users.Include(x => x.Role).OrderByDescending(x => x.Id)
                    .Skip((pageNumbers - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Users = users
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var urs = await _context.Users.Include(x => x.Role).OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (urs == null)
                {
                    return NotFound();
                }
                return Ok(urs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("User/Login")]
        public async Task<IActionResult> UserReturnData([FromBody] UserLoginDTO loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid login data.");
            }

            try
            {
                var user = await _context.Users
                    .Where(u => u.Username == loginDto.Username && u.Password == loginDto.Password)
                    .Include(u => u.Role) 
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO userDto)
        {
            try
            {
                var role = await _context.Roles.FindAsync(userDto.RoleId);
                if (role == null)
                {
                    return BadRequest("Invalid RoleId.");
                }

                var user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    MobileNo = userDto.MobileNo,
                    EmailId = userDto.EmailId,
                    Username = userDto.Username,
                    Password = userDto.Password,
                    RoleId = userDto.RoleId,
                    Role = role,
                    Status = userDto.Status,
                    RegDate = userDto.RegDate
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok(user);
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

        private bool GrpExists(int id)
        {
            try
            {
                return _context.Users.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking user existence: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO userDto)
        {
            try
            {
                if (userDto.Id < 1)
                {
                    return BadRequest();
                }

                var existingUser = await _context.Users.FindAsync(userDto.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.MobileNo = userDto.MobileNo;
                existingUser.EmailId = userDto.EmailId;
                existingUser.Username = userDto.Username;
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    existingUser.Password = userDto.Password;
                }
                existingUser.RoleId = userDto.RoleId;
                existingUser.Image = userDto.Image;
                existingUser.Status = userDto.Status;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!GrpExists(userDto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating user: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDTO updateStatusDTO)
        {
            if (updateStatusDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.Users.FindAsync(updateStatusDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = updateStatusDTO.Status;
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


        [HttpDelete("id")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var urs = await _context.Users.FindAsync(id);
                if (urs == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(urs);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}