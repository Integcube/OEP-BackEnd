using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectReadinessChecklistController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DAL _context;
        public ProjectReadinessChecklistController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        [HttpGet("getReadnessChartList")]
        public async Task<IActionResult> GetReadnessChartList(int UserId)
        {
            try
            {

                var Phase = await (from a in _context.ProjectReadiness.Where(a => a.IsDeleted == 0)
                                   join plnt in _context.Sites on a.SiteId equals plnt.siteId
                                   join user in _context.AppUser on a.CreatedBy equals user.userId
                                   join pgm in _context.AppUser on a.AssignTo equals pgm.userId
                                   join ausite in _context.AUSite.Where(aa => aa.userId == UserId) on a.SiteId equals ausite.siteId
                                   select new
                                   {
                                       a.PlanId,
                                       a.SiteId,
                                       a.CreatedOn,
                                       a.CreatedBy,
                                       site = plnt.siteName,
                                       plan = a.Title,
                                       user = user.userName,
                                       assignTo = pgm.userName,
                                       AssignPgm = a.AssignTo,
                                   }).ToListAsync();


                return Ok(Phase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("saveReadiness")]
        public async Task<IActionResult> SaveReadiness(ProjectReadiness reg)
        {
            try
            {
                if (reg.PlanId == -1)
                {
                    ProjectReadiness region = new ProjectReadiness();
                    region.IsDeleted = 0;
                    region.Title = reg.Title;
                    region.SiteId = reg.SiteId;
                    region.AssignTo = reg.AssignTo;
                    region.CreatedBy = reg.CreatedBy;
                    region.CreatedOn = DateTime.Now;

                    _context.Add(region);
                    _context.SaveChanges();
                }
                else
                {


                    ProjectReadiness region = await (from r in _context.ProjectReadiness.Where(a => a.PlanId == reg.PlanId)
                                                     select r).FirstOrDefaultAsync();
                    region.IsDeleted = 0;
                    region.Title = reg.Title;
                    region.SiteId = reg.SiteId;
                    region.AssignTo = reg.AssignTo;
                    region.ModifiedBy = reg.CreatedBy;
                    region.ModifiedOn = DateTime.Now;


                    _context.SaveChanges();

                }
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("deletePlan")]
        public async Task<IActionResult> DeletePlan(int planId)
        {
            try
            {
                ProjectReadiness plan = await (from r in _context.ProjectReadiness.Where(a => a.PlanId == planId)
                                               select r).FirstOrDefaultAsync();
                plan.IsDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getPlanTaskBy")]
        public async Task<IActionResult> GetPlanTaskBy(int planId)
        {
            try

            {
                var planInfo = await (from pl in _context.ProjectReadiness.Where(aa => aa.IsDeleted == 0 && aa.PlanId == planId)
                                      join plnt in _context.Sites on pl.SiteId equals plnt.siteId
                                      join cl in _context.Cluster on plnt.clusterId equals cl.clusterId
                                      join reg in _context.Regions2 on plnt.region2 equals reg.regionId
                                      select new
                                      {
                                          pl.PlanId,
                                          pl.SiteId,
                                          site = plnt.siteName,
                                          plan = pl.Title,
                                          region = reg.title,
                                          cluster = cl.clusterTitle
                                      }).FirstOrDefaultAsync();


                List<ProjectReadinessDTO> results = new List<ProjectReadinessDTO>();
                results = await _context.ProjectReadinessDTO.FromSqlRaw("EXEC dbo.[GetProjectReadinessPlans] @PlanId", new SqlParameter("@PlanId", planId)).ToListAsync();

                var responseData = new
                {
                    planInfo,
                    results
                };
                return Ok(responseData);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost("savePhaseTask")]
        public async Task<IActionResult> SavePhaseTask(ProjectReadinessphase reg)
        {
            try
            {
                if (reg.phaseId == -1)
                {
                    ProjectReadinessPhase task = new ProjectReadinessPhase();
                    task.IsDeleted = 0;
                    task.Title = reg.title;
                    task.planId = reg.planId;
                    task.Weightage = Convert.ToDecimal(reg.weightage);
                    task.CreatedBy = reg.createdBy;
                    task.CreatedOn = DateTime.Now;
                    task.displayOrder = reg.displayOrder;
                    _context.Add(task);
                    _context.SaveChanges();
                    //await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));
                }
                else
                {
                    ProjectReadinessPhase task = await (from r in _context.ProjectReadinessPhase.Where(a => a.PhaseId == reg.phaseId)
                                                        select r).FirstOrDefaultAsync();

                    task.IsDeleted = 0;
                    task.Title = reg.title;
                    task.planId = reg.planId;
                    task.Weightage = Convert.ToDecimal(reg.weightage);
                    task.CreatedBy = reg.createdBy;
                    task.CreatedOn = DateTime.Now;
                    task.displayOrder = reg.displayOrder;
                    _context.SaveChanges();
                    //await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));

                }


                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("deletephase")]
        public async Task<IActionResult> Deletephase(int phaseId, int userId)
        {
            try
            {
                ProjectReadinessPhase plan = await (from r in _context.ProjectReadinessPhase.Where(a => a.PhaseId == phaseId)
                                                    select r).FirstOrDefaultAsync();
                plan.IsDeleted = 1;
                plan.ModifiedBy = userId;
                plan.ModifiedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpDelete("deletetask")]
        public async Task<IActionResult> Deletetask(int taskId, int userId)
        {
            try
            {
                ProjectReadinessChecklist plan = await (from r in _context.ProjectReadinessChecklist.Where(a => a.TaskId == taskId)
                                                    select r).FirstOrDefaultAsync();
                plan.IsDeleted = 1;
                plan.ModifiedBy = userId;
                plan.ModifiedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("savemaintask")]
        public async Task<IActionResult> SaveMainTask(ProjectReadinessChecklist reg)
        {
            try
            {
                if (reg.TaskId == -1)
                {
                    ProjectReadinessChecklist task = new ProjectReadinessChecklist();
                    task.IsDeleted = 0;
                    task.Title = reg.Title;
                    task.PhaseId = reg.PhaseId;
                    task.ParentId = -1;
                    task.PlanId = reg.PlanId;

                    task.ActualScore = reg.ActualScore;
                    task.CreatedBy = reg.CreatedBy;
                    task.CreatedOn = DateTime.Now;
                    task.IdealScore = reg.IdealScore;
                    task.Remarks = reg.Remarks;
                    task.Responsibility = reg.Responsibility;
                    task.TargetDate = reg.TargetDate;
                    _context.Add(task);
                    _context.SaveChanges();
                }
                else
                {
                    ProjectReadinessChecklist task = await (from r in _context.ProjectReadinessChecklist.Where(a => a.TaskId == reg.TaskId)
                                                            select r).FirstOrDefaultAsync();
                    task.IsDeleted = 0;
                    task.Title = reg.Title;
                    task.PhaseId = reg.PhaseId;
                    task.ParentId = -1;
                    task.PlanId = reg.PlanId;
                    task.ActualScore = reg.ActualScore;
                    task.CreatedBy = reg.CreatedBy;
                    task.CreatedOn = DateTime.Now;
                    task.IdealScore = reg.IdealScore;
                    task.Remarks = reg.Remarks;
                    task.Responsibility = reg.Responsibility;
                    task.TargetDate = reg.TargetDate;
                    _context.SaveChanges();

                }
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Files
        [Authorize]
        [HttpGet("deleteDocument")]
        public async Task<IActionResult> DeleteDocument(int DocId)
        {
            try
            {


                var fileIR = await (from a in _context.ProjectReadinessDocument.Where(a => a.DocId == DocId)
                                    select a).FirstOrDefaultAsync();
                if (fileIR != null)
                {
                    fileIR.IsDeleted = 1;
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("uploadFile")]
        public async Task<IActionResult> uploadFiles([FromForm] UploadFileDtoReadiness reg)
        {
            try
            {
                if (reg.report != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.report.FileName);
                    string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "OT_Uploads", FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await reg.report.CopyToAsync(stream);
                    }
                    string FileUrl = $"OT_Uploads/{FileName}";

                    ProjectReadinessDocument ot = new ProjectReadinessDocument();
                    ot.Path = FileUrl;
                    ot.SiteId = Convert.ToInt32(reg.siteId);
                    ot.Date = DateTime.Parse(reg.date).ToLocalTime();
                    ot.FileName = reg.fileName;
                    ot.Remarks = reg.remarks;
                    ot.UserId = Convert.ToInt32(reg.userId);
                    ot.TypeId = Convert.ToInt32(reg.type);
                    ot.TaskId = Convert.ToInt32(reg.taskId);
                    
                    ot.IsDeleted = 0;
                    _context.Add(ot);
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception E)
            {
                return BadRequest(E.Message);
            }
        }

        [Authorize]
        [HttpPost("getFiles")]
        public async Task<IActionResult> GetFiles(DocInfoReadiness reg)
        {
            try
            {
                DateTime Date = DateTime.Parse(reg.Date);
                var files = await (from a in _context.ProjectReadinessDocument.Where(a => a.SiteId == reg.SiteId & a.TaskId==reg.taskId & a.IsDeleted == 0)
                                   select new
                                   {
                                       a.SiteId,
                                       a.FileName,
                                       a.Remarks,
                                       a.Path,
                                       a.Date,
                                       a.DocId,
                                       type = a.TypeId,
                                       a.TaskId
                                   }).ToListAsync();

                return Ok(files);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("downloadFile/{DocId}")]
        public async Task<IActionResult> DownloadTilFile(int DocId)
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var fileIR = await (from a in _context.ProjectReadinessDocument.Where(a => a.DocId == DocId)
                                    select a).FirstOrDefaultAsync();

                if (fileIR != null)
                {
                    var relativePath = fileIR.Path.Replace('/', Path.DirectorySeparatorChar);
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, relativePath);
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", fileIR.Path);
                }
                else
                {
                    return Ok(-1);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Authorize]
        [HttpGet("makecopyplan")]
        public async Task<IActionResult> MakecopyPlan(int planId)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.CopyProjectReadinessPlan @PlanId", new SqlParameter("@PlanId", planId));

                return Ok(1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
