using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.EntityFrameworkCore;

namespace ActionTrakingSystem.Controllers
{

    public class MitigationActionFileUploaderController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MitigationActionFileUploaderController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context= context;
            _hostingEnvironment = hostingEnvironment;

        }
        [Authorize]
        [HttpPost("getDocumnetList")]
        public async Task<IActionResult> GetDocumnetList(OMA_DocumnetListDto reg)
        {
            try
            {
                var docList = await (from a in _context.OMA_Evidence.Where(a => a.siteId == reg.siteId && a.technologyId == reg.technologyId && a.actionId == reg.actionId && a.isDeleted == 0)
                                     select new
                                     {
                                         a.name,
                                         a.remarks,
                                         a.path,
                                         a.evidenceId,
                                     }).ToListAsync();
                return Ok(docList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("downloadFile/{url}")]
        public async Task<IActionResult> DownloadTilFile(string url)
        {
            try
            {


                if (url != null)
                {
                    string decodedUrl = HttpUtility.UrlDecode(url);
                    var tilUrl = decodedUrl.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + tilUrl;

                    // Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", decodedUrl);
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
        [HttpPost("saveDocumnet")]
        public async Task<IActionResult> SaveDocument([FromForm] OMA_UploadDocumentDto reg)
        {
            try
            {
                if (reg.file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.file.FileName);
                    string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "OMA_Uploads", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await reg.file.CopyToAsync(stream);
                    }
                    string url = $"~/OMA_Uploads/{fileName}";
                    OMA_Evidence fileIR = new OMA_Evidence();
                    fileIR.path = url;
                    fileIR.actionId = Convert.ToInt32(reg.actionId);
                    fileIR.siteId = Convert.ToInt32(reg.siteId);
                    fileIR.technologyId = Convert.ToInt32(reg.technologyId);
                    fileIR.name = reg.name;
                    fileIR.createdBy = Convert.ToInt32(reg.userId);
                    fileIR.createdOn = DateTime.Now;
                    fileIR.remarks = reg.remarks;
                    fileIR.isDeleted = 0;
                    _context.Add(fileIR);
                    _context.SaveChanges();
                    var obj = new
                    {
                        reg.name,
                        reg.remarks,
                        filePath,
                        evidenceId = fileIR.evidenceId,
                    };
                    return Ok(obj);
                }
                else
                {
                    return BadRequest("Error File Didn't Save");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteDocument")]
        public async Task<IActionResult> DeleteDocument(OMA_DeleteDocumentDto reg)
        {
            try
            {
                OMA_Evidence f = await (from a in _context.OMA_Evidence.Where(a => a.evidenceId == reg.evidenceId)
                                        select a).FirstOrDefaultAsync();
                if (f != null)
                {
                    
                    f.isDeleted = 1;
                    _context.SaveChanges();
                    return Ok();

                }
                else
                {
                    return BadRequest("Error File Didn't Save");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
