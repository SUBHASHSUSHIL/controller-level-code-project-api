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

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CameraTrackingDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraTrackingDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CreateCameraTrackingDataDTO> createCameraTrackings)
        {
            if (createCameraTrackings == null || !createCameraTrackings.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var cameraTracking = createCameraTrackings.Select(dto => new CameraTrackingData
                {
                    CameraId = dto.CameraId,
                    VichelImage = dto.VichelImage,
                    NoPlateImage = dto.NoPlateImage,
                    VichelNo = dto.VichelNo,
                    Status = dto.Status,
                    RegDate = dto.RegDate
                }).ToList();

                _context.CameraTrackingDatas.AddRange(cameraTracking);
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
                var cameraTrackings = await _context.CameraTrackingDatas.ToListAsync();
                return Ok(cameraTrackings);
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
                var records = await _context.CameraTrackingDatas.ToListAsync();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                };

                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(records);
                    var csvContent = writer.ToString();
                    var byteArray = System.Text.Encoding.UTF8.GetBytes(csvContent);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", "CameraTrackingData.csv");
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
                var ucp = await _context.CameraTrackingDatas.Include(i => i.Camera).OrderByDescending(x => x.Id).ToListAsync();
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
                var totalCameraTrackingDatas = await _context.CameraTrackingDatas.CountAsync();
                var activeCameraTrackingDatas = await _context.CameraTrackingDatas.CountAsync(c => c.Status == true);
                var inactiveCameraTrackingDatas = await _context.CameraTrackingDatas.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalCameraTrackingData = totalCameraTrackingDatas,
                    ActiveCameraTrackingDatas = activeCameraTrackingDatas,
                    InactiveCameraTrackingData = inactiveCameraTrackingDatas
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
        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            int? cameraId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? orderBy = null,
            string orderType = "asc")
        {
            try
            {
                var query = _context.CameraTrackingDatas.Include(i => i.Camera).AsQueryable();

                
                if (cameraId.HasValue)
                {
                    query = query.Where(x => x.CameraId == cameraId.Value);
                }

                
                if (dateFrom.HasValue)
                {
                    query = query.Where(x => x.RegDate >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    query = query.Where(x => x.RegDate <= dateTo.Value);
                }

                
                if (!string.IsNullOrEmpty(orderBy))
                {
                    bool isDescending = orderType.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    switch (orderBy.ToLower())
                    {
                        case "cameraid":
                            query = isDescending ? query.OrderByDescending(x => x.CameraId) : query.OrderBy(x => x.CameraId);
                            break;
                        case "regdate":
                            query = isDescending ? query.OrderByDescending(x => x.RegDate) : query.OrderBy(x => x.RegDate);
                            break;
                        default:
                            query = isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                            break;
                    }
                }
                else
                {
                    query = orderType.Equals("desc", StringComparison.OrdinalIgnoreCase)
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                }

                
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
                    CameraTrackingData = pagedData
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
                var ctd = await _context.CameraTrackingDatas.Include(i => i.Camera).OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (ctd == null)
                {
                    return NotFound();
                }
                return Ok(ctd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("Camera-List/{cameraId}")]
        public async Task<IActionResult> CameraList(int cameraId)
        {
            try
            {
                var cameraTrackingDatas = await _context.CameraTrackingDatas.Include(i => i.Camera).OrderByDescending(x => x.Id)
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (cameraTrackingDatas == null || !cameraTrackingDatas.Any())
                {
                    return NotFound($"No records found for Camera ID: {cameraId}");
                }

                return Ok(cameraTrackingDatas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCameraTrackingDataDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cameraTrackingData = new CameraTrackingData
                {
                    CameraId = dto.CameraId,
                    VichelImage = dto.VichelImage,
                    NoPlateImage = dto.NoPlateImage,
                    VichelNo = dto.VichelNo,
                    Status = dto.Status,
                    RegDate = dto.RegDate
                };

                await _context.CameraTrackingDatas.AddAsync(cameraTrackingData);
                await _context.SaveChangesAsync();
                return Ok(cameraTrackingData);
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

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCameraTrackingDataDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cameraTrackingData = await _context.CameraTrackingDatas.FindAsync(dto.Id);
                if (cameraTrackingData == null)
                {
                    return NotFound();
                }

                cameraTrackingData.CameraId = dto.CameraId;
                cameraTrackingData.VichelImage = dto.VichelImage;
                cameraTrackingData.NoPlateImage = dto.NoPlateImage;
                cameraTrackingData.VichelNo = dto.VichelNo;
                cameraTrackingData.Status = dto.Status;
                cameraTrackingData.RegDate = dto.RegDate;

                _context.CameraTrackingDatas.Update(cameraTrackingData);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.CameraTrackingDatas.Any(e => e.Id == dto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating camera tracking data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusCameraTrackingDataDTO cameraTrackingData)
        {
            if (cameraTrackingData == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.CameraTrackingDatas.FindAsync(cameraTrackingData.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = cameraTrackingData.Status;
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
        public async Task<IActionResult> DeleteCTD(int id)
        {
            try
            {
                var ctd = await _context.CameraTrackingDatas.FindAsync(id);
                if (ctd == null)
                {
                    return NotFound();
                }

                _context.CameraTrackingDatas.Remove(ctd);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting camera tracking data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearRecords()
        {
            try
            {
                var records = await _context.CameraActivities.ToListAsync();

                if (records.Any())
                {
                    _context.CameraActivities.RemoveRange(records);
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

        [HttpDelete("Clear/{cameraId}")]
        public async Task<IActionResult> ClearRecords(int cameraId)
        {
            try
            {
                var records = await _context.CameraTrackingDatas
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (records.Any())
                {
                    _context.CameraTrackingDatas.RemoveRange(records);
                    await _context.SaveChangesAsync();
                    return Ok($"All records for Camera ID: {cameraId} have been cleared.");
                }
                else
                {
                    return NotFound($"No records found for Camera ID: {cameraId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
