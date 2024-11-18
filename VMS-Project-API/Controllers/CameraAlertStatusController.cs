using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using VMS_Project_API.Data;
using VMS_Project_API.Entities;
using VMS_Project_API.EntitiesDTO;
using VMS_Project_API.EntitiesDTO.Create;
using VMS_Project_API.EntitiesDTO.ImportDTO;
using VMS_Project_API.EntitiesDTO.Update;

namespace VMS_Project_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CameraAlertStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraAlertStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJson([FromBody] List<ImportCameraAlertStatusDTO> cameraStatusDTOs)
        {
            if (cameraStatusDTOs == null || !cameraStatusDTOs.Any())
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                foreach (var cameraStatusDTO in cameraStatusDTOs)
                {
                    var cameraStatus = new CameraAlertStatus
                    {
                        CameraId = cameraStatusDTO.CameraId,
                        Recording = cameraStatusDTO.Recording,
                        ANPR = cameraStatusDTO.ANPR,
                        Snapshot = cameraStatusDTO.Snapshot,
                        PersonDetection = cameraStatusDTO.PersonDetection,
                        FireDetection = cameraStatusDTO.FireDetection,
                        AnimalDetection = cameraStatusDTO.AnimalDetection,
                        BikeDetection = cameraStatusDTO.BikeDetection,
                        MaskDetection = cameraStatusDTO.MaskDetection,
                        UmbrelaDetection = cameraStatusDTO.UmbrelaDetection,
                        BrifecaseDetection = cameraStatusDTO.BrifecaseDetection,
                        GarbageDetection = cameraStatusDTO.GarbageDetection,
                        WeaponDetection = cameraStatusDTO.WeaponDetection,
                        WrongDetection = cameraStatusDTO.WrongDetection,
                        QueueDetection = cameraStatusDTO.QueueDetection,
                        SmokeDetection = cameraStatusDTO.SmokeDetection
                    };

                    var existingStatus = await _context.CameraAlertStatuss.FindAsync(cameraStatus.Id);
                    if (existingStatus == null)
                    {
                        await _context.CameraAlertStatuss.AddAsync(cameraStatus);
                    }
                    else
                    {
                        existingStatus.CameraId = cameraStatus.CameraId;
                        existingStatus.Recording = cameraStatus.Recording;
                        existingStatus.ANPR = cameraStatus.ANPR;
                        existingStatus.Snapshot = cameraStatus.Snapshot;
                        existingStatus.PersonDetection = cameraStatus.PersonDetection;
                        existingStatus.FireDetection = cameraStatus.FireDetection;
                        existingStatus.AnimalDetection = cameraStatus.AnimalDetection;
                        existingStatus.BikeDetection = cameraStatus.BikeDetection;
                        existingStatus.MaskDetection = cameraStatus.MaskDetection;
                        existingStatus.UmbrelaDetection = cameraStatus.UmbrelaDetection;
                        existingStatus.BrifecaseDetection = cameraStatus.BrifecaseDetection;
                        existingStatus.GarbageDetection = cameraStatus.GarbageDetection;
                        existingStatus.WeaponDetection = cameraStatus.WeaponDetection;
                        existingStatus.WrongDetection = cameraStatus.WrongDetection;
                        existingStatus.QueueDetection = cameraStatus.QueueDetection;
                        existingStatus.SmokeDetection = cameraStatus.SmokeDetection;

                        _context.CameraAlertStatuss.Update(existingStatus);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("export-json")]
        public async Task<IActionResult> ExportJson()
        {
            try
            {
                var cameraAlertStatuses = await _context.CameraAlertStatuss.Include(i => i.Camera).ToListAsync();

                var exportList = cameraAlertStatuses.Select(status => new ExportCameraAlertStatusDTO
                {
                    Id = status.Id,
                    CameraId = status.CameraId,
                    Recording = status.Recording,
                    ANPR = status.ANPR,
                    Snapshot = status.Snapshot,
                    PersonDetection = status.PersonDetection,
                    FireDetection = status.FireDetection,
                    AnimalDetection = status.AnimalDetection,
                    BikeDetection = status.BikeDetection,
                    MaskDetection = status.MaskDetection,
                    UmbrelaDetection = status.UmbrelaDetection,
                    BrifecaseDetection = status.BrifecaseDetection,
                    GarbageDetection = status.GarbageDetection,
                    WeaponDetection = status.WeaponDetection,
                    WrongDetection = status.WrongDetection,
                    QueueDetection = status.QueueDetection,
                    SmokeDetection = status.SmokeDetection
                }).ToList();

                return Ok(exportList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("export-excel")]
        public async Task<IActionResult> Export()
        {
            try
            {
                var cameraAlertStatuses = await _context.CameraAlertStatuss.Include(i => i.Camera).ToListAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Id,CameraId,Recording,ANPR,Snapshot,PersonDetection,FireDetection,AnimalDetection,BikeDetection,MaskDetection,UmbrelaDetection,BrifecaseDetection,GarbageDetection,WeaponDetection,WrongDetection,QueueDetection,SmokeDetection");

                foreach (var status in cameraAlertStatuses)
                {
                    var line = $"{status.Id},{status.CameraId},{status.Recording},{status.ANPR},{status.Snapshot},{status.PersonDetection},{status.FireDetection},{status.AnimalDetection},{status.BikeDetection},{status.MaskDetection},{status.UmbrelaDetection},{status.BrifecaseDetection},{status.GarbageDetection},{status.WeaponDetection},{status.WrongDetection},{status.QueueDetection},{status.SmokeDetection}";
                    csv.AppendLine(line);
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                var output = new MemoryStream(bytes);

                return File(output, "text/csv", "CameraAlertStatus.csv");
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
                var ucp = await _context.CameraAlertStatuss.Include(i => i.Camera).OrderByDescending(c => c.Camera).ToListAsync();
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
                var totalCameraAlertStatus = await _context.CameraAlertStatuss.CountAsync();

                var result = new
                {
                    TotalCameraAlert = totalCameraAlertStatus,
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
                var query = _context.CameraAlertStatuss.Include(i => i.Camera).AsQueryable();

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
                var cmrs = await _context.CameraAlertStatuss.Include(i => i.Camera).FirstOrDefaultAsync(u => u.Id == id);
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

        [AllowAnonymous]
        [HttpGet("CameraAlert/{cameraId}")]
        public async Task<IActionResult> CameraList(int cameraId)
        {
            try
            {
                var cameraAlert = await _context.CameraAlertStatuss
                    .Where(cr => cr.CameraId == cameraId).FirstOrDefaultAsync();
                    

              

                return Ok(cameraAlert);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCameraAlertStatusDTO cameraStatusDTO)
        {
            if (cameraStatusDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraStatus = new CameraAlertStatus
                {
                    CameraId = cameraStatusDTO.CameraId,
                    Recording = cameraStatusDTO.Recording,
                    ANPR = cameraStatusDTO.ANPR,
                    Snapshot = cameraStatusDTO.Snapshot,
                    PersonDetection = cameraStatusDTO.PersonDetection,
                    FireDetection = cameraStatusDTO.FireDetection,
                    AnimalDetection = cameraStatusDTO.AnimalDetection,
                    BikeDetection = cameraStatusDTO.BikeDetection,
                    MaskDetection = cameraStatusDTO.MaskDetection,
                    UmbrelaDetection = cameraStatusDTO.UmbrelaDetection,
                    BrifecaseDetection = cameraStatusDTO.BrifecaseDetection,
                    GarbageDetection = cameraStatusDTO.GarbageDetection,
                    WeaponDetection = cameraStatusDTO.WeaponDetection,
                    WrongDetection = cameraStatusDTO.WrongDetection,
                    QueueDetection = cameraStatusDTO.QueueDetection,
                    SmokeDetection = cameraStatusDTO.SmokeDetection
                };

                await _context.CameraAlertStatuss.AddAsync(cameraStatus);
                await _context.SaveChangesAsync();

                return Ok(cameraStatus);
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
        public async Task<IActionResult> Update([FromBody] UpdateCameraAlertStatusDTO cameraStatusDTO)
        {
            if (cameraStatusDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var cameraStatus = await _context.CameraAlertStatuss.FindAsync(cameraStatusDTO.Id);
                if (cameraStatus == null)
                {
                    return NotFound();
                }

                cameraStatus.CameraId = cameraStatusDTO.CameraId;
                cameraStatus.Recording = cameraStatusDTO.Recording;
                cameraStatus.ANPR = cameraStatusDTO.ANPR;
                cameraStatus.Snapshot = cameraStatusDTO.Snapshot;
                cameraStatus.PersonDetection = cameraStatusDTO.PersonDetection;
                cameraStatus.FireDetection = cameraStatusDTO.FireDetection;
                cameraStatus.AnimalDetection = cameraStatusDTO.AnimalDetection;
                cameraStatus.BikeDetection = cameraStatusDTO.BikeDetection;
                cameraStatus.MaskDetection = cameraStatusDTO.MaskDetection;
                cameraStatus.UmbrelaDetection = cameraStatusDTO.UmbrelaDetection;
                cameraStatus.BrifecaseDetection = cameraStatusDTO.BrifecaseDetection;
                cameraStatus.GarbageDetection = cameraStatusDTO.GarbageDetection;
                cameraStatus.WeaponDetection = cameraStatusDTO.WeaponDetection;
                cameraStatus.WrongDetection = cameraStatusDTO.WrongDetection;
                cameraStatus.QueueDetection = cameraStatusDTO.QueueDetection;
                cameraStatus.SmokeDetection = cameraStatusDTO.SmokeDetection;

                _context.CameraAlertStatuss.Update(cameraStatus);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.CameraAlertStatuss.Any(e => e.Id == cameraStatusDTO.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"An error occurred while updating camera alert status: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCMRS(int id)
        {
            try
            {
                var cmrs = await _context.CameraAlertStatuss.FindAsync(id);
                if (cmrs == null)
                {
                    return NotFound();
                }

                _context.CameraAlertStatuss.Remove(cmrs);
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
