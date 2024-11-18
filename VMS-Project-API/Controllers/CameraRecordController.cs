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
    public class CameraRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonData([FromBody] List<CameraRecordCreateDTO> cameraRecordCreates)
        {
            if (cameraRecordCreates == null || !cameraRecordCreates.Any())
            {
                return BadRequest("No data provided.");
            }

            try
            {
                var cameraRecord = cameraRecordCreates.Select(dto => new CameraRecord
                {
                    CameraId = dto.CameraId,
                    RecordPath = dto.RecordPath,
                    Status = dto.Status,
                    RegDate = dto.RegDate
                }).ToList();

                _context.CameraRecords.AddRange(cameraRecord);
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
                var cameraRecord = await _context.CameraRecords.ToListAsync();
                return Ok(cameraRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting data: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportRecords()
        {
            try
            {
                var records = await _context.CameraRecords.Include(i => i.Camera).ToListAsync();

                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Id,CameraId,RecordPath,Status,RegDate");

                foreach (var record in records)
                {
                    csvBuilder.AppendLine($"{record.Id},{record.CameraId},{record.RecordPath},{record.Status},{record.RegDate:yyyy-MM-ddTHH:mm:ss}");
                }

                var csvContent = csvBuilder.ToString();
                var fileBytes = Encoding.UTF8.GetBytes(csvContent);
                var fileStream = new MemoryStream(fileBytes);

                return File(fileStream, "text/csv", "camera_records.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting records: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ucp = await _context.CameraRecords.Include(i => i.Camera).OrderByDescending(x => x.Id).ToListAsync();
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
                var totalCameraRecords = await _context.CameraRecords.CountAsync();
                var activeCameraRecords = await _context.CameraRecords.CountAsync(c => c.Status == true);
                var inactiveCameraRecords = await _context.CameraRecords.CountAsync(c => c.Status == false);

                var result = new
                {
                    TotalCameraRecord = totalCameraRecords,
                    ActiveCameraRecords = activeCameraRecords,
                    InactiveCameraRecords = inactiveCameraRecords
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
        public async Task<IActionResult> Pagination(
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
                var query = _context.CameraRecords.Include(i => i.Camera).AsQueryable();

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
                    CameraAlertStatuses = pagedData
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
                var crd = await _context.CameraRecords.Include(i => i.Camera).OrderByDescending(x => x.Id).FirstOrDefaultAsync(u => u.Id == id);
                if (crd == null)
                {
                    return NotFound();
                }
                return Ok(crd);
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
                var cameraRecords = await _context.CameraRecords.Include(i => i.Camera).OrderByDescending(x => x.Id)
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                

                return Ok(cameraRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CameraRecordCreateDTO cameraRecordDTO)
        {
            if (cameraRecordDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraRecord = new CameraRecord
                {
                    CameraId = cameraRecordDTO.CameraId,
                    RecordPath = cameraRecordDTO.RecordPath,
                    Status = cameraRecordDTO.Status,
                    RegDate = cameraRecordDTO.RegDate
                };

                await _context.CameraRecords.AddAsync(cameraRecord);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = cameraRecord.Id }, cameraRecord);
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
        public async Task<IActionResult> Update([FromBody] CameraRecordUpdateDTO cameraRecordDTO)
        {
            if (cameraRecordDTO == null || cameraRecordDTO.Id < 1)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraRecord = await _context.CameraRecords.FindAsync(cameraRecordDTO.Id);

                if (cameraRecord == null)
                {
                    return NotFound();
                }

                cameraRecord.RecordPath = cameraRecordDTO.RecordPath;
                cameraRecord.Status = cameraRecordDTO.Status;
                cameraRecord.RegDate = cameraRecordDTO.RegDate;

                _context.CameraRecords.Update(cameraRecord);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"An error occurred while updating record: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusCameraRecordDTO cameraRecordDTO)
        {
            if (cameraRecordDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var user = await _context.CameraRecords.FindAsync(cameraRecordDTO.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = cameraRecordDTO.Status;
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
        public async Task<IActionResult> DeleteCRecord(int id)
        {
            try
            {
                var crd = await _context.CameraRecords.FindAsync(id);
                if (crd == null)
                {
                    return NotFound();
                }

                _context.CameraRecords.Remove(crd);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while deleting camera record: {ex.Message}");
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
                var records = await _context.CameraRecords.ToListAsync();

                if (records.Any())
                {
                    _context.CameraRecords.RemoveRange(records);
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
                var records = await _context.CameraRecords
                    .Where(cr => cr.CameraId == cameraId)
                    .ToListAsync();

                if (records.Any())
                {
                    _context.CameraRecords.RemoveRange(records);
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
