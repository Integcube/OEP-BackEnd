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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;

namespace ActionTrakingSystem.Controllers
{

    public class UploadDocumentController : BaseAPIController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly DAL _context;
        public UploadDocumentController(DAL context, IWebHostEnvironment hostingEnvironment) 
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

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
        public async Task<IActionResult> SaveDocument([FromForm] uploadDocumentDto reg)
        {
            try
            {
                if (reg.file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.file.FileName);
                    string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await reg.file.CopyToAsync(stream);
                    }
                    string url = $"~/uploads/{fileName}";
                    IATrackingFile fileIR = new IATrackingFile();
                    fileIR.path = url;
                    fileIR.iatId = Convert.ToInt32(reg.insurenceActionTrackerId);
                    fileIR.fileName = reg.name;
                    fileIR.createdBy = Convert.ToInt32(reg.userId);
                    fileIR.remarks = reg.remarks;
                    fileIR.isDeleted = 0;
                    _context.Add(fileIR);
                    _context.SaveChanges();
                    var obj = new
                    {
                        reg.name,
                        reg.remarks,
                        filePath,
                        id = Convert.ToInt32(reg.insurenceActionTrackerId),
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
        public async Task<IActionResult> DeleteDocument(deleteDocumentDto reg)
        {
            try
            {
                if (reg.filePath != null)
                {
                    var fileUrl = reg.filePath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + fileUrl;
                    // Get the file path from the URL
                    //string filePath = _hostingEnvironment.ContentRootPath + '\\' + fileUrl;

                    // Check if the file exists
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    else
                    {
                        return BadRequest("File can not be delete because it can't be found");

                    }
                    IATrackingFile f = await (from a in _context.IATrackingFile.Where(a => a.iatId == reg.id && a.iatFileId == reg.docId)
                                              select a).FirstOrDefaultAsync();
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
        [Authorize]
        [HttpPost("saveDocumnetTil")]
        public async Task<IActionResult> SaveDocumentTil([FromForm] uploadDocumentTilDto reg)
        {
            try
            {
                if (reg.file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.file.FileName);
                    string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await reg.file.CopyToAsync(stream);
                    }
                    string url = $"~/uploads/{fileName}";
                    TILActiontrackerFile fileIR = new TILActiontrackerFile();
                    fileIR.reportPath = url;
                    fileIR.tapId = Convert.ToInt32(reg.tapId);
                    fileIR.equipId = Convert.ToInt32(reg.equipId);
                    fileIR.reportName = reg.name;
                    fileIR.createdBy = Convert.ToInt32(reg.userId);
                    fileIR.remarks = reg.remarks;
                    fileIR.isDeleted = 0;
                    fileIR.createdOn = DateTime.Now;
                    _context.Add(fileIR);
                    _context.SaveChanges();
            

                    var obj = new
                    {
                        reg.name,
                        reg.remarks,
                        filePath,
                        tapId = Convert.ToInt32(reg.tapId),
                        equipId = Convert.ToInt32(reg.equipId),
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
        [HttpPost("deleteDocumentTil")]
        public async Task<IActionResult> DeleteDocumentTil(deleteDocumentDtoTil reg)
        {
            try
            {
                if (reg.filePath != null)
                {
                    var fileUrl = reg.filePath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + fileUrl;
                    // Get the file path from the URL
                    //string filePath = _hostingEnvironment.ContentRootPath + '\\' + fileUrl;

                    // Check if the file exists
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    else
                    {
                        return BadRequest("File can not be delete because it can't be found");

                    }
                    TILActiontrackerFile f = await (from a in _context.TILActiontrackerFile.Where(a => a.tapfId == reg.docId)
                                              select a).FirstOrDefaultAsync();
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
