using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ActionTrakingSystem.DTOs;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Net;

namespace ActionTrakingSystem.Controllers
{
    public class InsurenceRecommendationController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InsurenceRecommendationController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }
        [Authorize]
        [HttpPost("downloadReport/{irId}")]
        public async Task<IActionResult> DownloadTilFile(int irId)
        {
            try
            {
                var fileIR = await (from a in _context.InsuranceRecFile.Where(a => a.irId == irId && a.isDeleted == 0)
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
        public async Task<IActionResult> uploadFiles([FromForm] uploadIRFilesDto reg)
        {
            try
            {
               
                if (reg.recommendationReport != null)
                {
                    string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.recommendationReport.FileName);
                    string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", tilFileName);
                    using (var stream = new FileStream(tilfilePath, FileMode.Create))
                    {
                        await reg.recommendationReport.CopyToAsync(stream);
                    }
                    string tilFileUrl = $"~/uploads/{tilFileName}";


                    _context.Database.ExecuteSqlCommand("DELETE FROM InsuranceRecFile WHERE irId = @tilId", new SqlParameter("tilId", Convert.ToInt32(reg.irId)));

                    InsuranceRecFile fileIR = new InsuranceRecFile();
                        fileIR.path = tilFileUrl;
                        fileIR.irId = Convert.ToInt32(reg.irId);
                        fileIR.fileName = reg.recommendationReport.FileName;
                        fileIR.createdBy = Convert.ToInt32(reg.userId);
                        fileIR.isDeleted = 0;
                        _context.Add(fileIR);
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
        [HttpPost("getInsurenceRecommendation")]
        public async Task<IActionResult> GetInsuranceRecommendations(IRApiDataList reg)
        {
            try
            {
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> ClusterIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.clusterList))
                    ClusterIds = (reg.clusterList.Split(',').Select(Int32.Parse).ToList());

                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

                List<int> ProactivesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.proactiveList))
                    ProactivesIds = (reg.proactiveList.Split(',').Select(Int32.Parse).ToList());

                List<int> SourceIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sourceList))
                    SourceIds = (reg.sourceList.Split(',').Select(Int32.Parse).ToList());

                List<int> NomacIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.nomacStatus))
                    NomacIds = (reg.nomacStatus.Split(',').Select(Int32.Parse).ToList());

                List<int> InsuranceIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.insuranceStatus))
                    InsuranceIds = (reg.insuranceStatus.Split(',').Select(Int32.Parse).ToList());

                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());
                List<string> YearIds = new List<string>();
                if (!string.IsNullOrEmpty(reg.yearList))
                    YearIds = (reg.yearList.Split(',').ToList());

                //var reommendations = await (from ir in _context.InsuranceRecommendations
                //                            .Where(
                //                            a => a.isDeleted == 0 
                //                            && ((SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)) && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)a.priorityId)) && ((YearIds.Count == 0) || YearIds.Contains(a.year))
                //                            && ((SourceIds.Count == 0) || SourceIds.Contains((int)a.sourceId)) && ((NomacIds.Count == 0) || NomacIds.Contains((int)a.nomacStatusId)) && ((InsuranceIds.Count == 0) || InsuranceIds.Contains((int)a.insuranceStatusId))
                //                            )
                //                            join s in _context.Sites on ir.siteId equals s.siteId
                //                            join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                //                            join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                //                            join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                //                            join r in _context.Regions2.Where(a=>a.isDeleted == 0 && ((RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId))) on s.region2 equals r.regionId
                //                            join p in _context.ProactiveRiskPrevention on ir.proactiveId equals p.proactiveId into allIns
                //                            from ains in allIns.DefaultIfEmpty()
                //                            join t in _context.InsuranceRecType on ir.recommendationTypeId equals t.typeId into allsec
                //                            from aa in allsec.DefaultIfEmpty()
                //                            join i in _context.InsuranceRecSource on ir.sourceId equals i.sourceId into allxc
                //                            from ii in allxc.DefaultIfEmpty()
                //                            join d in _context.InsuranceRecDocumentType on ir.documentTypeId equals d.documentId into allthree
                //                            from bb in allthree.DefaultIfEmpty()
                //                            join rs in _context.InsuranceRecFile on ir.irId equals rs.irId into all4
                //                            from rss in all4.DefaultIfEmpty()
                //                            join ns in _context.InsuranceRecNomacStatus on ir.nomacStatusId equals ns.nomacStatusId into allghf
                //                            from nss in allghf.DefaultIfEmpty()
                //                            join insurStatus in _context.InsuranceRecInsurenceStatus on ir.insuranceStatusId equals insurStatus.insurenceStatusId into alldes
                //                            from insurStatuss in alldes.DefaultIfEmpty()
                //                            join ip in _context.InsuranceRecPriority on ir.priorityId equals ip.ipId into allgty
                //                            from ipp in allgty.DefaultIfEmpty()
                //                            join icc in _context.Cluster.Where(a => a.isDeleted == 0 && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))) on s.clusterId equals icc.clusterId 
                //                            select new
                //                            {
                //                                recommendationId = ir.irId,
                //                                ir.title,
                //                                ir.insuranceRecommendation,
                //                                ir.referenceNumber,
                //                                priorityId = ipp.ipId,
                //                                priorityTitle = ipp.ipTitle,
                //                                insurenceStatusId = insurStatuss.insurenceStatusId,
                //                                insurenceStatusTitle = insurStatuss.insurenceStatusTitle,
                //                                nomacStatusId = nss.nomacStatusId,
                //                                nomacStatusTitle = nss.nomacStatusTitle,
                //                                ir.latestStatus,
                //                                ir.targetDate,
                //                                ir.siteUpdates,
                //                                ir.pcComments,
                //                                ir.type,
                //                                ir.expectedBudget,
                //                                siteTitle = s.siteName,
                //                                siteId = ir.siteId,
                //                                ir.significance,
                //                                proactiveReference = ains.proactiveReference,
                //                                proactiveId = ains.proactiveId==null?-1: ains.proactiveId,
                //                                proactiveTitle = ains.proactivetitle,
                //                                regionTitle = r.title,
                //                                regionId = r.regionId,
                //                                sourceId = ii.sourceId,
                //                                sourceTitle = ii.sourceTitle,
                //                                documentTypeId = bb.documentId,
                //                                documentTypeTitle = bb.documnetTitle,
                //                                ir.year,
                //                                recommendationTypeId = aa.typeId,
                //                                recommendationTypeTitle = aa.typeTitle,
                //                                report = ir.report,
                //                                reportAttahced = rss.faId == null?false:true,
                //                                reportName = rss.fileName,
                //                                icc.clusterId,
                //                                icc.clusterTitle,
                //                            }).Distinct().OrderByDescending(z => z.recommendationId).ToListAsync();
                var recommendations = await _context.InsuranceRecommendatioSpDto
     .FromSqlInterpolated($"exec GetRecommendationData @userId = {reg.userId}")
     .ToListAsync();

                var data = new
                {
                    reommendations = recommendations.Where(a => (ProactivesIds.Count == 0 || ProactivesIds.Contains((int)a.proactiveId))
                    && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))
                    && ((SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId))
                    && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)a.priorityId))
                    && ((YearIds.Count == 0) || YearIds.Contains(a.year))
                    && ((SourceIds.Count == 0) || SourceIds.Contains((int)a.sourceId))
                    && ((NomacIds.Count == 0) || NomacIds.Contains((int)a.nomacStatusId))
                    && ((InsuranceIds.Count == 0) || InsuranceIds.Contains((int)a.insurenceStatusId))
                    && ((RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId))).ToList()
                   
                };
                return Ok(data);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> getInterfaces(UserIdDto reg)
        {
            try
            {
                var documentType = await (from d in _context.InsuranceRecDocumentType.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              d.documentId,
                                              d.documnetTitle
                                          }).ToListAsync();
                var proactive = await (from p in _context.ProactiveRiskPrevention
                                       select new
                                       {
                                           p.proactiveId,
                                           p.proactivetitle,
                                           p.proactiveReference
                                       }).ToListAsync();

                var source = await (from sr in _context.InsuranceRecSource.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        sr.sourceId,
                                        sr.sourceTitle
                                    }).ToListAsync();
                var recommendationType = await (from sr in _context.InsuranceRecType.Where(a => a.isDeleted == 0)
                                                select new
                                                {
                                                    sr.typeId,
                                                    sr.typeTitle
                                                }).ToListAsync();
                var nomacStatus = await (from sr in _context.InsuranceRecNomacStatus.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             sr.nomacStatusId,
                                             sr.nomacStatusTitle
                                         }).ToListAsync();
                var insurenceStatus = await (from sr in _context.InsuranceRecInsurenceStatus.Where(a => a.isDeleted == 0)
                                             select new
                                             {
                                                 sr.insurenceStatusId,
                                                 sr.insurenceStatusTitle
                                             }).ToListAsync();
                var priority = await (from ip in _context.InsuranceRecPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          priorityId = ip.ipId,
                                          priorityTitle = ip.ipTitle
                                      }).ToListAsync();
                var obj = new
                {
                    documentType,
                    proactive,
                    source,
                    recommendationType,
                    nomacStatus,
                    insurenceStatus,
                    priority,
                };
                return Ok(obj);
            }
            catch(Exception E)
            {
                return BadRequest(E.Message);
            }
        }
        [Authorize]
        [HttpPost("saveInsuranceRecommendation")]
        public async Task<IActionResult> SaveInsurenceRecommendation(AddInsuranceDto recommend)
        {
            try
            {
                if (recommend.recommendation.recommendationId == -1)
                {
                    InsuranceRecommendations rec = new InsuranceRecommendations();
                    rec.referenceNumber = recommend.recommendation.referenceNumber;
                    rec.report = recommend.recommendation.report;
                    rec.title = recommend.recommendation.title;
                    rec.significance = recommend.recommendation.significance;
                    rec.type = recommend.recommendation.type;
                    rec.priorityId = recommend.recommendation.priorityId;
                    rec.insuranceRecommendation = recommend.recommendation.insuranceRecommendation;
                    rec.siteUpdates = recommend.recommendation.siteUpdates;
                    rec.pcComments = recommend.recommendation.pcComments;
                    rec.insuranceStatusId = recommend.recommendation.insurenceStatusId;

                    //rec.nomacStatusId = recommend.recommendation.nomacStatusId;
                    if (rec.insuranceStatusId == 1)
                    {
                        rec.nomacStatusId = 1;
                    }
                    else if (rec.insuranceStatusId == 4)
                    {
                        rec.nomacStatusId = 5;
                    }
                    else if(rec.insuranceStatusId == 3)
                    {
                        rec.nomacStatusId = 2;
                    }
                    else if(rec.insuranceStatusId == 5)
                    {
                        rec.nomacStatusId = 6;
                    }
                    else if (rec.insuranceStatusId == 2)
                    {
                        rec.nomacStatusId = 3;
                    }
                    rec.latestStatus = recommend.recommendation.latestStatus;
                    rec.targetDate = recommend.recommendation.targetDate;
                    rec.expectedBudget = recommend.recommendation.expectedBudget;
                    rec.proactiveId = recommend.recommendation.proactiveId;
                    rec.siteId = recommend.recommendation.siteId;
                    rec.sourceId = recommend.recommendation.sourceId;
                    rec.documentTypeId = recommend.recommendation.documentTypeId;
                    rec.year = recommend.recommendation.year;
                    rec.recommendationTypeId = recommend.recommendation.recommendationTypeId;
                    rec.createdDate = DateTime.Now;
                    rec.createdBy = recommend.userId;

                    rec.isDeleted = 0;
                    _context.Add(rec);
                    _context.SaveChanges();
                    recommend.recommendation.recommendationId = rec.irId;
                    return Ok(recommend.recommendation);
                }
                else
                {
                    InsuranceRecommendations rec = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == recommend.recommendation.recommendationId)
                                                          select r).FirstOrDefaultAsync();
                    rec.referenceNumber = recommend.recommendation.referenceNumber;
                    rec.report = recommend.recommendation.report;
                    rec.title = recommend.recommendation.title;
                    rec.significance = recommend.recommendation.significance;
                    rec.type = recommend.recommendation.type;
                    rec.priorityId = recommend.recommendation.priorityId;
                    rec.insuranceRecommendation = recommend.recommendation.insuranceRecommendation;
                    rec.siteUpdates = recommend.recommendation.siteUpdates;
                    rec.pcComments = recommend.recommendation.pcComments;
                    rec.insuranceStatusId = recommend.recommendation.insurenceStatusId;
                    if (rec.insuranceStatusId == 1)
                    {
                        rec.nomacStatusId = 1;
                    }
                    else if (rec.insuranceStatusId == 4)
                    {
                        rec.nomacStatusId = 5;
                    }

                    else if (rec.insuranceStatusId == 5)
                    {
                        rec.nomacStatusId = 6;
                    }
                    else if (rec.insuranceStatusId == 2 || rec.insuranceStatusId == 3)
                    {
                        var statusChecker = (from a in _context.InsurenceActionTracker.Where(a => a.recommendationId == recommend.recommendation.recommendationId && a.isDeleted == 0)
                                             select a).ToList();
                        decimal? avgTotal = 0;
                        if (statusChecker.Count > 0)
                        {
                            decimal? calTotal = 0;
                            foreach(var chk in statusChecker)
                            {
                                calTotal += (((chk.calcEvid + chk.calcStatus) / 2)*100);
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
                    }
                    rec.latestStatus = recommend.recommendation.latestStatus;
                    rec.targetDate = recommend.recommendation.targetDate;
                    rec.expectedBudget = recommend.recommendation.expectedBudget;
                    rec.proactiveId = recommend.recommendation.proactiveId;
                    rec.siteId = recommend.recommendation.siteId;
                    rec.sourceId = recommend.recommendation.sourceId;
                    rec.documentTypeId = recommend.recommendation.documentTypeId;
                    rec.year = recommend.recommendation.year;
             
                    rec.recommendationTypeId = recommend.recommendation.recommendationTypeId;
                    rec.modifiedDate = DateTime.Now;
                    rec.modifiedBy = recommend.userId;
                    _context.SaveChanges();
                    return Ok(recommend.recommendation);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteInsurenceRecommendation")]
        public async Task<IActionResult> DeleteInsurenceRecommendation(InsuranceRecommendationDto reg)
        {
            try
            {
                InsuranceRecommendations regions = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == reg.recommendationId)
                                                          select r).FirstOrDefaultAsync();
                regions.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(reg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
