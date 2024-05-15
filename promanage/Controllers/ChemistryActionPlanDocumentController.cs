using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System;
using ActionTrakingSystem.DTOs;
using System.Linq;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace ActionTrakingSystem.Controllers
{

    public class ChemistryActionPlanDocumentController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ChemistryActionPlanDocumentController(DAL context, IWebHostEnvironment hostingEnvironment) 
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("deleteDoc")]
        public async Task<IActionResult> DeleteDoc([FromForm] uploadDocDto reg)
        {
            try
            {
                CAP_Documents doc = await (from a in _context.CAP_Documents.Where(a => a.docId == Convert.ToInt32(reg.docId))
                                           select a).FirstOrDefaultAsync();
                doc.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(doc);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("pdfReport/{docId}")]
        public async Task<IActionResult> DownloadInsuranceFile(int docId)
        {
            try
            {
                CAP_Documents doc = await (from a in _context.CAP_Documents.Where(a => a.docId == Convert.ToInt32(docId))
                                           select a).FirstOrDefaultAsync();

                if (doc != null)
                {
                    var insuranceURL = doc.filePath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + insuranceURL;
                    // Get the file path from the URL
                    // Get the file extension
                    string fileExtension = Path.GetExtension(filePath);
                    // Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", doc.filePath + fileExtension);

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
        [HttpPost("uploadFile")]
        public async Task<IActionResult> uploadFiles([FromForm] uploadDocDto reg)
        {
            try
            {
                if(Convert.ToInt32(reg.docId) == -1)
                {
                    CAP_Documents doc = new CAP_Documents();
                    doc.siteId = Convert.ToInt32(reg.siteId);
                    if (reg.file != null)
                    {
                        string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.file.FileName);
                        string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "CAP_Uploads", tilFileName);
                        using (var stream = new FileStream(tilfilePath, FileMode.Create))
                        {
                            await reg.file.CopyToAsync(stream);
                        }
                        string tilFileUrl = $"~/CAP_Uploads/{tilFileName}";
                        doc.filePath = tilFileUrl;
                        doc.fileName = reg.file.FileName;
                    }
                    doc.createdOn = DateTime.Now;
                    doc.createdBy = Convert.ToInt32(reg.userId);
                    _context.Add(doc);
                    _context.SaveChanges();
                }
                else
                {
                    CAP_Documents doc = await (from a in _context.CAP_Documents.Where(a => a.docId == Convert.ToInt32(reg.docId))
                                                  select a).FirstOrDefaultAsync();
                    doc.siteId = Convert.ToInt32(reg.siteId);
                    if (reg.file != null)
                    {
                        string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.file.FileName);
                        string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "CAP_Uploads", tilFileName);
                        using (var stream = new FileStream(tilfilePath, FileMode.Create))
                        {
                            await reg.file.CopyToAsync(stream);
                        }
                        string tilFileUrl = $"~/CAP_Uploads/{tilFileName}";
                        doc.filePath = tilFileUrl;
                        doc.fileName = reg.file.FileName;
                    }
                    doc.modifiedOn = DateTime.Now;
                    doc.modifiedBy = Convert.ToInt32(reg.userId);
                    _context.SaveChanges();
                }


                return Ok();
            }
            catch (Exception E)
            {
                return BadRequest(E.Message);
            }
        }
        [HttpPost("getDocuments")]
        public async Task<IActionResult> GetDocuments(UserIdDto reg)
        {
            try
            {
                var documents = await (from a in _context.CAP_Documents.Where(a => a.isDeleted == 0)
                                       join s in _context.Sites on a.siteId equals s.siteId
                                       join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                       join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                       join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                       join r in _context.Regions.Where(a => a.isDeleted == 0) on s.regionId equals
                                       r.regionId
                                       select new
                                       {
                                           a.docId,
                                           docName = a.fileName,
                                           s.siteId,
                                           siteTitle = s.siteName,
                                           regionTitle = r.title,
                                           r.regionId
                                       }).Distinct().ToListAsync();
                


               
                return Ok(documents);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
