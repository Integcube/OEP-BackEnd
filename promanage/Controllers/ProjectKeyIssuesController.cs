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
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectKeyIssuesController : ControllerBase
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ProjectKeyIssuesController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        [HttpGet("getKeyIssues")]
        public async Task<IActionResult> getKeyIssues(DateTime date, int siteId)
        {
            try
            {
                var data = await (from a in _context.ProjectKeyIssues.Where(a => a.siteId == siteId && a.month.Month == date.Month && a.month.Year == date.Year)
                                  select new
                                  {
                                      a.id,
                                      a.siteId,
                                      a.hsseIssues,
                                      a.recruitmentIssues,
                                      a.technicalIssues,
                                      financialcommercial = a.financialCommercial,
                                      a.month,
                                      a.roschecklistprogress,
                                      a.technicalriskregister,
                                      a.lessonslearned,
                                      a.previousactioncall,
                                      a.receiveddocument,
                                      a.revieweddocument
                               }).FirstOrDefaultAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("saveKeyIssues")]
        public async Task<IActionResult> SaveKeyIssues(ProjectKeyIssuesDTO reg)
        {
            try
            {
                var data = await _context.ProjectKeyIssues
                    .Where(a => a.siteId == reg.siteId && a.month.Month == reg.date.Month && a.month.Year == reg.date.Year)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    ProjectKeyIssues task = new ProjectKeyIssues
                    {
                        siteId = reg.siteId,
                        month = reg.date,
                        hsseIssues = reg.hsseIssues,
                        technicalIssues = reg.technicalIssues,
                        recruitmentIssues = reg.recruitmentIssues,
                        financialCommercial = reg.financialCommercial,

                        roschecklistprogress = reg.roschecklistprogress,
                        technicalriskregister = reg.technicalriskregister,
                        lessonslearned = reg.lessonslearned,
                        previousactioncall = reg.previousactioncall,
                        receiveddocument=reg.receiveddocument,
                        revieweddocument=reg.revieweddocument,
                        createdBy = reg.userId,
                        createdOn = DateTime.Now,
                     
                    };
                    _context.ProjectKeyIssues.Add(task);
                }
                else
                {
                    data.hsseIssues = reg.hsseIssues;
                    data.technicalIssues = reg.technicalIssues;
                    data.recruitmentIssues = reg.recruitmentIssues;
                    data.financialCommercial = reg.financialCommercial;

                    data.roschecklistprogress = reg.roschecklistprogress;
                    data.technicalriskregister = reg.technicalriskregister;
                    data.lessonslearned = reg.lessonslearned;
                    data.previousactioncall = reg.previousactioncall;
                    data.receiveddocument = reg.receiveddocument;
                    data.revieweddocument = reg.revieweddocument;
                    data.modifiedBy = reg.userId;
                    data.modifiedOn = DateTime.Now;
                }

                await _context.SaveChangesAsync();
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
               

                var fileIR = await (from a in _context.ProjectKeyIssuesDocument.Where(a =>a.DocId== DocId)
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
        public async Task<IActionResult> uploadFiles([FromForm] UploadFileDto reg)
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
                    
                    ProjectKeyIssuesDocument ot = new ProjectKeyIssuesDocument();
                    ot.Path = FileUrl;
                    ot.SiteId = Convert.ToInt32(reg.siteId);
                    ot.Date = DateTime.Parse(reg.date).ToLocalTime();
                    ot.FileName = reg.fileName;
                    ot.Remarks = reg.remarks;
                    ot.UserId = Convert.ToInt32(reg.userId);
                    ot.TypeId= Convert.ToInt32(reg.type);
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
        public async Task<IActionResult> GetFiles(DocInfo reg)
        {
            try
            {
                DateTime Date = DateTime.Parse(reg.Date);
                var files = await (from a in _context.ProjectKeyIssuesDocument.Where(a => a.SiteId == reg.SiteId & a.Date.Month == Date.Month & a.Date.Year == Date.Year & a.IsDeleted==0 & a.TypeId== reg.type)
                                   select new
                                   {
                                       a.SiteId,
                                       a.FileName,
                                       a.Remarks,
                                       a.Path,
                                       a.Date,
                                       a.DocId,
                                        type= a.TypeId
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
                var fileIR = await (from a in _context.ProjectKeyIssuesDocument.Where(a => a.DocId == DocId)
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
        [HttpGet("getsites")]
        public async Task<IActionResult> Getsites(int UserId)
        {
            try
            {
                var Phase = await (from a in _context.AUSite.Where(a => a.userId == UserId)
                                   join aus in _context.Sites.Where(a => a.isDeleted == 0) on a.siteId equals aus.siteId
                                   join st in _context.ProjectStartPlan.Where(a => a.isDeleted == 0) on aus.siteId equals st.siteId
                                   
                                   select new
                                   {

                                       a.siteId,
                                       siteTitle = aus.siteName
                                   }).Distinct().OrderBy(a => a.siteTitle).ToListAsync();


                return Ok(Phase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
