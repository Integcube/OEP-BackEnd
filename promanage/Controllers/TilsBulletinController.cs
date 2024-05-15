using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class TilsBulletinController : BaseAPIController
    {

        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public TilsBulletinController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("deleteTilsBulletins")]
        public async Task<IActionResult> DeleteTilsBulletin(TILBulletin reg)
        {
            try
            {
                TILBulletin til = await (from r in _context.TILBulletin.Where(a => a.tilId == reg.tilId)
                                         select r).FirstOrDefaultAsync();
                til.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(til);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("updateTilsBulletins")]
        public async Task<IActionResult> UpdateTilsBulletin(tilUserDto reg)
        {
            try
            {
                if (reg.til.tilId == -1)
                {
                    TILBulletin til = new TILBulletin();
                    til.tilNumber = reg.til.tilNumber;
                    til.tilNumber = reg.til.tilNumber;
                    til.tilTitle = reg.til.tilTitle;
                    til.alternateNumber = reg.til.alternateNumber;

                    til.oem = reg.til.oem;
                    til.recommendations = reg.til.recommendations;
                    til.oemTimimgCodeId = reg.til.oemTimingId;
                    til.oemSeverityId = reg.til.oemSeverityId;
                    til.focusId = reg.til.tilFocusId;
                    til.dateReceivedNomac = reg.til.dateReceivedNomac;
                    til.dateIssuedDocument = reg.til.dateIssuedDocument;
                    til.currentRevision = reg.til.currentRevision;
                    til.documentTypeId = reg.til.documentTypeId;
                    til.sourceId = reg.til.sourceId;
                    til.reviewStatusId = reg.til.reviewStatusId;
                    til.notes = reg.til.notes;
                    til.reviewForumId = reg.til.reviewForumId;
                    til.componentId = reg.til.componentId;
                    til.applicabilityNotes = reg.til.applicabilityNotes;
                    til.report = reg.til.report;
                    til.yearOfIssue = reg.til.yearOfIssue;
                    til.implementationNotes = reg.til.implementationNotes;
                    til.createdBy = reg.userId;
                    til.createdOn = DateTime.Now;
                    til.tbEquipmentId = reg.til.tbEquipmentId;
                    _context.Add(til);
                    _context.SaveChanges();
                    reg.til.tilId = til.tilId;
                    return Ok(reg.til);
                }
                else
                {
                    TILBulletin til = await (from r in _context.TILBulletin.Where(a => a.tilId == reg.til.tilId)
                                             select r).FirstOrDefaultAsync();
                    til.tilNumber = reg.til.tilNumber;
                    til.tilNumber = reg.til.tilNumber;
                    til.tilTitle = reg.til.tilTitle;
                    til.oem = reg.til.oem;
                    til.alternateNumber = reg.til.alternateNumber;
                    til.recommendations = reg.til.recommendations;
                    til.oemTimimgCodeId = reg.til.oemTimingId;
                    til.oemSeverityId = reg.til.oemSeverityId;
                    til.focusId = reg.til.tilFocusId;
                    til.dateReceivedNomac = reg.til.dateReceivedNomac;
                    til.dateIssuedDocument = reg.til.dateIssuedDocument;
                    til.currentRevision = reg.til.currentRevision;
                    til.documentTypeId = reg.til.documentTypeId;
                    til.sourceId = reg.til.sourceId;
                    til.reviewStatusId = reg.til.reviewStatusId;
                    til.notes = reg.til.notes;
                    til.reviewForumId = reg.til.reviewForumId;
                    til.componentId = reg.til.componentId;
                    til.applicabilityNotes = reg.til.applicabilityNotes;
                    til.report = reg.til.report;
                    til.yearOfIssue = reg.til.yearOfIssue;
                    til.implementationNotes = reg.til.implementationNotes;
                    til.tbEquipmentId = reg.til.tbEquipmentId;
                    til.modifiedBy = reg.userId;
                    til.modifiedOn = DateTime.Now;
                    _context.SaveChanges();
                    return Ok(reg.til);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("downloadReport/{tilId}")]
        public async Task<IActionResult> DownloadTilFile(int tilId)
        {
            try
            {
                var fileIR = await (from a in _context.TILBulletinFile.Where(a => a.tilId == tilId && a.isDeleted == 0)
                                    select a).FirstOrDefaultAsync();

                if (fileIR != null)
                {
                    var tilUrl = fileIR.reportPath.TrimStart('~').Replace('/', '\\');
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + tilUrl;
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", fileIR.reportPath);
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
        public async Task<IActionResult> uploadFiles([FromForm] uploadTilFileDto reg)
        {
            try
            {

                if (reg.tilReport != null)
                {
                    string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.tilReport.FileName);
                    string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", tilFileName);
                    using (var stream = new FileStream(tilfilePath, FileMode.Create))
                    {
                        await reg.tilReport.CopyToAsync(stream);
                    }
                    string tilFileUrl = $"~/uploads/{tilFileName}";


                    _context.Database.ExecuteSqlCommand("DELETE FROM TILBulletinFile WHERE tilId = @tilId", new SqlParameter("tilId", Convert.ToInt32(reg.tilId)));
                    TILBulletinFile fileIR = new TILBulletinFile();
                    fileIR.reportPath = tilFileUrl;
                    fileIR.tilId = Convert.ToInt32(reg.tilId);
                    fileIR.reportName = reg.tilReport.FileName;
                    fileIR.createdOn = DateTime.Now;
                    fileIR.createdBy = Convert.ToInt32(reg.userId);
                    fileIR.isDeleted = 0;
                    _context.Add(fileIR);


                }


                _context.SaveChanges();
                return Ok();
            }
            catch (Exception E)
            {
                return BadRequest(E.Message);
            }
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {

                var tilComponent = await (from tc in _context.TILComponent.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.componentId,
                                              tc.componentTitle
                                          }).ToListAsync();
                var tilDocType = await (from tc in _context.TILDocumentType.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            tc.typeId,
                                            tc.typeTitle
                                        }).ToListAsync();

                var oemSeverity = await (from tc in _context.TILOEMSeverity.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.oemSeverityId,
                                             tc.oemSeverityTitle
                                         }).ToListAsync();
                var oemSeverityTiming = await (from tc in _context.TILOEMTimimgCode.Where(a => a.isDeleted == 0)
                                               select new
                                               {
                                                   tc.timingId,
                                                   tc.timingCode
                                               }).ToListAsync();

                var tilFocus = await (from tc in _context.TILFocus.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          tc.focusId,
                                          tc.focusTitle
                                      }).ToListAsync();
                var reviewForum = await (from tc in _context.TILReviewForm.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.reviewFormId,
                                             tc.reviewFormTitle
                                         }).ToListAsync();
                var reviewStatus = await (from tc in _context.TILReviewStatus.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.reviewStatusId,
                                              tc.reviewStatusTitle
                                          }).ToListAsync();
                var tilSite = await (from tc in _context.Sites.Where(a => a.isDeleted == 0)
                                     select new
                                     {
                                         tc.siteId,
                                         tc.siteName
                                     }).ToListAsync();
                var tilSource = await (from tc in _context.TILSource.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           tc.sourceId,
                                           tc.sourceTitle
                                       }).ToListAsync();

                var tbEquipemnt = await (from tc in _context.TilBulletinEquipment.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             tc.tilEquipmentId,
                                             tc.title,
                                         }).ToListAsync();
                var obj = new
                {
                    tilComponent,
                    tilDocType,
                    oemSeverity,
                    oemSeverityTiming,
                    tilFocus,
                    reviewForum,
                    reviewStatus,
                    tilSite,
                    tilSource,
                    tbEquipemnt
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [Authorize]
        [HttpPost("getTils")]
        public async Task<IActionResult> GetTils(getTilUserFilterDto rege)
        {
            try
            {
                List<int> DocIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.docTypeList))
                    DocIds = (rege.docTypeList.Split(',').Select(Int32.Parse).ToList());

                List<int> StausIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.statusList))
                    StausIds = (rege.statusList.Split(',').Select(Int32.Parse).ToList());

                List<int> EquipmentIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.equipmentList))
                    EquipmentIds = (rege.equipmentList.Split(',').Select(Int32.Parse).ToList());


                List<int> FormIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.formList))
                    FormIds = (rege.formList.Split(',').Select(Int32.Parse).ToList());

                List<int> FocusIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.focusList))
                    FocusIds = (rege.focusList.Split(',').Select(Int32.Parse).ToList());

                List<int> SeveruityIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.severityList))
                    SeveruityIds = (rege.severityList.Split(',').Select(Int32.Parse).ToList());

                var tils = await (from t in _context.TILBulletin.Where(a => a.isDeleted == 0 && ((DocIds.Count == 0) || DocIds.Contains((int)a.documentTypeId)) && ((FormIds.Count == 0) || FormIds.Contains((int)a.reviewForumId)) && ((StausIds.Count == 0) || StausIds.Contains((int)a.reviewStatusId)) && ((FocusIds.Count == 0) || FocusIds.Contains((int)a.focusId)) && ((SeveruityIds.Count == 0) || SeveruityIds.Contains((int)a.oemSeverityId)))
                                  join c in _context.TILComponent on t.componentId equals c.componentId into all
                                  from aa in all.DefaultIfEmpty()
                                  join d in _context.TILDocumentType on t.documentTypeId equals d.typeId into all2
                                  from bb in all2.DefaultIfEmpty()
                                  join f in _context.TILFocus on t.focusId equals f.focusId into all4
                                  from dd in all4.DefaultIfEmpty()
                                  join os in _context.TILOEMSeverity on t.oemSeverityId equals os.oemSeverityId into all5
                                  from ee in all5.DefaultIfEmpty()
                                  join ot in _context.TILOEMTimimgCode on t.oemTimimgCodeId equals ot.timingId into all6
                                  from ff in all6.DefaultIfEmpty()
                                  join rf in _context.TILReviewForm on t.reviewForumId equals rf.reviewFormId into all7
                                  from gg in all7.DefaultIfEmpty()
                                  join rs in _context.TILReviewStatus on t.reviewStatusId equals rs.reviewStatusId into all8
                                  from hh in all8.DefaultIfEmpty()
                                  join ts in _context.TILSource on t.sourceId equals ts.sourceId into all9
                                  from ii in all9.DefaultIfEmpty()
                                  join techEval in _context.TechnicalEvaluation on t.technicalReviewId equals techEval.teId into all12
                                  from nn in all12.DefaultIfEmpty()
                                  join fff in _context.TILBulletinFile on t.tilId equals fff.tilId into all122
                                  from nnn in all122.DefaultIfEmpty()
                                  join tbe in _context.TilBulletinEquipment on t.tbEquipmentId equals tbe.tilEquipmentId into all1222
                                  from tbee in all1222.DefaultIfEmpty()
                                  select new
                                  {
                                      t.tilId,
                                      t.tilNumber,
                                      t.alternateNumber,
                                      t.applicabilityNotes,
                                      t.tilTitle,
                                      t.currentRevision,
                                      tilFocusId = t.focusId,
                                      tilFocusTitle = dd.focusTitle,
                                      documentTypeId = t.documentTypeId,
                                      documentTypeTitle = bb.typeTitle,
                                      t.oem,
                                      oemSeverityId = t.oemSeverityId,
                                      oemSeverityTitle = ee.oemSeverityTitle,
                                      oemTimingId = t.oemTimimgCodeId,
                                      oemTimingTitle = ff.timingCode,
                                      reviewForumId = t.reviewForumId,
                                      reviewForumtitle = gg.reviewFormTitle,
                                      t.recommendations,
                                      technicalReviewId = nn.teId,
                                      technicalReviewUserId = nn.userId,
                                      technicalReviewSummary = nn.technicalEvaluation,
                                      t.dateReceivedNomac,
                                      t.dateIssuedDocument,
                                      sourceId = t.sourceId,
                                      sourceTitle = ii.sourceTitle,
                                      reviewStatusId = t.reviewStatusId,
                                      reviewStatusTitle = hh.reviewStatusTitle,
                                      t.notes,
                                      componentId = t.componentId,
                                      componentTitle = aa.componentTitle,
                                      t.implementationNotes,
                                      t.report,
                                      t.yearOfIssue,
                                      reportAttahced = nnn.tbfId == null ? false : true,
                                      reportName = nnn.reportName,
                                      tbEquipmentId =  t.tbEquipmentId == null? -1: t.tbEquipmentId,
                                      tbTitle = tbee.title,
                                  }).OrderByDescending(a => a.tilId).ToListAsync();

                var obj = new
                {
                    tils = tils.Where(a => (EquipmentIds.Count == 0) || EquipmentIds.Contains((int)a.tbEquipmentId)).ToList()
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
