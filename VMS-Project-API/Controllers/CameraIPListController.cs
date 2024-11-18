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

namespace VMS_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraIPListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraIPListController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("Get")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.tbl_CameraIPList.ToListAsync();
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
                var totalCameraIP = await _context.tbl_CameraIPList.CountAsync();

                var result = new
                {
                    TotalCameraIPList = totalCameraIP,
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
                var query = _context.tbl_CameraIPList.AsQueryable();

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
        [HttpGet("GetByCameraIP")]
        public async Task<IActionResult> GetById(string cameraIP)
        {
            try
            {
                var cmrs = await _context.tbl_CameraIPList.FirstOrDefaultAsync(u => u.CameraIP == cameraIP);
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

        [HttpPost("Post")]
        public async Task<IActionResult> Create(CreateCameraListIPDTO cameraIPListDTO)
        {
            try
            {
                var cameralist = new CameraIPList
                {
                    CameraIP = cameraIPListDTO.CameraIP,
                    ObjectList = cameraIPListDTO.ObjectList
                };

                await _context.tbl_CameraIPList.AddAsync(cameralist);
                await _context.SaveChangesAsync();

                return Ok(cameralist);
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
                return _context.tbl_CameraIPList.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking role existence: {ex.Message}");
            }
        }

        [HttpPut("Put")]
        public async Task<IActionResult> UpdateCMRS([FromBody] CreateCameraListIPDTO updateDto)
        {
            try
            {
                if (updateDto.Id < 1)
                {
                    return BadRequest("Invalid ID.");
                }

                var existingActivityLog = await _context.tbl_CameraIPList.FindAsync(updateDto.Id);
                if (existingActivityLog == null)
                {
                    return NotFound("Activity Log not found.");
                }

                if (updateDto.CameraIP != null)
                    existingActivityLog.CameraIP = updateDto.CameraIP;

                if (updateDto.ObjectList != null)
                    existingActivityLog.ObjectList = updateDto.ObjectList;


                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CMRSExists(updateDto.Id))
                {
                    return NotFound("Activity Log not found.");
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


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCMRS(int id)
        {
            try
            {
                var cmrs = await _context.tbl_CameraIPList.FindAsync(id);
                if (cmrs == null)
                {
                    return NotFound();
                }

                _context.tbl_CameraIPList.Remove(cmrs);
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
