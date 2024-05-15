using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ActionTrakingSystem.Controllers
{

    public class InsurenceActionTrackerController : BaseAPIController
    {
        private readonly DAL _context;
        public InsurenceActionTrackerController(DAL context)
        {
            _context = context;
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
                                                recommendationReference = rec.referenceNumber + " - " + rec.title,

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
                                           iatst.statusTitle
                                       }).ToListAsync();
                var priority = await (from ip in _context.InsuranceRecPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          priorityId = ip.ipId,
                                          priorityTitle = ip.ipTitle
                                      }).ToListAsync();

                var obj = new
                {
                    recommendation,
                    iatSource,
                    iatCompany,
                    iatStatus,
                    priority,
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
        public async Task<IActionResult> getInsurenceTracker(AssignInsurenceUserFilter reg)
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


                List<int> iatCompanyId = new List<int>();
                if (!string.IsNullOrEmpty(reg.department))
                    iatCompanyId = (reg.department.Split(',').Select(Int32.Parse).ToList());

                List<int> SourceIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.sourceList))
                    SourceIds = (reg.sourceList.Split(',').Select(Int32.Parse).ToList());


                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.priorityList))
                    PriorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());

                var tracker = await (from iat in _context.InsurenceActionTracker.Where(a => a.isDeleted == 0 && ((SourceIds.Count == 0) || SourceIds.Contains((int)a.sourceId)) && ((iatCompanyId.Count == 0) || iatCompanyId.Contains((int)a.iatCompanyId)))
                                         //from iat in _context.InsurenceActionTracker.Where(a => a.isDeleted == 0 && (a.createdOn >= reg.filter.startData || reg.filter.startData == null) && (a.createdOn <= reg.filter.endDate || reg.filter.endDate == null) && (a.sourceId == reg.filter.source || reg.filter.source == -1) && (a.iatCompanyId == reg.filter.department || reg.filter.department == -1))
                                     join ir in _context.InsuranceRecommendations.Where(sd => sd.isDeleted == 0 && ((SitesIds.Count == 0) || SitesIds.Contains((int)sd.siteId)) && ((PriorityIds.Count == 0) || PriorityIds.Contains((int)sd.priorityId))) on iat.recommendationId equals ir.irId
                                     join ip in _context.InsuranceRecPriority on ir.priorityId equals ip.ipId into allgty
                                     from ipp in allgty.DefaultIfEmpty()
                                     join s in _context.Sites on ir.siteId equals s.siteId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join st in _context.IATrackingStatus on iat.iatStatusId equals st.statusId into aa
                                     from bb in aa.DefaultIfEmpty()
                                     join iatr in _context.IATrackingFile on iat.iaId equals iatr.iatId into alll
                                     from iatrr in alll.DefaultIfEmpty()
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
                                     select new
                                     {
                                         insurenceActionTrackerId = iat.iaId,
                                         recommendationId = iat.recommendationId,
                                         recommendationTitle = ir.title,
                                         recommendationReference = ir.referenceNumber + '-' + ir.title,
                                         iat.action,
                                         iat.targetDate,
                                         ir.priorityId,
                                         priorityTitle = ipp.ipTitle,
                                         statusId = iat.iatStatusId,
                                         statusTitle = bb.statusTitle,
                                         statusScore = bb.score,
                                         companyId = iat.iatCompanyId,
                                         companyTitle = bbb.iatTitle,
                                         iat.comments,
                                         iat.closureDate,
                                         evidenceAvailableId = iat.evidenceAvailableId,
                                         evidenceAvailable = evd.aitEvidenceTitle,
                                         evidenceAvailableScore = evd.score,
                                         dayStatusId = iat.dayStatus,
                                         dayStatusTitle = dsd.iatDayStatus,
                                         dayStatusScore = dsd.score,
                                         iat.calcStatus,
                                         iat.calcEvid,
                                         iat.calcDate,
                                         iat.completionScore,
                                         iat.daysToTarget,
                                         iat.scoreDetails,
                                         siteId = ir.siteId,
                                         siteTitle = s.siteName,
                                         regionId = s.regionId,
                                         regionTitle = rege.title,
                                         iat.sourceId,
                                         sourceTitle = bbbb.sourceTitle,
                                         reportAttahced = iatrr.iatFileId == null ? false : true,
                                         reportName = iatrr.fileName,
                                         icc.clusterId,
                                         icc.clusterTitle,
                                     }
                    ).Distinct().OrderByDescending(a => a.insurenceActionTrackerId).ToListAsync();
                var obj = new
                {
                    tracker,
                };

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPost("saveInsurenceActionTracker")]
        public async Task<IActionResult> SaveInsurenceTracker(AssignInsurenceTrackerDto reg)
        {
            try
            {
                if (reg.insurenceAction.insurenceActionTrackerId != -1)
                {
                    InsurenceActionTracker ins = await (from i in _context.InsurenceActionTracker.Where(a => a.iaId == reg.insurenceAction.insurenceActionTrackerId)
                                                        select i).FirstOrDefaultAsync();
                    if (ins != null)
                    {
                        ins.action = reg.insurenceAction.action;
                        ins.targetDate = Convert.ToDateTime(reg.insurenceAction.targetDate);
                        ins.iatCompanyId = reg.insurenceAction.companyId;
                        ins.sourceId = reg.insurenceAction.sourceId;
                        ins.modifiedOn = DateTime.Now;
                        ins.modifiedBy = reg.userId;
                        _context.SaveChanges();

                        var statusChecker = (from a in _context.InsurenceActionTracker.Where(a => a.recommendationId == ins.recommendationId && a.isDeleted == 0 )
                                             select a).OrderByDescending(a => a.targetDate).ToList();
                        InsuranceRecommendations rec = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == ins.recommendationId)
                                                              select r).FirstOrDefaultAsync();
                        rec.targetDate = statusChecker.FirstOrDefault().targetDate;
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
                            else if (avgTotal >= 50)
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
                        _context.SaveChanges();


                    }

                    return Ok(reg.insurenceAction);
                }
                else
                {
                    InsurenceActionTracker ins = new InsurenceActionTracker();
                    ins.recommendationId = reg.insurenceAction.recommendationId;
                    ins.action = reg.insurenceAction.action;
                    ins.targetDate = Convert.ToDateTime(reg.insurenceAction.targetDate);
                    ins.iatCompanyId = reg.insurenceAction.companyId;
                    ins.sourceId = reg.insurenceAction.sourceId;
                    ins.iatStatusId = 2;
                    ins.createdOn = DateTime.Now;
                    ins.createdBy = reg.userId;
                    _context.Add(ins);
                    _context.SaveChanges();
                    reg.insurenceAction.insurenceActionTrackerId = ins.iaId;

                    var statusChecker = (from a in _context.InsurenceActionTracker.Where(a => a.recommendationId == ins.recommendationId && a.isDeleted == 0 )
                                         select a).OrderByDescending(a => a.targetDate).ToList();
                    InsuranceRecommendations rec = await (from r in _context.InsuranceRecommendations.Where(a => a.irId == ins.recommendationId)
                                                          select r).FirstOrDefaultAsync();
                    rec.targetDate = statusChecker.FirstOrDefault().targetDate;

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
                    }
                    _context.SaveChanges();

                    return Ok(reg.insurenceAction);
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("filterRecom")]
        public async Task<IActionResult> GetSites(FilterEqUserDto reg)
        {
            try
            {
                var recommendation = await (from rec in _context.InsuranceRecommendations.Where(a => a.isDeleted == 0)
                                            join s in _context.Sites.Where(a => a.siteId == reg.siteId) on rec.siteId equals s.siteId
                                            join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                            join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                            select new
                                            {
                                                recommendationId = rec.irId,
                                                recommendationTitle = rec.title,
                                                recommendationReference = rec.referenceNumber + " - " + rec.title,
                                            }).Distinct().ToListAsync();
                return Ok(recommendation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("delUsersInsurenceActionTracker")]
        public async Task<IActionResult> DeleteInsurenceTracker(AssignInsurenceDto reg)
        {
            try
            {
                var ins = await (from ap in _context.InsurenceActionTracker.Where(a => a.iaId == reg.insurenceActionTrackerId)
                                 select ap).FirstOrDefaultAsync();
                ins.isDeleted = 1;
                _context.SaveChanges();
                return Ok(ins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
