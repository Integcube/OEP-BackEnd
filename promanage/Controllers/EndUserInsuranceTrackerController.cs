using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Data.SqlClient;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.Recommendations;

namespace ActionTrakingSystem.Controllers
{

    public class EndUserInsuranceTrackerController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EndUserInsuranceTrackerController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("downloadFile/{iatId}")]
        public async Task<IActionResult> DownloadTilFile(int iatId)
        {
            try
            {
                var fileIR = await (from a in _context.IATrackingFile.Where(a => a.iatId == iatId && a.isDeleted == 0)
                                    select a).FirstOrDefaultAsync();

                if (fileIR != null)
                {
                    var tilUrl = fileIR.path.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + tilUrl;

                    // Check if the file exists
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
        [Authorize]
        [HttpPost("uploadFile")]
        public async Task<IActionResult> uploadFiles([FromForm] iatFileDto reg)
        {
            try
            {

                if (reg.iatReport != null)
                {
                    string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.iatReport.FileName);
                    string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", tilFileName);
                    using (var stream = new FileStream(tilfilePath, FileMode.Create))
                    {
                        await reg.iatReport.CopyToAsync(stream);
                    }
                    string tilFileUrl = $"~/uploads/{tilFileName}";
                    _context.Database.ExecuteSqlCommand("DELETE FROM IATrackingFile WHERE iatId = @iatId", new SqlParameter("iatId", Convert.ToInt32(reg.iatId)));
                    IATrackingFile fileIR = new IATrackingFile();
                    fileIR.path = tilFileUrl;
                    fileIR.iatId = Convert.ToInt32(reg.iatId);
                    fileIR.fileName = reg.iatReport.FileName;
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
        [HttpPost("getAttachedFiles")]
        public async Task<IActionResult> GetAttachedFiles(iatFileListDto reg)
        {
            try
            {

                var list = await (from a in _context.IATrackingFile.Where(a => a.iatId == reg.iatId && a.isDeleted == 0)
                                  join b in _context.InsurenceActionTracker on a.iatId equals b.iaId
                                  select new
                                  {
                                      id = b.iaId,
                                      filePath = a.path,
                                      name = a.fileName,
                                      a.remarks,
                                      docId = a.iatFileId,
                                  }).ToListAsync();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> getInterfaces(UserIdDto reg)
        {
            try
            {

                var recommendation = await (from rec in _context.InsuranceRecommendations.Where(a => a.isDeleted == 0)
                                            join s in _context.Sites on rec.siteId equals s.siteId
                                            join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                            join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                            select new
                                            {
                                                recommendationId = rec.irId,
                                                recommendationTitle = rec.title,
                                                recommendationReference = rec.referenceNumber + "-" + rec.title,

                                            }).Distinct().ToListAsync();
                var iatSource = await (from iats in _context.IATrackingSource.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           iats.sourceId,
                                           iats.sourceTitle
                                       }).ToListAsync();
                var iatCompany = await (from iatc in _context.IATrackingCompany.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            companyId = iatc.iatId,
                                            companyTitle = iatc.iatTitle
                                        }).ToListAsync();
                var iatStatus = await (from iatst in _context.IATrackingStatus.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           iatst.statusId,
                                           iatst.statusTitle,
                                           statusScore = iatst.score,
                                       }).ToListAsync();
                var evidenceAvailable = await (from a in _context.IATrackingEvidence.Where(a => a.isDeleted == 0)
                                               select new
                                               {
                                                   evidenceTitle = a.aitEvidenceTitle,
                                                   evidenceId = a.iatEvidenceId,
                                                   evidenceScore = a.score
                                               }).ToListAsync();
                var daysStatus = await (from a in _context.IATrackingDaysStatus.Where(a => a.isDeleted == 0)
                                        select new
                                        {
                                            dayStatusTitle = a.iatDayStatus,
                                            dayStatusId = a.iatDayStatusId,
                                            dayStatusScore = a.score
                                        }).ToListAsync();
                var priority = await (from ip in _context.InsuranceRecPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          priorityId = ip.ipId,
                                          priorityTitle = ip.ipTitle
                                      }).ToListAsync();
                var obj = new
                {

                    iatSource,
                    iatCompany,
                    iatStatus,
                    evidenceAvailable,
                    daysStatus,
                    recommendation,
                    priority
                };

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPost("getIR")]
        public async Task<IActionResult> getIR(getIRDto reg)
        {
            try
            {
                var reommendations = await (from ir in _context.InsuranceRecommendations.Where(a => a.irId == reg.recommendationId)
                                            join s in _context.Sites on ir.siteId equals s.siteId
                                            join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                            join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                            join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                            join r in _context.Regions2 on s.region2 equals r.regionId
                                            join p in _context.ProactiveRiskPrevention on ir.proactiveId equals p.proactiveId into allIns
                                            from ains in allIns.DefaultIfEmpty()
                                            join t in _context.InsuranceRecType on ir.recommendationTypeId equals t.typeId into allsec
                                            from aa in allsec.DefaultIfEmpty()
                                            join rs in _context.InsuranceRecFile on ir.irId equals rs.irId into all4
                                            from rss in all4.DefaultIfEmpty()
                                            join i in _context.InsuranceRecSource on ir.sourceId equals i.sourceId
                                            join d in _context.InsuranceRecDocumentType on ir.documentTypeId equals d.documentId into allthree
                                            from bb in allthree.DefaultIfEmpty()
                                            join ns in _context.InsuranceRecNomacStatus on ir.nomacStatusId equals ns.nomacStatusId
                                            join insurStatus in _context.InsuranceRecInsurenceStatus on ir.insuranceStatusId equals insurStatus.insurenceStatusId
                                            join ip in _context.InsuranceRecPriority on ir.priorityId equals ip.ipId
                                            select new
                                            {
                                                recommendationId = ir.irId,
                                                ir.title,
                                                ir.insuranceRecommendation,
                                                ir.referenceNumber,
                                                priorityId = ip.ipId,
                                                priorityTitle = ip.ipTitle,
                                                insurenceStatusId = insurStatus.insurenceStatusId,
                                                insurenceStatusTitle = insurStatus.insurenceStatusTitle,
                                                nomacStatusId = ns.nomacStatusId,
                                                nomacStatusTitle = ns.nomacStatusTitle,
                                                ir.latestStatus,
                                                ir.targetDate,
                                                ir.siteUpdates,
                                                ir.pcComments,
                                                ir.type,
                                                ir.expectedBudget,
                                                siteTitle = s.siteName,
                                                siteId = ir.siteId,
                                                ir.significance,
                                                proactiveReference = ains.proactiveReference,
                                                proactiveId = ains.proactiveId,
                                                proactiveTitle = ains.proactivetitle,
                                                regionTitle = r.title,
                                                regionId = r.regionId,
                                                sourceId = i.sourceId,
                                                sourceTitle = i.sourceTitle,
                                                documentTypeId = bb.documentId,
                                                documentTypeTitle = bb.documnetTitle,
                                                ir.year,
                                                recommendationTypeId = aa.typeId,
                                                recommendationTypeTitle = aa.typeTitle,
                                                report = ir.report,
                                                reportAttahced = rss.faId == null ? false : true,
                                                reportName = rss.fileName,
                                            }).FirstOrDefaultAsync();


                return Ok(reommendations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPost("getInsurenceActionTracker")]
        public async Task<IActionResult> getInsurenceTracker(InsuranceTrackerUserFilterList reg)
        {
            try
            {
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

                List<int> ClusterIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.clusterList))
                    ClusterIds = (reg.clusterList.Split(',').Select(Int32.Parse).ToList());
                List<int> SourceIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sourceList))
                    SourceIds = (reg.sourceList.Split(',').Select(Int32.Parse).ToList());

                List<int> CompanyIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.companyList))
                    CompanyIds = (reg.companyList.Split(',').Select(Int32.Parse).ToList());
                List<int> QuarterIds = QuaterCalc(reg.quaterList);

                List<int> StatusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.statusList))
                    StatusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                List<string> DaysIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.dayList))
                    DaysIds = (reg.dayList.Split(',').ToList());

                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());
                List<int> YearIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.yearList))
                    YearIds = (reg.yearList.Split(',').Select(Int32.Parse).ToList());
                List<string> IssueYearIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.issueYearList))
                    IssueYearIds = (reg.issueYearList.Split(',').ToList());

                var nowDate = DateTime.Now;
                var tracker = await (from iat in _context.InsurenceActionTracker.Where(a => a.isDeleted == 0 &&
                                     ((SourceIds.Count == 0) || SourceIds.Contains((int)a.sourceId)) &&
                                     ((CompanyIds.Count == 0) || CompanyIds.Contains((int)a.iatCompanyId)) &&
                                     ((StatusIds.Count == 0) || StatusIds.Contains((int)a.iatStatusId)) &&
                                     ((QuarterIds.Count == 0) || QuarterIds.Contains((int)a.targetDate.Month)) &&
                                     ((YearIds.Count == 0) || YearIds.Contains((int)a.targetDate.Year)))
                                     join au in _context.AppUser.Where(a => a.isDeleted == 0) on iat.assignedTo equals au.userId into alluser
                                     from auu in alluser.DefaultIfEmpty()
                                     join ir in _context.InsuranceRecommendations.Where(sd => sd.isDeleted == 0 && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)sd.priorityId))
                                     && ((IssueYearIds.Count == 0) || IssueYearIds.Contains(sd.year))) on iat.recommendationId equals ir.irId
                                     join ip in _context.InsuranceRecPriority on ir.priorityId equals ip.ipId
                                     join ns in _context.InsuranceRecNomacStatus on ir.nomacStatusId equals ns.nomacStatusId
                                     join insurStatus in _context.InsuranceRecInsurenceStatus on ir.insuranceStatusId equals insurStatus.insurenceStatusId
                                     join d in _context.InsuranceRecDocumentType on ir.documentTypeId equals d.documentId into allthree
                                     from bccc in allthree.DefaultIfEmpty()


                                     join s in _context.Sites.Where(a => (SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)) on ir.siteId equals s.siteId
                                     join cn in _context.Country on s.countryId equals cn.countryId
                                     join cnvp in _context.CountryExecutiveVp on cn.countryId equals cnvp.countryId into allcn
                                     from cnvpp in allcn.DefaultIfEmpty()
                                         //join its in _context.InsuranceAccessControl.Where(a => (reg.userId == -1 || a.userId == reg.pocId)) on s.siteId equals its.siteId
                                     join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0) on s.siteId equals ts.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                     join st in _context.IATrackingStatus on iat.iatStatusId equals st.statusId into aa
                                     from bb in aa.DefaultIfEmpty()
                                     join ev in _context.IATrackingEvidence on iat.evidenceAvailableId equals ev.iatEvidenceId into all3
                                     from evd in all3.DefaultIfEmpty()
                                     join c in _context.IATrackingCompany on iat.iatCompanyId equals c.iatId into aaa
                                     from bbb in aaa.DefaultIfEmpty()
                                         //join iatr in _context.IATrackingFile on iat.iaId equals iatr.iatId into alll
                                         //from iatrr in alll.DefaultIfEmpty()
                                     join so in _context.IATrackingSource on iat.sourceId equals so.sourceId into aaaa
                                     from bbbb in aaaa.DefaultIfEmpty()
                                     join icc in _context.Cluster.Where(a => a.isDeleted == 0 && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))) on s.clusterId equals icc.clusterId
                                     join rege in _context.Regions2.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on s.region2 equals rege.regionId
                                     join rvp in _context.RegionsExecutiveVp on rege.regionId equals rvp.regionId into allrege
                                     from rvpp in allrege.DefaultIfEmpty()
                                     join ds in _context.IATrackingDaysStatus on iat.dayStatus equals ds.iatDayStatusId into all5
                                     from dsd in all5.DefaultIfEmpty()
                                     join uxx in _context.AppUser on iat.actionClosedBy equals uxx.userId into all90
                                     from uxxx in all90.DefaultIfEmpty()
                                     where (s.insurancePOCId == reg.userId || rege.executiveDirector == reg.userId || rvpp.userId == reg.userId || cnvpp.userId == reg.userId || cn.executiveDirector == reg.userId ||
                                    (iat == null ? -1 : iat.assignedTo) == reg.userId || reg.userId == 22 || reg.userId == 1)
                                     select new
                                     {
                                         assignedToId = iat.assignedTo == null ? -1 : iat.assignedTo,
                                         assignedToTitle = auu.firstName + " " + auu.lastName,
                                         insurenceActionTrackerId = iat.iaId,
                                         recommendationId = iat.recommendationId,
                                         recommendationTitle = ir.title,
                                         recommendationReference = ir.referenceNumber + '-' + ir.title,
                                         iat.action,
                                         insurenceStatusId = insurStatus.insurenceStatusId,
                                         insurenceStatusTitle = insurStatus.insurenceStatusTitle,
                                         nomacStatusId = ns.nomacStatusId,
                                         nomacStatusTitle = ns.nomacStatusTitle,
                                         iat.targetDate,
                                         statusId = iat.iatStatusId,
                                         statusTitle = bb.statusTitle,
                                         statusScore = bb.score,
                                         companyId = iat.iatCompanyId,
                                         companyTitle = bbb.iatTitle,
                                         iat.comments,
                                         iat.closureDate,
                                         ir.priorityId,
                                         priorityTitle = ip.ipTitle,
                                         isCompleted = iat.isCompleted == 1 ? true : false,
                                        iat.reviewerComment ,
                                         clusterReviewed = iat.clusterReviewed == 1 ? true : false,
                                         iat.implementedDate,
                                         rework = iat.rework == 1 ? true : false,
                                         evidenceAvailableId = iat.evidenceAvailableId == null? 2: iat.evidenceAvailableId,
                                         evidenceAvailable = evd.aitEvidenceTitle == null? "No": evd.aitEvidenceTitle,
                                         evidenceAvailableScore = evd.score,
                                         dayStatusId = iat.dayStatus,
                                         dayStatusTitle = dsd.iatDayStatus,
                                         dayStatusScore = dsd.score,
                                         iat.calcStatus,
                                         iat.calcEvid,
                                         ir.type,
                                         iat.adminComment,

                                         documentTypeId = bccc.documentId,
                                         documentTypeTitle = bccc.documnetTitle,
                                         iat.calcDate,
                                         iat.completionScore,
                                         iat.daysToTarget,
                                         iat.scoreDetails,
                                         ir.siteId,
                                         siteTitle = s.siteName,
                                         regionId = s.regionId,
                                         regionTitle = rege.title,
                                         iat.sourceId,
                                         sourceTitle = bbbb.sourceTitle,
                                         icc.clusterId,
                                         icc.clusterTitle,
                                         //reportAttahced = iatrr.iatFileId == null ? false : true,
                                         //reportName = iatrr.fileName,
                                         iat.actionClosedBy,
                                         actionClosedTitle = uxxx.firstName + " " + uxxx.lastName,
                                     }
                    ).Distinct().OrderByDescending(a => a.insurenceActionTrackerId).ToListAsync();
                var daysToTargetList = tracker.Select(s => new
                {

                    s.documentTypeTitle,
                    s.type,
                    s.assignedToId,
                    s.assignedToTitle,
                    s.insurenceActionTrackerId,
                    s.recommendationId,
                    s.recommendationTitle,
                    s.recommendationReference,
                    s.action,
                    s.targetDate,
                    s.statusId,
                    s.statusTitle,
                    s.statusScore,
                    s.insurenceStatusId,
                    s.insurenceStatusTitle,
                    s.nomacStatusId,
                    s.nomacStatusTitle,
                    s.companyId,
                    s.adminComment,
                    s.companyTitle,
                    s.comments,
                    s.closureDate,
                    s.priorityTitle,
                    s.evidenceAvailableId,
                    s.evidenceAvailable,
                    s.evidenceAvailableScore,
                    s.isCompleted,
                    s.reviewerComment,
                    s.clusterReviewed,
                    s.implementedDate,
                    s.rework,
                    s.dayStatusId,
                    s.dayStatusTitle,
                    s.dayStatusScore,
                    s.calcStatus,
                    s.calcEvid,
                    s.calcDate,
                    s.completionScore,
                    daysToTarget = DaysToTargetCalculation.CalculateMonths(s.targetDate, (int?)s.statusId, (decimal?)s.calcDate, (decimal?)s.calcStatus, (decimal?)s.calcEvid),
                    s.scoreDetails,
                    s.siteId,
                    s.siteTitle,
                    s.regionId,
                    s.regionTitle,
                    s.sourceId,
                    s.sourceTitle,
                    s.clusterId,
                    s.clusterTitle,
                    //s.reportAttahced,
                    //s.reportName,
                    s.actionClosedBy,
                    s.actionClosedTitle,
                }).ToList();

                var obj = new
                {
                    tracker = daysToTargetList.Where(a => (DaysIds.Count == 0) || DaysIds.Contains((string)a.daysToTarget)).ToList()
                };
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getInsurenceActionTrackerReport")]
        public async Task<IActionResult> getInsurenceTrackerReport(InsuranceTrackerUserFilterList reg)
        {
            try
            {
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());
                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());
                List<int> ClusterIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.clusterList))
                    ClusterIds = (reg.clusterList.Split(',').Select(Int32.Parse).ToList());
                List<int> SourceIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sourceList))
                    SourceIds = (reg.sourceList.Split(',').Select(Int32.Parse).ToList());

                List<int> CompanyIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.companyList))
                    CompanyIds = (reg.companyList.Split(',').Select(Int32.Parse).ToList());


                List<int> StatusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.statusList))
                    StatusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                List<string> DaysIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.dayList))
                    DaysIds = (reg.dayList.Split(',').ToList());
                var nowDate = DateTime.Now;

                var tracker = await (from iat in _context.InsurenceActionTracker.Where(a => a.isDeleted == 0 &&
                                  ((SourceIds.Count == 0) || SourceIds.Contains((int)a.sourceId)) &&
                                  ((CompanyIds.Count == 0) || CompanyIds.Contains((int)a.iatCompanyId)) &&
                                  ((StatusIds.Count == 0) || StatusIds.Contains((int)a.iatStatusId)))
                                     join au in _context.AppUser.Where(a => a.isDeleted == 0) on iat.assignedTo equals au.userId into alluser
                                     from auu in alluser.DefaultIfEmpty()
                                     join ir in _context.InsuranceRecommendations.Where(sd => sd.isDeleted == 0 && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)sd.priorityId))) on iat.recommendationId equals ir.irId
                                     join ip in _context.InsuranceRecPriority on ir.priorityId equals ip.ipId
                                     join ns in _context.InsuranceRecNomacStatus on ir.nomacStatusId equals ns.nomacStatusId
                                     join insurStatus in _context.InsuranceRecInsurenceStatus on ir.insuranceStatusId equals insurStatus.insurenceStatusId
                                     join d in _context.InsuranceRecDocumentType on ir.documentTypeId equals d.documentId into allthree
                                     from bccc in allthree.DefaultIfEmpty()

                                     join s in _context.Sites.Where(a => (SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)) on ir.siteId equals s.siteId
                                     join cn in _context.Country on s.countryId equals cn.countryId
                                     join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0) on s.siteId equals ts.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                     join st in _context.IATrackingStatus on iat.iatStatusId equals st.statusId into aa
                                     from bb in aa.DefaultIfEmpty()
                                     join ev in _context.IATrackingEvidence on iat.evidenceAvailableId equals ev.iatEvidenceId into all3
                                     from evd in all3.DefaultIfEmpty()
                                     join c in _context.IATrackingCompany on iat.iatCompanyId equals c.iatId into aaa
                                     from bbb in aaa.DefaultIfEmpty()
                                    
                                     join so in _context.IATrackingSource on iat.sourceId equals so.sourceId into aaaa
                                     from bbbb in aaaa.DefaultIfEmpty()

                                     join rege in _context.Regions2.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on s.region2 equals rege.regionId
                                     
                                     join ds in _context.IATrackingDaysStatus on iat.dayStatus equals ds.iatDayStatusId into all5
                                     from dsd in all5.DefaultIfEmpty()
                                     join icc in _context.Cluster.Where(a => a.isDeleted == 0 && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))) on s.clusterId equals icc.clusterId
                                     join uxx in _context.AppUser on iat.actionClosedBy equals uxx.userId into all90
                                     from uxxx in all90.DefaultIfEmpty()
                                     select new
                                     {
                                         assignedToId = iat.assignedTo == null ? -1 : iat.assignedTo,
                                         assignedToTitle = auu.firstName + " " + auu.lastName,
                                         insurenceActionTrackerId = iat.iaId,
                                         recommendationId = iat.recommendationId,
                                         recommendationTitle = ir.title,
                                         recommendationReference = ir.referenceNumber + '-' + ir.title,
                                         iat.action,
                                         insurenceStatusId = insurStatus.insurenceStatusId,
                                         insurenceStatusTitle = insurStatus.insurenceStatusTitle,
                                         nomacStatusId = ns.nomacStatusId,
                                         nomacStatusTitle = ns.nomacStatusTitle,
                                         iat.targetDate,
                                         iat.adminComment,
                                         statusId = iat.iatStatusId,
                                         statusTitle = bb.statusTitle,
                                         statusScore = bb.score,
                                         companyId = iat.iatCompanyId,
                                         companyTitle = bbb.iatTitle,
                                         iat.comments,
                                         iat.closureDate,
                                         ir.priorityId,
                                         priorityTitle = ip.ipTitle,
                                         evidenceAvailableId = iat.evidenceAvailableId == null ? 2 : iat.evidenceAvailableId,
                                         evidenceAvailable = evd.aitEvidenceTitle == null ? "No" : evd.aitEvidenceTitle,
                                         evidenceAvailableScore = evd.score,
                                         dayStatusId = iat.dayStatus,
                                         dayStatusTitle = dsd.iatDayStatus,
                                         dayStatusScore = dsd.score,
                                         iat.calcStatus,
                                         iat.calcEvid,
                                         ir.type,
                                         documentTypeId = bccc.documentId,
                                         documentTypeTitle = bccc.documnetTitle,
                                         iat.calcDate,
                                         iat.completionScore,
                                         iat.daysToTarget,
                                         iat.scoreDetails,
                                         ir.siteId,
                                         siteTitle = s.siteName,
                                         regionId = s.regionId,
                                         regionTitle = rege.title,
                                         iat.sourceId,
                                         sourceTitle = bbbb.sourceTitle,
                                         icc.clusterId,
                                         icc.clusterTitle,
                                         isCompleted = iat.isCompleted == 1 ? true : false,
                                         iat.reviewerComment,
                                         clusterReviewed = iat.clusterReviewed == 1 ? true : false,
                                         rework = iat.rework == 1 ? true : false,
                                         iat.implementedDate,
                                         //reportAttahced = iatrr.iatFileId == null ? false : true,
                                         //reportName = iatrr.fileName,
                                         iat.actionClosedBy,
                                         actionClosedTitle = uxxx.firstName + " " + uxxx.lastName,
                                     }
                 ).Distinct().OrderByDescending(a => a.insurenceActionTrackerId).ToListAsync();
            
                var daysToTargetList = tracker.Select(s => new
                {
                    s.documentTypeTitle,
                    s.type,
                    s.assignedToId,
                    s.assignedToTitle,
                    s.insurenceActionTrackerId,
                    s.recommendationId,
                    s.recommendationTitle,
                    s.recommendationReference,
                    s.action,
                    s.targetDate,
                    s.clusterId,
                    s.clusterTitle,
                    s.statusId,
                    s.statusTitle,
                    s.statusScore,
                    s.insurenceStatusId,
                    s.insurenceStatusTitle,
                    s.nomacStatusId,
                    s.nomacStatusTitle,
                    s.companyId,
                    s.companyTitle,
                    s.comments,
                    s.closureDate,
                    s.priorityTitle,
                    s.isCompleted,
                    s.reviewerComment,
                    s.clusterReviewed,
                    s.implementedDate,
                    s.rework,
                    s.evidenceAvailableId,
                    s.evidenceAvailable,
                    s.evidenceAvailableScore,
                    s.dayStatusId,
                    s.dayStatusTitle,
                    s.dayStatusScore,
                    s.calcStatus,
                    s.calcEvid,
                    s.calcDate,
                    s.completionScore,
                    daysToTarget = DaysToTargetCalculation.CalculateMonths(s.targetDate, (int?)s.statusId, (decimal?)s.calcDate, (decimal?)s.calcStatus, (decimal?)s.calcEvid),
                    s.scoreDetails,
                    s.siteId,
                    s.siteTitle,
                    s.regionId,
                    s.regionTitle,
                    s.adminComment,
                    s.sourceId,
                    s.sourceTitle,
                    s.actionClosedBy,
                    s.actionClosedTitle
                }).ToList();

                var obj = new
                {
                    tracker = daysToTargetList.Where(a => (DaysIds.Count == 0) || DaysIds.Contains((string)a.daysToTarget)).ToList()
                };
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("endUserStausInsurenceActionTracker")]
        public async Task<IActionResult> StatusTracker(AssignInsurenceStatusDto reg)
        {
            try
            {
                InsurenceActionTracker ins = await (from i in _context.InsurenceActionTracker.Where(a => a.iaId == reg.insurenceAction.insurenceActionTrackerId)
                                                    select i).FirstOrDefaultAsync();
                if (ins != null)
                {
                    ins.adminComment = reg.insurenceAction.adminComment;
                    ins.isCompleted = 0;
                    ins.rework = 0;
                    ins.clusterReviewed = 0;
                    ins.comments = reg.insurenceAction.comments;
                    ins.iatStatusId = reg.insurenceAction.statusId;
                  
                    if(ins.iatStatusId == 1)
                    {
                        ins.closureDate = DateTime.Now;
                        ins.actionClosedBy = reg.userId;
                    }
                    else if (ins.iatStatusId == 4)
                    {
                        ins.implementedDate = DateTime.Now;
                        ins.actionClosedBy = null;

                    }
                    else
                    {
                        ins.closureDate = null;
                        ins.actionClosedBy = null;

                    }
                    ins.evidenceAvailableId = reg.insurenceAction?.evidenceAvailableId;
                    ins.calcEvid = reg.insurenceAction.evidenceAvailableScore;
                    ins.calcStatus = reg.insurenceAction.statusScore;
                    if (reg.insurenceAction.assignedToId != -1 || reg.insurenceAction.assignedToId != null)
                    {
                        ins.assignedTo = reg.insurenceAction.assignedToId;

                    }
                    if (ins.targetDate > ins.closureDate)
                    {
                        ins.calcDate = 1;
                    }
                    else
                    {
                        ins.calcDate = 0;
                    }
                    var totalCalc = (ins.calcEvid + ins.calcStatus) / 2 * 100;
                    ins.completionScore = totalCalc.ToString() + "%";
                    ins.scoreDetails = "Evidence Score = " + " " + ins.calcEvid + ";" + "Status Score = " + " " + reg.insurenceAction.statusScore + ";" + "Total Score = " + " " + ins.completionScore;
                    _context.SaveChanges();

                    var statusChecker = (from a in _context.InsurenceActionTracker.Where(a => a.recommendationId == ins.recommendationId && a.isDeleted == 0)
                                         select a).ToList();
                    InsuranceRecommendations rec = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == ins.recommendationId)
                                                          select r).FirstOrDefaultAsync();
                    if (rec.insuranceStatusId == 2 || rec.insuranceStatusId == 3)
                    {
                        decimal? avgTotal = 0;

                        if (statusChecker.Count > 0)
                        {
                            decimal? calTotal = 0;
                            foreach (var chk in statusChecker)
                            {
                                calTotal += (((chk.calcEvid + chk.calcStatus) / 2) * 100);
                            }
                            avgTotal = calTotal / statusChecker.Count;

                        }
                        if (avgTotal == 100)
                        {
                            rec.nomacStatusId = 4;
                        }
                        else if (avgTotal >= 30)
                        {
                            rec.nomacStatusId = 3;
                        }
                        else
                        {
                            if (rec.insuranceStatusId == 1)
                            {
                                rec.nomacStatusId = 1;
                            }
                            else if (rec.insuranceStatusId == 4)
                            {
                                rec.nomacStatusId = 5;
                            }
                            else if (rec.insuranceStatusId == 3)
                            {
                                rec.nomacStatusId = 2;
                            }
                            else if (rec.insuranceStatusId == 5)
                            {
                                rec.nomacStatusId = 6;
                            }
                            else if (rec.insuranceStatusId == 2)
                            {
                                rec.nomacStatusId = 3;
                            }
                        }
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.insurenceAction);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("reviewerInsurenceActionTracker")]
        public async Task<IActionResult> StatusTrackerReviewer(AssignInsurenceStatusDto reg)
        {
            try
            {
                InsurenceActionTracker ins = await (from i in _context.InsurenceActionTracker.Where(a => a.iaId == reg.insurenceAction.insurenceActionTrackerId)
                                                    select i).FirstOrDefaultAsync();
                if (ins != null)
                {

                    ins.reviewerComment = reg.insurenceAction.reviewerComment;
                    ins.isCompleted = (int)(reg.insurenceAction.isCompleted == false ? 0 : 1);
                    ins.rework = (int)(reg.insurenceAction.rework == false ? 0 : 1);
                    ins.clusterReviewed = (int)(reg.insurenceAction.clusterReviewed == false ? 0 : 1);
                    ins.comments = reg.insurenceAction.comments;
                    ins.iatStatusId = reg.insurenceAction.statusId;
                    if (ins.isCompleted == 1)
                    {
                        ins.iatStatusId = 1;
                    }
                    if (ins.rework == 1)
                    {
                        ins.iatStatusId = 2;
                    }
                    if (ins.iatStatusId == 1)
                    {
                        ins.closureDate = DateTime.Now;
                        ins.actionClosedBy = reg.userId;
                    }
                    else if(ins.iatStatusId == 4)
                    {
                        ins.implementedDate = DateTime.Now;
                        ins.actionClosedBy = null;
                    }
                    else
                    {
                        ins.actionClosedBy = null;
                        ins.closureDate = null;
                    }
                    ins.evidenceAvailableId = reg.insurenceAction?.evidenceAvailableId;
                    ins.calcEvid = reg.insurenceAction.evidenceAvailableScore;
                    ins.calcStatus = reg.insurenceAction.statusScore;
                    if (reg.insurenceAction.assignedToId != -1 || reg.insurenceAction.assignedToId != null)
                    {
                        ins.assignedTo = reg.insurenceAction.assignedToId;

                    }
                    if (ins.targetDate > ins.closureDate)
                    {
                        ins.calcDate = 1;
                    }
                    else
                    {
                        ins.calcDate = 0;
                    }
                    var totalCalc = (ins.calcEvid + ins.calcStatus) / 2 * 100;
                    ins.completionScore = totalCalc.ToString() + "%";
                    ins.scoreDetails = "Evidence Score = " + " " + ins.calcEvid + ";" + "Status Score = " + " " + reg.insurenceAction.statusScore + ";" + "Total Score = " + " " + ins.completionScore;
                    _context.SaveChanges();

                    var statusChecker = (from a in _context.InsurenceActionTracker.Where(a => a.recommendationId == ins.recommendationId && a.isDeleted == 0)
                                         select a).ToList();
                    InsuranceRecommendations rec = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == ins.recommendationId)
                                                          select r).FirstOrDefaultAsync();
                    if (rec.insuranceStatusId == 2 || rec.insuranceStatusId == 3)
                    {
                        decimal? avgTotal = 0;

                        if (statusChecker.Count > 0)
                        {
                            decimal? calTotal = 0;
                            foreach (var chk in statusChecker)
                            {
                                calTotal += (((chk.calcEvid + chk.calcStatus) / 2) * 100);
                            }
                            avgTotal = calTotal / statusChecker.Count;

                        }
                        if (avgTotal == 100)
                        {
                            rec.nomacStatusId = 4;
                        }
                        else if (avgTotal >= 30)
                        {
                            rec.nomacStatusId = 3;
                        }
                        else
                        {
                            if (rec.insuranceStatusId == 1)
                            {
                                rec.nomacStatusId = 1;
                            }
                            else if (rec.insuranceStatusId == 4)
                            {
                                rec.nomacStatusId = 5;
                            }
                            else if (rec.insuranceStatusId == 3)
                            {
                                rec.nomacStatusId = 2;
                            }
                            else if (rec.insuranceStatusId == 5)
                            {
                                rec.nomacStatusId = 6;
                            }
                            else if (rec.insuranceStatusId == 2)
                            {
                                rec.nomacStatusId = 3;
                            }
                        }
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.insurenceAction);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public List<int> QuaterCalc(string quarters)
        {
            List<int> MonthIds = new List<int>();
            List<int[]> quarterMonths = new List<int[]>
    {
        new int[] { 1, 2, 3 },
        new int[] { 4, 5, 6 },
        new int[] { 7, 8, 9 },
        new int[] { 10, 11, 12 }
    };

            if (!string.IsNullOrEmpty(quarters))
            {
                List<int> QuarterIds = quarters.Split(',').Select(int.Parse).ToList();
                foreach (var quarter in QuarterIds)
                {
                    if (quarter >= 1 && quarter <= 4)
                    {
                        MonthIds.AddRange(quarterMonths[quarter - 1]);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid quarter: {quarter}");
                    }
                }
            }

            return MonthIds;
        }
    }
}
