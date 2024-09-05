using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ActionTrakingSystem.Controllers
{
    public class OT_OutageTrackerController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public OT_OutageTrackerController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("outageInfo")]
        public async Task<IActionResult> OutageInfo(OT_OutageTrackerUserSDto reg)
        {
            try
            {
                var userData = await (from c in _context.AUSite.Where(a => a.isDeleted == 0 && a.siteId == reg.action.siteId)
                                      join d in _context.AppUser.Where(a => a.isDeleted == 0) on c.userId equals d.userId
                                      join z in _context.OT_IActionOwnerUser on d.userId equals z.userId
                                      join b in _context.OT_PhaseReadinessDescriptionAO on new { z.actionOwnerId, p = reg.action.phaseReadId } equals new { b.actionOwnerId, p = b.phaseReadId }
                                      select new
                                      {
                                          d.userId,
                                          d.userName,
                                          name = d.firstName + " " + d.lastName,
                                      }).Distinct().ToListAsync();

                var outageData = await (from a in _context.OT_PhaseOutageTrackerProgress.Where(a => a.potId == reg.action.potId ) //&& a.monthId == reg.action.startDate.Month && a.yearId == reg.action.startDate.Year
                                        orderby a.potId descending
                                        select new
                                        {
                                            a.monthId,
                                            a.yearId,
                                            a.remarks,
                                            a.progress,
                                            a.progressId,
                                        }).FirstOrDefaultAsync();
                List<OT_DateCalc> monthList = new List<OT_DateCalc>();

                //var monthDifference = CalculateMonthDifference(reg.action.startDate, reg.action.endDate);
                //if (monthDifference == 0)
                //{
                //    OT_DateCalc c = new OT_DateCalc();
                //    c.date = reg.action.startDate;
                //    c.monthId = reg.action.startDate.Month + "-" + reg.action.startDate.Year;
                //    monthList.Add(c);
                //}
                //else
                //{
                //    for (var i = 0; i < monthDifference; i++)
                //    {
                //        OT_DateCalc c = new OT_DateCalc();
                //        c.date = reg.action.startDate.AddMonths(i);
                //        c.monthId = c.date.Month + "-" + c.date.Year;
                //        monthList.Add(c);
                //    }
                //}

                var obj = new
                {
                    outageData,
                    monthList,
                    userData
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("outageInfoC")]
        public async Task<IActionResult> OutageInfoC(OT_OutageTrackerUserCDto reg)
        {
            try
            {
                string[] parts = reg.monthId.Split('-');
                int month = 0;
                int year = 0;
                if (parts.Length == 2)
                {
                    month = int.Parse(parts[0]);
                    year = int.Parse(parts[1]);
                }
                var outageData = await (from a in _context.OT_PhaseOutageTrackerProgress.Where(a => a.potId == reg.potId ) //&& a.monthId == month && a.yearId == year
                                        select new
                                        {
                                            a.monthId,
                                            a.yearId,
                                            a.remarks,
                                            a.progress,
                                            a.progressId,
                                        }).FirstOrDefaultAsync();
                return Ok(outageData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        static int CalculateMonthDifference(DateTime startDate, DateTime endDate)
        {
            int yearDifference = endDate.Year - startDate.Year;
            int monthDifference = endDate.Month - startDate.Month;

            // Adjust for negative month difference and carryover from years
            if (monthDifference < 0)
            {
                monthDifference += 12;
                yearDifference--;
            }

            // Calculate the total month difference
            int totalMonthDifference = (yearDifference * 12) + monthDifference;

            return totalMonthDifference;
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var phaseNumber = await (from a in _context.OT_Phase.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             a.phaseId,
                                             a.phaseNumber
                                         }).OrderBy(b => b.phaseNumber).ToListAsync();
                var outage = await (from a in _context.OT_ISiteOutages.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.outageId,
                                        a.outageTitle
                                    }).ToListAsync();
                var status = await (from a in _context.OT_IStatus
                                    select new
                                    {
                                        a.statusTitle,
                                        a.statusId
                                    }).ToListAsync();

                var obj = new
                {
                    phaseNumber,
                    outage,
                    status
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public static DateTime AddDecimalMonths(DateTime date, decimal months)
        {
            int wholeMonths = (int)months;
            double fractionalMonths = (double)(months - wholeMonths);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            date = date.AddMonths(wholeMonths);
            date = date.AddDays(fractionalMonths * daysInMonth);

            return date;
        }
        [Authorize]
        [HttpPost("getOutageTracker")]
        public async Task<IActionResult> GetOutageTracker(OT_FilterUserDto reg)
        {
            try
            {
                DateTime filterDate = new DateTime(2023, 12, 31);
                var outagesInfo = await (from d in _context.OT_Phase.Where(a => a.isDeleted == 0)
                                         join e in _context.OT_PhaseDuration.Where(a => a.isDeleted == 0) on d.phaseId equals e.phaseId
                                         select new
                                         {
                                             d.phaseId,
                                             e.outageId,
                                             e.durationMonths
                                         }
                                     ).ToListAsync();

                var actions2 = await (from a in _context.OT_IActionOwnerUser.Where(a => a.userId == reg.userId)
                                      join u in _context.OT_IActionOwner.Where(a => a.isDeleted == 0) on a.actionOwnerId equals u.actionOwnerId
                                      join b in _context.OT_PhaseReadinessDescriptionAO.Where(a => a.isDeleted == 0) on a.actionOwnerId equals b.actionOwnerId
                                      join c in _context.OT_PhaseReadinessDescription.Where(a => a.isDeleted == 0) on b.phaseReadId equals c.phaseReadId
                                      join d in _context.OT_Phase.Where(a => a.isDeleted == 0 && (reg.filter.phaseNumber == -1 || a.phaseId == reg.filter.phaseNumber)) on c.phaseId equals d.phaseId
                                      join e in _context.OT_PhaseDuration.Where(a => a.isDeleted == 0) on d.phaseId equals e.phaseId
                                      join f in _context.OT_SiteNextOutages.Where(a => a.isDeleted == 0 && a.nextOutageDate > filterDate) on e.outageId equals f.outageId
                                      join ff in _context.OT_ISiteOutages.Where(a => a.isDeleted == 0 && (reg.filter.outageId == -1 || a.outageId == reg.filter.outageId)) on f.outageId equals ff.outageId
                                      join eq in _context.OT_SiteEquipment.Where(a => a.isDeleted == 0) on f.equipmentId equals eq.equipmentId
                                      join g in _context.Sites.Where(a => a.isDeleted == 0 && (reg.filter.siteId == -1 || a.siteId == reg.filter.siteId) && (a.otValid == 1)) on eq.siteId equals g.siteId
                                      join z in _context.Country on g.countryId equals z.countryId
                                      join zz in _context.Cluster on z.clustedId equals zz.clusterId
                                      join r in _context.Regions2 on zz.regionId equals r.regionId
                                      join h in _context.AUSite.Where(a => a.userId == reg.userId) on g.siteId equals h.siteId
                                      join i in _context.OT_PhaseOutageTracker.Where(a => a.isDeleted == 0)
                                                                                      on new { PhaseId = d.phaseId, PhaseReadId = c.phaseReadId, SNOId = f.snoId }
                                                                                  equals new { PhaseId = i.phaseId, PhaseReadId = i.phaseReadId, SNOId = i.snoId ?? -1 } into all
                                      from ii in all.DefaultIfEmpty()
                                      join j in _context.OT_IStatus on ii.statusId equals j.statusId into all2
                                      from jj in all2.DefaultIfEmpty()
                                      select new OutageTrackerModel
                                      {
                                          siteId = g.siteId,
                                          siteTitle = g.siteName,
                                          phaseId = d.phaseId,
                                          phaseNumber = d.phaseNumber,
                                          phaseTitle = d.phaseTitle,
                                          phaseReadId = c.phaseReadId,
                                          phaseReadDesc = c.phaseReadDesc,
                                          clusterId = zz.clusterId,
                                          clusterTitle = zz.clusterTitle,
                                          //remarks = ii.remarks,
                                          //filePath = ii.filePath,
                                          //fileName = ii.fileName,
                                          //progress = ii.progress,
                                          notApplicable = ii.notApplicable == null || ii.notApplicable == 0 ? false : true,
                                          statusId = jj.statusId == null ? 1 : jj.statusId,
                                          statusTitle = jj.statusTitle == null ? "Pending" : jj.statusTitle,
                                          potId = ii.potId == null ? -1 : ii.potId,
                                          //isCompleted = ii.isCompleted == null || ii.isCompleted == 0 ? false:true,
                                          snoId = f.snoId,
                                          outageId = f.outageId,
                                          nextOutageDate = f.nextOutageDate,
                                          outageTitle = ff.outageTitle,
                                          phaseDurId = e.phaseDurId,
                                          durationMonths = e.durationMonths,
                                          equipmentId = eq.equipmentId,
                                          unit = eq.unit,
                                          startDate = DateTime.Now,
                                          endDate = DateTime.Now,
                                      }
                                    ).Distinct().OrderBy(a => a.nextOutageDate).ToListAsync();

                var combinedUser = (from k in actions2
                                    join c in _context.AUSite.Where(a => a.isDeleted == 0) on k.siteId equals c.siteId
                                    join d in _context.AppUser.Where(a => a.isDeleted == 0) on c.userId equals d.userId
                                    join z in _context.OT_IActionOwnerUser on d.userId equals z.userId
                                    join b in _context.OT_PhaseReadinessDescriptionAO on new { ActionOwnerId = z.actionOwnerId, k.phaseReadId } equals new { ActionOwnerId = b.actionOwnerId, b.phaseReadId }
                                    select new
                                    {
                                        userName = d.userName,
                                        k.siteId,
                                        k.siteTitle,
                                        k.phaseId,
                                        k.phaseNumber,
                                        k.phaseTitle,
                                        k.phaseReadId,
                                        k.phaseReadDesc,
                                        k.clusterId,
                                        k.clusterTitle,
                                        k.notApplicable,
                                        k.statusId,
                                        k.statusTitle,
                                        k.potId,
                                        k.snoId,
                                        k.outageId,
                                        k.nextOutageDate,
                                        k.outageTitle,
                                        k.phaseDurId,
                                        k.durationMonths,
                                        k.equipmentId,
                                        k.unit,
                                        k.startDate,
                                        k.endDate,
                                    }).Distinct().ToList();

                var actions = combinedUser
                 .GroupBy(k => new
                 {
                     k.siteId,
                     k.siteTitle,
                     k.phaseId,
                     k.phaseNumber,
                     k.phaseTitle,
                     k.phaseReadId,
                     k.phaseReadDesc,
                     k.clusterId,
                     k.clusterTitle,
                     k.notApplicable,
                     k.statusId,
                     k.statusTitle,
                     k.potId,
                     k.snoId,
                     k.outageId,
                     k.nextOutageDate,
                     k.outageTitle,
                     k.phaseDurId,
                     k.durationMonths,
                     k.equipmentId,
                     k.unit,
                     k.startDate,
                     k.endDate
                 })
                       .Select(group => new OutageTrackerModel
                       {
                           siteId = group.FirstOrDefault().siteId,
                           siteTitle = group.FirstOrDefault().siteTitle,
                           phaseId = group.FirstOrDefault().phaseId,
                           phaseNumber = group.FirstOrDefault().phaseNumber,
                           phaseTitle = group.FirstOrDefault().phaseTitle,
                           phaseReadId = group.FirstOrDefault().phaseReadId,
                           phaseReadDesc = group.FirstOrDefault().phaseReadDesc,
                           clusterId = group.FirstOrDefault().clusterId,
                           clusterTitle = group.FirstOrDefault().clusterTitle,
                           notApplicable = group.FirstOrDefault().notApplicable,
                           statusId = group.FirstOrDefault().statusId,
                           statusTitle = group.FirstOrDefault().statusTitle,
                           potId = group.FirstOrDefault().potId,
                           snoId = group.FirstOrDefault().snoId,
                           outageId = group.FirstOrDefault().outageId,
                           nextOutageDate = group.FirstOrDefault().nextOutageDate,
                           outageTitle = group.FirstOrDefault().outageTitle,
                           phaseDurId = group.FirstOrDefault().phaseDurId,
                           durationMonths = group.FirstOrDefault().durationMonths,
                           equipmentId = group.FirstOrDefault().equipmentId,
                           unit = group.FirstOrDefault().unit,
                           startDate = group.FirstOrDefault().startDate,
                           endDate = group.FirstOrDefault().endDate,
                           name = string.Join(", ", group.Select(u => u.userName).Distinct())
                       }).ToList();


                var todayDate = DateTime.Now;
                for (var a = 0; a < actions.Count; a++)
                {
                    var uniqueEquipments = outagesInfo.Where(b => b.outageId == actions[a].outageId && b.durationMonths < actions[a].durationMonths).OrderByDescending(a => a.durationMonths).ToList();
                    var uniqueNegativeEquipments = outagesInfo.Where(b => b.outageId == actions[a].outageId && b.durationMonths > actions[a].durationMonths && b.durationMonths < 0).OrderByDescending(a => a.durationMonths).ToList();

                    if (actions[a].durationMonths > 0)
                    {
                        actions[a].startDate = actions[a].nextOutageDate.AddMonths(-(int)((actions[a].durationMonths == null) ? 0 : actions[a].durationMonths));
                    }
                    else if (actions[a].durationMonths < 0)
                    {
                        actions[a].startDate = actions[a].nextOutageDate.AddMonths(-(int)((uniqueNegativeEquipments.Count == 0) ? 0 : uniqueNegativeEquipments[0].durationMonths));
                    }
                    DateTime obd = actions[a].startDate;
                    var dds = (uniqueEquipments.Count > 0 ? (actions[a].durationMonths - ((uniqueEquipments.FirstOrDefault().durationMonths == 0) ? 0 : uniqueEquipments.FirstOrDefault().durationMonths)) : 0);
                    if (actions[a].durationMonths > 0)
                    {
                        actions[a].endDate = obd.AddMonths((int)dds);
                    }
                    else if (actions[a].durationMonths < 0)
                    {
                        actions[a].endDate = actions[a].startDate.AddMonths(-(int)((actions[a].durationMonths == null) ? 0 : ((uniqueNegativeEquipments.Count == 0) ? actions[a].durationMonths : (actions[a].durationMonths - uniqueNegativeEquipments[0].durationMonths))));
                    }
                    var diffOfMonths = ((actions[a].nextOutageDate.Year - todayDate.Year) * 12) + actions[a].nextOutageDate.Month - todayDate.Month;
                    if (diffOfMonths > actions[a].durationMonths)
                    {
                        actions.RemoveAt(a);
                        a--;
                    }
                }
                return Ok(actions.Where(a => reg.filter.status == -1 || a.statusId == reg.filter.status));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveOutageTracker")]
        public async Task<IActionResult> SaveOutageTracker(OT_OutageTrackerUserDto reg)
        {
            try
            {
                if (reg.action.outageTracker.potId == -1)
                {
                    OT_PhaseOutageTracker ot = new OT_PhaseOutageTracker();
                    ot.equipmentId = reg.action.outageTracker.equipmentId;
                    ot.phaseId = reg.action.outageTracker.phaseId;
                    ot.phaseReadId = reg.action.outageTracker.phaseReadId;
                    ot.outageDate = reg.action.outageTracker.nextOutageDate;
                    ot.snoId = reg.action.outageTracker.snoId;
                    if (reg.action.outageTracker.notApplicable == true)
                    {
                        ot.notApplicable = 1;
                        ot.statusId = 3;
                    }
                    else
                    {
                        ot.notApplicable = 0;
                        ot.statusId = 1;
                    }
                    ot.createdOn = DateTime.Now;
                    ot.createdBy = reg.userId;
                    _context.Add(ot);
                    _context.SaveChanges();
                    reg.action.outageTracker.potId = ot.potId;
                    OT_PhaseOutageTrackerProgress p = new OT_PhaseOutageTrackerProgress();
                    p.monthId = reg.action.monthlyData.monthId;
                    p.yearId = reg.action.monthlyData.yearId;
                    p.potId = ot.potId;
                    p.progress = reg.action.monthlyData.progress;
                    p.remarks = reg.action.monthlyData.remarks;
                    p.createdBy = reg.userId;
                    p.createdOn = DateTime.Now;
                    _context.Add(p);
                    if (ot.notApplicable != 1)
                    {
                        if (p.progress <= 0)
                            ot.statusId = 1;
                        else if (p.progress >= 100)
                            ot.statusId = 3;
                        else
                            ot.statusId = 2;
                    }
                    _context.SaveChanges();
                    reg.action.monthlyData.progressId = p.progressId;
                }
                else
                {
                    var ot = await (from a in _context.OT_PhaseOutageTracker.Where(a => a.potId == reg.action.outageTracker.potId)
                                    select a).FirstOrDefaultAsync();
                    if (reg.action.outageTracker.notApplicable == true)
                    {
                        ot.notApplicable = 1;
                        ot.statusId = 3;
                    }
                    else
                    {
                        ot.notApplicable = 0;
                        ot.statusId = 1;
                    }

                    ot.modifiedOn = DateTime.Now;
                    ot.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    var p = await (from a in _context.OT_PhaseOutageTrackerProgress.Where(a => (a.progressId == reg.action.monthlyData.progressId))
                                   select a).FirstOrDefaultAsync();
                    if (p != null)
                    {
                        p.monthId = reg.action.monthlyData.monthId;
                        p.yearId = reg.action.monthlyData.yearId;
                        p.potId = ot.potId;
                        p.progress = reg.action.monthlyData.progress;
                        p.remarks = reg.action.monthlyData.remarks;
                        p.createdBy = reg.userId;
                        p.createdOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else
                    {

                        OT_PhaseOutageTrackerProgress q = new OT_PhaseOutageTrackerProgress();
                        q.monthId = reg.action.monthlyData.monthId;
                        q.yearId = reg.action.monthlyData.yearId;
                        q.potId = ot.potId;
                        q.progress = reg.action.monthlyData.progress;
                        q.remarks = reg.action.monthlyData.remarks;
                        q.createdBy = reg.userId;
                        q.createdOn = DateTime.Now;
                        _context.Add(q);
                        _context.SaveChanges();

                    }
                    var cc = (from a in _context.OT_PhaseOutageTrackerProgress.Where(a => a.potId == ot.potId)
                              select a).OrderByDescending(a => a.progress).ToList();
                    if (cc.Count > 0)
                    {
                        var finalP = cc.FirstOrDefault().progress;
                        if (ot.notApplicable != 1)
                        {
                            if (finalP <= 0)
                                ot.statusId = 1;
                            else if (finalP >= 100)
                                ot.statusId = 3;
                            else
                                ot.statusId = 2;
                        }
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.action);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //Files
        [Authorize]
        [HttpPost("deleteDocument")]
        public async Task<IActionResult> DeleteDocument(OT_OutageTrackerFileDto reg)
        {
            try
            {
                var fileIR = await (from a in _context.OT_PhaseOutageTrackerFile.Where(a => a.fileId == reg.fileId)
                                    select a).FirstOrDefaultAsync();
                if (fileIR != null)
                {
                    fileIR.isDeleted = 1;
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
        public async Task<IActionResult> uploadFiles([FromForm] OT_UploadFileDto reg)
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
                    string FileUrl = $"~/OT_Uploads/{FileName}";
                    OT_PhaseOutageTrackerFile ot = new OT_PhaseOutageTrackerFile();
                    ot.path = filePath;
                    ot.equipmentId = Convert.ToInt32(reg.equipmentId);
                    ot.phaseId = Convert.ToInt32(reg.phaseId);
                    ot.outageDate = DateTime.Parse(reg.outageDate);
                    ot.phaseReadId = Convert.ToInt32(reg.phaseReadId);
                    ot.potId = Convert.ToInt32(reg.potId);
                    ot.fileName = reg.fileName;
                    ot.remarks = reg.remarks;
                    ot.createdBy = Convert.ToInt32(reg.userId);
                    ot.createdOn = DateTime.Now;
                    ot.isDeleted = 0;
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
        public async Task<IActionResult> GetFiles(OT_OutageTrackerUserSDto reg)
        {
            try
            {
                var files = await (from a in _context.OT_PhaseOutageTrackerFile.Where(a => a.equipmentId == reg.action.equipmentId && a.phaseId == reg.action.phaseId && a.phaseReadId == reg.action.phaseReadId && a.potId == reg.action.potId && a.isDeleted == 0)
                                   select new
                                   {
                                       a.fileId,
                                       a.fileName,
                                       a.remarks,
                                       a.path,
                                   }).ToListAsync();

                return Ok(files);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("downloadFile/{fileId}")]
        public async Task<IActionResult> DownloadTilFile(int fileId)
        {
            try
            {
                var fileIR = await (from a in _context.OT_PhaseOutageTrackerFile.Where(a => a.fileId == fileId)
                                    select a).FirstOrDefaultAsync();

                if (fileIR != null)
                {
                    var filePath = fileIR.path.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    //string filePath = _hostingEnvironment.ContentRootPath + '\\' + tilUrl;

                    //Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", fileIR.path);
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
    }
}