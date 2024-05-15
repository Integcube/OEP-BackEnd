using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ActionTrakingSystem.Controllers
{

    public class AssignTilActionController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AssignTilActionController(DAL context, IWebHostEnvironment hostingEnvironment) 
        {

            _context= context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("getFilterSites")]
        public async Task<IActionResult> GetSites(FilterEqSiteDto reg)
        {
            try
            {
                var sites = await (from r in _context.Regions2.Where(a => a.regionId == reg.regionId)
                                   join s in _context.Sites.Where(a => a.isDeleted == 0) on r.regionId equals s.regionId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }).Distinct().ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getFilterEq")]
        public async Task<IActionResult> GetEq(filterEqTap reg)
        {
            try
            {
                var sites = await (from e in _context.SiteEquipment.Where(a => a.isDeleted == 0)
                                   join s in _context.Sites.Where(r=>r.siteId == reg.siteId) on e.siteId equals s.siteId
                                   select new
                                   {
                                       e.equipmentId,
                                       //e.siteId,
                                       unit = e.unit,
                                   }).Distinct().ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //[Authorize]
        //[HttpPost("getTAT")]
        //public async Task<IActionResult> GetTilPackage(packageActionUserDto reg)
        //{
        //    try
        //    {
        //        var action = await (from a in _context.TILActionTracker.Where(a => a.tapId == reg.packageId && a.isDeleted == 0)
        //                            join tap in _context.TILActionPackage on a.tapId equals tap.packageId
        //                            join tils in _context.TILBulletin on tap.tilId equals tils.tilId
        //                            join se in _context.SiteEquipment on a.siteEquipmentId equals se.equipmentId
        //                            join au in _context.AppUser on a.assignedTo equals au.userId
        //                            join bs in _context.TABudgetSource on a.budgetSourceId equals bs.budgetSourceId into all
        //                            from _bs in all.DefaultIfEmpty()
        //                            join df in _context.TAStatus on a.statusId equals df.tasId into asdas
        //                            from _sts in asdas.DefaultIfEmpty()
        //                            join b in _context.TABudget on a.budgetId equals b.budgetId into all2
        //                            from _b in all2.DefaultIfEmpty()
        //                            join ps in _context.TAParts on a.partsServiceId equals ps.partId into all3
        //                            from _ps in all3.DefaultIfEmpty()
        //                            join fi in _context.TAFinalImplementation on a.finalImplementationId equals fi.finalImpId into all4
        //                            from _fi in all4.DefaultIfEmpty()
        //                            join sap in _context.TASapPlanning on a.sapPlanningId equals sap.sapPlanningId into all5
        //                            from _sap in all5.DefaultIfEmpty()
        //                            join p in _context.TAPPriority on a.priorityId equals p.priorityId into all6
        //                            from _p in all6.DefaultIfEmpty()
        //                            join ev in _context.TAEvidence on a.evidenceId equals ev.evidenceId into all7
        //                            from _ev in all7.DefaultIfEmpty()
        //                            select new
        //                            {
        //                                a.tilActionTrackerId,
        //                                a.tapId,
        //                                a.statusId,
        //                                statustitle = _sts.title,
        //                                tapTitle = tap.actionTitle,
        //                                tilAction =tap.actionTitle,
        //                                a.actionDescription,
        //                                a.siteEquipmentId,
        //                                siteEquipmentTitle = se.unit,
        //                                au.userId,
        //                                au.userName,
        //                                budgetSourceId = a.budgetSourceId,
        //                                budgetSourceTitle = _bs.budgetSourceTitle,
        //                                a.statusCalculated,
        //                                a.siteStatusDetail,
        //                                a.budgetId,
        //                                tilDescription = tils.tilTitle,
        //                                budgetTitle = _b.budgetName,
        //                                partServiceId = a.partsServiceId,
        //                                partServiceTitle = _ps.partTitle,
        //                                planningId = a.sapPlanningId,
        //                                planningTitle = _sap.sapPlanningTitle,
        //                                finalImplementationTitle = a.finalImplementationId,
        //                                finalImplementationId = _fi.finalImpTitle,
        //                                targetDate = a.targetDate,
        //                                priorityId = a.priorityId,
        //                                priorityTitle = _p.priorityTitle,
        //                                a.calStatus,
        //                                a.calcPriority,
        //                                a.budgetCalc,
        //                                a.ddtCalc,
        //                                a.evidenceCalc,
        //                                a.implementationCalc,
        //                                a.partsCalc,
        //                                a.sapCalc,
        //                                a.evidenceId,
        //                                evidenceTitle = _ev.evidenceTitle
        //                            }).ToListAsync();

        //        var package = await (from acc in _context.TILActionPackage.Where(a => a.packageId == reg.packageId)
        //                             join tapeq in _context.TAPEquipment on acc.packageId equals tapeq.tapId
        //                             join asd in _context.SiteEquipment on tapeq.eqId equals asd.equipmentId
        //                             join s in _context.Sites on asd.siteId equals s.siteId
        //                             join r in _context.Regions on s.regionId equals r.regionId
        //                             join t in _context.TILBulletin on acc.tilId equals t.tilId into all1
        //                             from aa in all1.DefaultIfEmpty()
        //                             join acg in _context.ActionClosureGuidelines on acc.actionClosureGuidelinesId equals acg.acId into all2
        //                             from bb in all2.DefaultIfEmpty()
        //                             join outage in _context.OutageTypes on acc.outageId equals outage.outageTypeId into all3
        //                             from cc in all3.DefaultIfEmpty()
        //                             join bs in _context.TAPBudgetSource on acc.budgetSourceId equals bs.budgetSourceId into all4
        //                             from dd in all4.DefaultIfEmpty()
        //                             join p in _context.TAPPriority on acc.priorityId equals p.priorityId into all5
        //                             from ee in all5.DefaultIfEmpty()
        //                             join rs in _context.TAPReviewStatus on acc.reviewStatusId equals rs.reviewStatusId into all6
        //                             from ff in all6.DefaultIfEmpty()
        //                             select new
        //                             {
        //                                 acc.packageId,
        //                                 tilId = acc.tilId,
        //                                 tilNumber = aa.tilNumber,
        //                                 acc.actionTitle,
        //                                 acc.actionClosureGuidelinesId,
        //                                 actionCategory = bb.title,
        //                                 acc.outageId,
        //                                 unitStatus = cc.title,
        //                                 acc.actionDescription,
        //                                 acc.expectedBudget,
        //                                 acc.budgetSourceId,
        //                                 budegetSource = dd.budgetSourceTitle,
        //                                 acc.patComments,
        //                                 acc.priorityId,
        //                                 priorityTitle = ee.priorityTitle,
        //                                 acc.recurrence,
        //                                 acc.reviewStatusId,
        //                                 reviewStatus = ff.reviewStatusTitle,
        //                                 siteId = s.siteId,
        //                                 siteTitle = s.siteName,
        //                                 regionId = s.regionId,
        //                                 regionTitle = r.title
        //                             }).FirstOrDefaultAsync();

        //        var equipment = await (from t in _context.TAPEquipment.Where(a => a.tapId == reg.packageId)
        //                        join e in _context.SiteEquipment on t.eqId equals e.equipmentId
        //                          select new
        //                          {
        //                              e.equipmentId,
        //                              unit = e.unit,
        //                          }).Distinct().ToListAsync();

        //        var priority = await (from t in _context.TAPPriority.Where(a=>a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.priorityTitle,
        //                                  t.priorityId,
        //                              }).Distinct().ToListAsync();
        //        var statusList = await (from s in _context.TAStatus.Where(a => a.isDeleted == 0)
        //                                select new
        //                                {
        //                                    statusId = s.tasId,
        //                                    statustitle = s.title

        //                                }).ToArrayAsync();
        //        var budgetSource = await (from t in _context.TABudgetSource.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.budgetSourceId,
        //                                  t.budgetSourceTitle,
        //                              }).Distinct().ToListAsync();
        //        var budget = await (from t in _context.TABudget.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.budgetId,
        //                                  t.budgetName,
        //                              }).Distinct().ToListAsync();
        //        var part = await (from t in _context.TAParts.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.partId,
        //                                  t.partTitle,
        //                              }).Distinct().ToListAsync();
        //        var finalImplementation = await (from t in _context.TAFinalImplementation.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.finalImpId,
        //                                  t.finalImpTitle,
        //                              }).Distinct().ToListAsync();
        //        var sapPlaning = await (from t in _context.TASapPlanning.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.sapPlanningTitle,
        //                                  t.sapPlanningId,
        //                              }).Distinct().ToListAsync();
        //        var evidence = await (from t in _context.TAEvidence.Where(a => a.isDeleted == 0)
        //                              select new
        //                              {
        //                                  t.evidenceId,
        //                                  t.evidenceTitle,
        //                              }).Distinct().ToListAsync();

        //        var users = await (from u in _context.AppUser.Where(a => a.isDeleted == 0)
        //                           join aus in _context.AUSite on u.userId equals aus.userId
        //                           join aut in _context.AUTechnology on u.userId equals aut.userId
        //                           join s in _context.Sites on aus.siteId equals s.siteId
        //                           join stech in _context.SitesTechnology on s.siteId equals stech.siteId
        //                           join e in _context.SiteEquipment on s.siteId equals e.siteId
        //                           join te in _context.TAPEquipment on e.equipmentId equals te.eqId
        //                           join tp in _context.TILActionPackage.Where(a => a.packageId == reg.packageId) on te.tapId equals tp.packageId
        //                           select new
        //                           {
        //                               u.email,
        //                               u.userName,
        //                               u.userId,
        //                           }).Distinct().ToListAsync();


        //        var obj = new
        //        {
        //            action,
        //            package,
        //            equipment,
        //            priority,
        //            budgetSource,
        //            budget,
        //            part,
        //            finalImplementation,
        //            evidence,
        //            users,
        //            sapPlaning,
        //            statusList
        //        };
        //        return Ok(obj);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var equipments = await (from e in _context.SiteEquipment.Where(a => a.isDeleted == 0)
                                        join s in _context.Sites on e.siteId equals s.siteId into all
                                        from ss in all.DefaultIfEmpty()
                                        join r in _context.Regions2 on ss.region2 equals r.regionId into all2
                                        from rr in all2.DefaultIfEmpty()
                                        //join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                        //join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                        //join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                        select new
                                        {
                                            e.equipmentId,
                                            e.siteId,
                                            unit = e.unit,
                                            siteTitle = ss.siteName,
                                            regionId = ss.regionId,
                                            regionTitle = rr.title,
                                            e.isGroup,
                                            e.groupedEquipments,
                                        }).Distinct().ToListAsync();
                var priority = await (from tc in _context.TAPPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          tc.priorityId,
                                          tc.priorityTitle
                                      }).ToListAsync();
                var budgetSource = await (from tc in _context.TAPBudgetSource.Where(a => a.isDeleted == 0)

                                          select new
                                          {
                                              tc.budgetSourceId,
                                              tc.budgetSourceTitle
                                          }).ToListAsync();


                var tilsList = await (from t in _context.TILBulletin.Where(a => a.isDeleted == 0)
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
                                  }).OrderByDescending(a => a.tilId).ToListAsync();
                var reviewStatus = await (from tc in _context.TAPReviewStatus.Where(a => a.isDeleted == 0)
                                          select new
                                          {
                                              tc.reviewStatusId,
                                              tc.reviewStatusTitle,
                                          }).ToListAsync();
                var actionClosureGuidelines = await (from tc in _context.ActionClosureGuidelines.Where(a => a.isDeleted == 0)
                                                     select new
                                                     {
                                                         tc.acId,
                                                         tc.title,
                                                     }).ToListAsync();
                var outages = await (from tc in _context.OutageTypes.Where(a => a.isDeleted == 0)
                                     select new
                                     {
                                         tc.outageTypeId,
                                         tc.title,
                                     }).ToListAsync();
                var obj = new
                {
                    equipments,
                    priority,
                    budgetSource,
                    reviewStatus,
                    actionClosureGuidelines,
                    outages,
                    tilsList
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getAssignedActions")]
        public async Task<IActionResult> GetAssignedActions(TAPFilterUserDto rege)
        {
            try
            {
                List<int> TilIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.tilList))
                    TilIds = (rege.tilList.Split(',').Select(Int32.Parse).ToList());

                List<int> OuatgeIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.outageList))
                    OuatgeIds = (rege.outageList.Split(',').Select(Int32.Parse).ToList());

                List<int> BudgetIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.budgetList))
                    BudgetIds = (rege.budgetList.Split(',').Select(Int32.Parse).ToList());

                List<int> ReviewIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.reviewList))
                    ReviewIds = (rege.reviewList.Split(',').Select(Int32.Parse).ToList());

                List<int> PriorityIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.priority))
                    PriorityIds = (rege.priority.Split(',').Select(Int32.Parse).ToList());


                var actions = await (from action in _context.TILActionPackage.Where(a => a.isDeleted == 0 && ((BudgetIds.Count == 0) || BudgetIds.Contains((int)a.budgetSourceId)) && ((TilIds.Count == 0) || TilIds.Contains((int)a.tilId)) && ((ReviewIds.Count == 0) || ReviewIds.Contains((int)a.reviewStatusId)) && ((OuatgeIds.Count == 0) || OuatgeIds.Contains((int)a.outageId)))
                                     join tapeqs in _context.TAPEquipment on action.packageId equals tapeqs.tapId into alll11
                                     from tapeq in alll11.DefaultIfEmpty()
                                         //join eqs in _context.SiteEquipment on tapeq.eqId equals eqs.equipmentId into alll12
                                         //from eq in alll12.DefaultIfEmpty()
                                         //join ss in _context.Sites on eq.siteId equals ss.siteId into alll13
                                         //from s in alll13.DefaultIfEmpty()
                                         //join stechs in _context.SitesTechnology on s.siteId equals stechs.siteId into alll14
                                         //from stech in alll14.DefaultIfEmpty()
                                         //join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                         //join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join t in _context.TILBulletin on action.tilId equals t.tilId into all1
                                     from aa in all1.DefaultIfEmpty()
                                     join acg in _context.ActionClosureGuidelines on action.actionClosureGuidelinesId equals acg.acId into all2
                                     from bb in all2.DefaultIfEmpty()
                                     join outage in _context.OutageTypes on action.outageId equals outage.outageTypeId into all3
                                     from cc in all3.DefaultIfEmpty()
                                     join bs in _context.TAPBudgetSource on action.budgetSourceId equals bs.budgetSourceId into all4
                                     from dd in all4.DefaultIfEmpty()
                                     join p in _context.TAPPriority on action.priorityId equals p.priorityId into all5
                                     from ee in all5.DefaultIfEmpty()
                                     join rs in _context.TAPReviewStatus on action.reviewStatusId equals rs.reviewStatusId into all6
                                     from ff in all6.DefaultIfEmpty()
                                     select new
                                     {
                                         action.packageId,
                                         tilId = action.tilId,
                                         tilNumber = aa.tilNumber,
                                         action.actionTitle,
                                         action.actionClosureGuidelinesId,
                                         actionCategory = bb.title,
                                         action.outageId,
                                         unitStatus = cc.title,
                                         action.actionDescription,
                                         action.expectedBudget,
                                         action.budgetSourceId,
                                         budegetSource = dd.budgetSourceTitle,
                                         action.patComments,
                                         priorityId = action.priorityId == null? -1 : action.priorityId,
                                         priorityTitle = ee.priorityTitle,
                                         action.recurrence,
                                         action.reviewStatusId,
                                         reviewStatus = ff.reviewStatusTitle,
                                         //siteTitle = s.siteName,
                                         //regionId = s.regionId,
                                         //regionTitle = r.title,
                                     }).Distinct().OrderByDescending(a => a.packageId).ToListAsync();


                var obj = new
                {
                    actions = actions.Where(a => (PriorityIds.Count == 0) || PriorityIds.Contains((int)a.priorityId))
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteAssignedActions")]
        public async Task<IActionResult> DeleteRegion(actionPackageDto reg)
        {
            try
            {
                TILActionPackage action = await (from r in _context.TILActionPackage.Where(a => a.packageId == reg.packageId)
                                         select r).FirstOrDefaultAsync();
                action.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(action);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteAction")]
        public async Task<IActionResult> deleteAction(tilPackagetrackeruser reg)
        {
            try
            {
                TILActionTracker action = await (from r in _context.TILActionTracker.Where(a => a.tilActionTrackerId == reg.action.tilActionTrackerId)
                                                 select r).FirstOrDefaultAsync();
                action.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(action);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getUsersActions")]
        public async Task<IActionResult> GetUsers(actionPackageDto reg)
        {
            try
            {
                //var users = await (from ap in _context.TAPUsers.Where(a=>a.tapId == reg.packageId)
                //                   join u in _context.AppUser on ap.userId equals u.userId
                //                   select new
                //                   {
                //                       u.email,
                //                       u.userId,
                //                       u.userName,
                //                   }).ToListAsync();
                var equipments = await (from e in _context.TAPEquipment.Where(a => a.tapId == reg.packageId)
                                        join eq in _context.SiteEquipment on e.eqId equals eq.equipmentId into all1
                                        from bb in all1.DefaultIfEmpty()
                                        join s in _context.Sites on bb.siteId equals s.siteId into all
                                        from aa in all.DefaultIfEmpty()
                                        select new
                                        {
                                            equipmentId = e.eqId,
                                            aa.siteId,
                                            aa.siteName,
                                            unit = bb.unit + "-" + aa.siteName,
                                           
                                        }).ToListAsync();
                var data = new {
                    //users,
                    equipments
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        ////[Authorize]
        ////[HttpPost("saveAction")]
        ////public async Task<IActionResult> UpdateActionTracker(tilPackagetrackeruser reg)
        ////{
        ////    try
        ////    {
        ////        if (reg.action.tilActionTrackerId != -1)
        ////        {
        ////            TILActionTracker til = await (from t in _context.TILActionTracker.Where(a => a.tilActionTrackerId == reg.action.tilActionTrackerId)
        ////                                          select t).FirstOrDefaultAsync();
        ////            til.actionDescription = reg.action.actionDescription;
        ////            til.tapId = reg.action.tapId;
        ////            til.siteEquipmentId = reg.action.siteEquipmentId;
        ////            til.priorityId = reg.action.priorityId;
        ////            til.budgetId = reg.action.budgetId;
        ////            til.targetDate = reg.action.targetDate;
        ////            til.assignedTo = reg.action.userId;
        ////            til.modifiedBy = reg.userId;
        ////            til.modifiedOn = DateTime.Now;
        ////            til.tilAction = reg.action.tilAction;

        ////            if (til.targetDate !=null)
        ////            {
        ////                DateTime todayDate = DateTime.Now;
        ////                TimeSpan duration = Convert.ToDateTime(reg.action.targetDate) - Convert.ToDateTime(todayDate);
        ////                int days = duration.Days;
        ////                if (days > 30)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 1)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score*100).ToString() + "%";
        ////                }
        ////                if (days > 182)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 3)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }
        ////                if (days > 365)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 4)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }

        ////            }

        ////            _context.SaveChanges();

        ////            return Ok(reg.action);
        ////        }
        ////        else
        ////        {
        ////            TILActionTracker til = new TILActionTracker();
        ////            til.actionDescription = reg.action.actionDescription;
        ////            til.tapId = reg.action.tapId;
        ////            til.siteEquipmentId = reg.action.siteEquipmentId;
        ////            til.priorityId = reg.action.priorityId;
        ////            til.targetDate = reg.action.targetDate;
        ////            til.assignedTo = reg.action.userId;
        ////            til.budgetId = reg.action.budgetId;
        ////            til.createdBy = reg.userId;
        ////            til.createdOn = DateTime.Now;
        ////            til.isDeleted = 0;
        ////            til.tilAction = reg.action.tilAction;
        ////            if (til.targetDate != null)
        ////            {
        ////                var todayDate = DateTime.Now;
        ////                var target = reg.action.targetDate;
        ////                TimeSpan duration = target - todayDate;
        ////                int days = duration.Days;
        ////                if (days < 30)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 1)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }
        ////                if (days < 182)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 3)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }
        ////                if (days < 365)
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 4)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }
        ////                else
        ////                {
        ////                    var ddt = await (from a in _context.TADayToTarget.Where(a => a.dayId == 2)
        ////                                     select a).FirstOrDefaultAsync();
        ////                    til.ddtCalc = (ddt.score * 100).ToString() + "%";
        ////                }

        ////            }
        ////            _context.Add(til);
        ////            _context.SaveChanges();
        ////            return Ok(reg.action);
        ////        }

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest(ex.Message);
        ////    }
        ////}
        [Authorize]
        [HttpPost("copyAction")]
        public async Task<IActionResult> CopyAction(copyActionDto reg)
        {
            try
            {
               
                    TILActionPackage tils = new TILActionPackage();
                    tils.actionDescription = reg.data.actionDescription;
                    tils.patComments = reg.data.patComments;
                    tils.recurrence = reg.data.recurrence;
                    tils.budgetSourceId = reg.data.budgetSourceId;
                    tils.actionTitle = reg.data.actionTitle;
                    tils.expectedBudget = reg.data.expectedBudget;
                    tils.actionClosureGuidelinesId = reg.data.actionClosureGuidelinesId;
                    tils.tilId = reg.data.tilId;
                    tils.outageId = reg.data.outageId;
                    tils.priorityId = reg.data.priorityId;
                    tils.reviewStatusId = reg.data.reviewStatusId;
                    tils.createdBy = reg.userId;
                    tils.createdOn = DateTime.Now;
                    tils.isDeleted = 0;
                    _context.Add(tils);
                    _context.SaveChanges();
                List<TAPEquipment> equipment = await (from a in _context.TAPEquipment.Where(a => a.tapId == reg.data.packageId)
                                       select a).ToListAsync();

                    for (var i = 0; i < equipment.Count; i++)
                    {
                        TAPEquipment eq = new TAPEquipment();
                        eq.tapId = tils.packageId;
                        eq.eqId = equipment[i].eqId;
                        eq.isDeleted = 0;
                        _context.Add(eq);
                        //SaveTILUserAccess(_context, eq.tapId, eq.eqId, reg.userId);
                    }
                    _context.SaveChanges();
                    return Ok(reg.data);
                

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveUsersActions")]
        public async Task<IActionResult> UpdateAction(assignTilUser reg)
        {
            try
            {
                if (reg.data.action.packageId != -1)
                {
                    TILActionPackage til = await (from t in _context.TILActionPackage.Where(a => a.packageId == reg.data.action.packageId)
                                                  select t).FirstOrDefaultAsync();
                    til.actionDescription = reg.data.action.actionDescription;
                    til.patComments = reg.data.action.patComments;
                    til.recurrence = reg.data.action.recurrence;
                    til.budgetSourceId = reg.data.action.budgetSourceId;
                    til.actionTitle = reg.data.action.actionTitle;
                    til.expectedBudget = reg.data.action.expectedBudget;
                    til.actionClosureGuidelinesId = reg.data.action.actionClosureGuidelinesId;
                    til.packageId = reg.data.action.packageId;
                    til.tilId = reg.data.action.tilId;
                    til.priorityId = reg.data.action.priorityId;
                    til.reviewStatusId = reg.data.action.reviewStatusId;
                    til.outageId = reg.data.action.outageId;
                    til.modifiedBy = reg.userId;
                    til.modifiedOn = DateTime.Now;
                    //if (reg.user.Length > 0)
                    //{
                    //    _context.Database.ExecuteSqlCommand("DELETE FROM TAPUsers WHERE tapId = @tapID", new SqlParameter("tapID", reg.action.packageId));
                    //    for (var i = 0; i < reg.user.Length; i++)
                    //    {
                    //        TAPUsers user = new TAPUsers();
                    //        user.tapId = reg.action.packageId;
                    //        user.userId = reg.user[i].userId;
                    //        user.isDeleted = 0;
                    //        _context.Add(user);
                    //    }
                    //}
                    _context.Database.ExecuteSqlCommand("DELETE FROM TAPEquipment WHERE tapId = @tapID", new SqlParameter("tapID", reg.data.action.packageId));

                    if (reg.data.equipment.Length > 0)
                    {
                        for (var i = 0; i < reg.data.equipment.Length; i++)
                        {
                            TAPEquipment eq = new TAPEquipment();
                            eq.tapId =til.packageId;
                            eq.eqId = reg.data.equipment[i].equipmentId;
                            eq.isDeleted = 0;
                            _context.Add(eq);
                            //SaveTILUserAccess(_context, eq.tapId, eq.eqId, reg.userId);
                        }
                    }

                    _context.SaveChanges();

                    return Ok(reg.data.action);
                }
                else
                {
                    TILActionPackage tils = new TILActionPackage();
                    tils.actionDescription = reg.data.action.actionDescription;
                    tils.patComments = reg.data.action.patComments;
                    tils.recurrence = reg.data.action.recurrence;
                    tils.budgetSourceId = reg.data.action.budgetSourceId;
                    tils.actionTitle = reg.data.action.actionTitle;
                    tils.expectedBudget = reg.data.action.expectedBudget;
                    tils.actionClosureGuidelinesId = reg.data.action.actionClosureGuidelinesId;
                    tils.tilId = reg.data.action.tilId;
                    tils.outageId = reg.data.action.outageId;

                    tils.priorityId = reg.data.action.priorityId;
                    tils.reviewStatusId = reg.data.action.reviewStatusId;
                    tils.createdBy = reg.userId;
                    tils.createdOn = DateTime.Now;
                    tils.isDeleted = 0;
                    _context.Add(tils);
                    _context.SaveChanges();
                    //for (var i = 0; i < reg.user.Length; i++)
                    //{
                    //    TAPUsers user = new TAPUsers();
                    //    user.tapId = reg.action.packageId;
                    //    user.userId = reg.user[i].userId;
                    //    user.isDeleted = 0;
                    //    _context.Add(user);
                    //}
                    for (var i = 0; i < reg.data.equipment.Length; i++)
                    {
                        TAPEquipment eq = new TAPEquipment();
                        eq.tapId = tils.packageId;
                        eq.eqId = reg.data.equipment[i].equipmentId;
                        eq.isDeleted = 0;
                        _context.Add(eq);
                        //SaveTILUserAccess(_context, eq.tapId, eq.eqId, reg.userId);
                    }
                    _context.SaveChanges();
                    return Ok(reg.data.action);
                }
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("downloadFile/{tatId}")]
        public async Task<IActionResult> DownloadTilFile(int tatId)
        {
            try
            {
                var fileIR = await (from a in _context.TILActiontrackerFile.Where(a => a.tapId == tatId && a.isDeleted == 0)
                                    select a).FirstOrDefaultAsync();

                if (fileIR != null)
                {
                    var tilUrl = fileIR.reportPath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + tilUrl;

                    // Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
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

        //public static void SaveTILUserAccess(DAL dal, int packageId, int equipId, int userId)
        //{

        //    dal.Database.ExecuteSqlRaw("EXEC TILAccessUser @PackageId, @EquipId, @UserId",
        //new SqlParameter("@PackageId", packageId),
        //new SqlParameter("@EquipId", equipId),
        //new SqlParameter("@UserId", userId));

        //}

    }
}
