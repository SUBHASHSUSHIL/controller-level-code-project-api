using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO.Create;

namespace VMS_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ANPRStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ANPRStatusController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.tbl_ANPRStatus.OrderByDescending(x => x.Id).ToListAsync();
                return Ok(ucp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateANPRStatus aNPR)
        {
            try
            {
                if (aNPR == null)
                {
                    return BadRequest("Invalid data.");
                }

                var cmr = new ANPRStatus
                {
                    CameraId = aNPR.CameraId,
                    CameraName = aNPR.CameraName,
                    URL = aNPR.URL,
                    Status = aNPR.Status
                };

                await _context.tbl_ANPRStatus.AddAsync(cmr);
                await _context.SaveChangesAsync();
                return Ok(cmr);
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

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            try
            {
                var cma = await _context.tbl_ANPRStatus.FirstOrDefaultAsync(x => x.Id == id);
                if (cma == null)
                {
                    return NotFound();
                }
                _context.tbl_ANPRStatus.Remove(cma);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting camera: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
