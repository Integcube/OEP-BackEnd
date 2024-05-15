using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis;

namespace ActionTrakingSystem.Controllers
{
    public class MitigationActionPlanActionsController : BaseAPIController
    {
        private readonly DAL _context;
        public MitigationActionPlanActionsController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterface(UserIdDto reg)
        {
            try
            {

                var status = await (from a in _context.OMA_IStatus.Where(a => a.isDeleted == 0 )
                                    select new
                                    {
                                        a.statusId,
                                        a.statusTitle,
                                    }).ToListAsync();
                var programs = await (from a in _context.OMA_IProgram.Where(a => a.isDeleted == 0)
                                      join at in _context.OMA_ProgramTechnologies.Where(a => a.isDeleted == 0) on a.programId equals at.programId
                                      join t in _context.Technology.Where(a => a.isDeleted == 0) on at.technologyId equals t.techId
                                      join aut in _context.AUTechnology.Where(a => (reg.userId == -1 || a.userId == reg.userId)) on t.techId equals aut.technologyId
                                      select new
                                      {
                                          a.programId,
                                          a.programTitle,
                                      }).Distinct().ToListAsync();
                var prioritys = await (from a in _context.OMA_IPriority.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           a.priorityId,
                                           a.priorityTitle,
                                       }).ToListAsync();
                var techAccountabilities = await (from a in _context.OMA_ITechAccountability.Where(a => a.isDeleted == 0)
                                                  select new
                                                  {
                                                      a.taId,
                                                      a.taTitle,
                                                  }).ToListAsync();
                var keyPhase = await (from u in _context.OMA_IKeyPhases.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          u.keyPhaseId,
                                          u.keyPhaseTitle,
                                          u.keyPhaseCode
                                      }).ToListAsync();


                var obj = new
                {
                    status,
                    programs,
                    prioritys,
                    techAccountabilities,
                    keyPhase
                };
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getMitigationActions")]
        public async Task<IActionResult> GetProgram(UserIdDto reg)
        {
            try
            {

                var mitigationActions = await (from a in _context.OMA_MitigationAction.Where(a => a.isDeleted == 0)
                                               join p in _context.OMA_IProgram on a.programId equals p.programId
                                               join pr in _context.OMA_IPriority on a.priorityId equals pr.priorityId
                                               join it in _context.OMA_ITechAccountability on a.techAccountabilityId equals it.taId into all2
                                               from itt in all2.DefaultIfEmpty()
                                               select new
                                               {
                                                   a.actionId,
                                                   a.actionTitle,
                                                   a.priorityId,
                                                   pr.priorityTitle,
                                                   a.programId,
                                                   p.programTitle,
                                                   a.techAccountabilityId,
                                                   itt.taTitle,
                                                   a.comments,
                                                   a.objectiveOutcome,
                                                   //a.targetDate,
                                               }).OrderByDescending(a => a.actionId).ToListAsync();

                return Ok(mitigationActions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getMitigationResultReviewer")]
        public async Task<IActionResult> MitigationResultReview(MitigationResultUserIDDto reg)
        {
            try
            {
                //List<int> siteIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.siteList))
                //    siteIDs = (reg.siteList.Split(',').Select(Int32.Parse).ToList());
                //List<int> technologyIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.technologyList))
                //    technologyIDs = (reg.technologyList.Split(',').Select(Int32.Parse).ToList());
                //List<int> statusIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.statusList))
                //    statusIDs = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                //List<int> priorityIds = new List<int>();
                //if (!string.IsNullOrEmpty(reg.priorityList))
                //    priorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());
                //List<int> programIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.programList))
                //    programIDs = (reg.programList.Split(',').Select(Int32.Parse).ToList());

                var mitigationActions = await (from a in _context.OMA_MitigationAction.Where(a => a.isDeleted == 0 && ((reg.filter.priority == -1) || a.priorityId == reg.filter.priority) && ((reg.filter.program == -1) || a.programId == reg.filter.program))
                                               join pr in _context.OMA_IPriority on a.priorityId equals pr.priorityId
                                               join it in _context.OMA_ITechAccountability on a.techAccountabilityId equals it.taId into all2
                                               from itt in all2.DefaultIfEmpty()
                                               join p in _context.OMA_IProgram on a.programId equals p.programId
                                               join pt in _context.OMA_ProgramTechnologies on p.programId equals pt.programId
                                               join t in _context.Technology.Where(a => a.isDeleted == 0 && ((reg.filter.technology == -1) || a.techId == reg.filter.technology)) on pt.technologyId equals t.techId
                                               join st in _context.SitesTechnology.Where(a => a.isDeleted == 0) on t.techId equals st.techId
                                               join s in _context.Sites.Where(a => a.isDeleted == 0 && ((reg.filter.site == -1) || a.siteId == reg.filter.site)) on st.siteId equals s.siteId
                                               join sut in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals sut.siteId
                                               join mr in _context.OMA_MitigationResult on new { a.actionId, s.siteId, t.techId } equals new { mr.actionId, mr.siteId, techId = mr.technologyId } into all1
                                               from mrr in all1.DefaultIfEmpty()
                                               join sc in _context.OMA_SiteControl on new { s.siteId, t.techId, p.programId } equals new { sc.siteId, techId = sc.technologyId, sc.programId }

                                               join status in _context.OMA_IStatus on mrr.statusId equals status.statusId into all3
                                               from stss in all3.DefaultIfEmpty()
                                               select new
                                               {
                                                   a.actionId,
                                                   a.actionTitle,
                                                   a.priorityId,
                                                   pr.priorityTitle,
                                                   a.programId,
                                                   p.programTitle,
                                                   a.techAccountabilityId,
                                                   itt.taTitle,
                                                   a.comments,
                                                   a.objectiveOutcome,
                                                   mrr.targetDate,
                                                   s.siteId,
                                                   siteTitle = s.siteName,
                                                   technologyId = t.techId,
                                                   technologyTitle = t.name,
                                                   resultId = mrr.resultId == null ? -1 : mrr.resultId,
                                                   tataInvolvement = mrr.tataInvolvement == 1 ? true : false,
                                                   mrr.thirdPartyInterface,
                                                   statusId = mrr.statusId == null ? -1 : mrr.statusId,
                                                   statusTitle = stss.statusTitle,
                                                   mrr.reviewerComment,
                                                   isReviewed = mrr.isReviewed == 1 ? true : false,
                                                   mrr.actionComment,
                                                   rework = mrr.rework == 1 ? true : false
                                               }).Distinct().OrderByDescending(a => a.actionId).ToListAsync();
                var dataA = (from s in mitigationActions
                             select new
                             {
                                 s.actionId,
                                 s.actionTitle,
                                 s.priorityId,
                                 s.priorityTitle,
                                 s.programId,
                                 s.programTitle,
                                 s.techAccountabilityId,
                                 s.taTitle,
                                 s.comments,
                                 s.objectiveOutcome,
                                 s.targetDate,
                                 s.siteId,
                                 s.siteTitle,
                                 s.technologyId,
                                 s.technologyTitle,
                                 s.resultId,
                                 s.tataInvolvement,
                                 s.thirdPartyInterface,
                                 statusId = processStatusId(s.targetDate, (int)s.statusId),
                                 //statusTitle= processStatusTitle((int)statusId),
                                 s.reviewerComment,
                                 s.isReviewed,
                                 s.actionComment,
                                 s.rework


                             }).ToList();

                var dataAb = (from s in dataA
                              select new
                              {
                                  s.actionId,
                                  s.actionTitle,
                                  s.priorityId,
                                  s.priorityTitle,
                                  s.programId,
                                  s.programTitle,
                                  s.techAccountabilityId,
                                  s.taTitle,
                                  s.comments,
                                  s.objectiveOutcome,
                                  s.targetDate,
                                  s.siteId,
                                  s.siteTitle,
                                  s.technologyId,
                                  s.technologyTitle,
                                  s.resultId,
                                  s.tataInvolvement,
                                  s.thirdPartyInterface,
                                  s.statusId,
                                  statusTitle = processStatusTitle((int)s.statusId),
                                  s.reviewerComment,
                                  s.isReviewed,
                                  s.actionComment,
                                  s.rework


                              }).ToList().Where(a => ((reg.filter.status == -1) || a.statusId == reg.filter.status));

                return Ok(dataAb);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getMitigationResult")]
        public async Task<IActionResult> MitigationResult(MitigationResultUserIDDto reg)
        {
            try
            {
                //List<int> siteIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.siteList))
                //    siteIDs = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

                //List<int> technologyIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.technologyList))
                //    technologyIDs = (reg.technologyList.Split(',').Select(Int32.Parse).ToList());

                //List<int> statusIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.statusList))
                //    statusIDs = (reg.statusList.Split(',').Select(Int32.Parse).ToList());

                //List<int> programIDs = new List<int>();
                //if (!string.IsNullOrEmpty(reg.programList))
                //    programIDs = (reg.programList.Split(',').Select(Int32.Parse).ToList());

                //List<int> priorityIds = new List<int>();
                //if (!string.IsNullOrEmpty(reg.priorityList))
                //    priorityIds = (reg.priorityList.Split(',').Select(Int32.Parse).ToList());

                var mitigationActions = await (from a in _context.OMA_MitigationAction.Where(a => a.isDeleted == 0 && ((reg.filter.priority == -1) || a.priorityId == reg.filter.priority) && ((reg.filter.program == -1) || a.programId == reg.filter.program))
                                               join pr in _context.OMA_IPriority on a.priorityId equals pr.priorityId
                                               join it in _context.OMA_ITechAccountability on a.techAccountabilityId equals it.taId into all2
                                               from itt in all2.DefaultIfEmpty()
                                               join p in _context.OMA_IProgram on a.programId equals p.programId
                                               join pt in _context.OMA_ProgramTechnologies on p.programId equals pt.programId
                                               join t in _context.Technology.Where(a => a.isDeleted == 0 && ((reg.filter.technology == -1) || a.techId == reg.filter.technology)) on pt.technologyId equals t.techId
                                               join st in _context.SitesTechnology.Where(a => a.isDeleted == 0) on t.techId equals st.techId
                                               join s in _context.Sites.Where(a => a.isDeleted == 0 && ((reg.filter.site == -1) || a.siteId == reg.filter.site)) on st.siteId equals s.siteId
                                               join sut in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals sut.siteId
                                               join sc in _context.OMA_SiteControl on new { s.siteId, t.techId, p.programId } equals new { sc.siteId , techId= sc.technologyId, sc.programId}

                                               join mr in _context.OMA_MitigationResult on new { a.actionId, s.siteId, t.techId } equals new { mr.actionId, mr.siteId, techId = mr.technologyId } into all1
                                               from mrr in all1.DefaultIfEmpty()
                                               join status in _context.OMA_IStatus on mrr.statusId equals status.statusId into all3
                                               from stss in all3.DefaultIfEmpty()
                                               select new
                                               {
                                                   a.actionId,
                                                   a.actionTitle,
                                                   a.priorityId,
                                                   pr.priorityTitle,
                                                   a.programId,
                                                   p.programTitle,
                                                   a.techAccountabilityId,
                                                   itt.taTitle,
                                                   a.comments,
                                                   a.objectiveOutcome,
                                                   mrr.targetDate,
                                                   s.siteId,
                                                   siteTitle = s.siteName,
                                                   technologyId = t.techId,
                                                   technologyTitle = t.name,
                                                   resultId = mrr.resultId == null? -1: mrr.resultId,
                                                   tataInvolvement = mrr.tataInvolvement == 1 ? true : false,
                                                   mrr.thirdPartyInterface,
                                                   statusId = mrr.statusId == null? -1:mrr.statusId,
                                                   //statusTitle = stss.statusTitle,
                                                   mrr.reviewerComment,
                                                   isReviewed = mrr.isReviewed == 1 ? true : false,
                                                   mrr.actionComment,
                                                   rework = mrr.rework == 1 ? true : false
                                               }).Distinct().OrderByDescending(a => a.actionId).ToListAsync();

                var dataA = (from s in mitigationActions
                             select new
                             {
                                 s.actionId,
                                 s.actionTitle,
                                 s.priorityId,
                                 s.priorityTitle,
                                 s.programId,
                                 s.programTitle,
                                 s.techAccountabilityId,
                                 s.taTitle,
                                 s.comments,
                                 s.objectiveOutcome,
                                 s.targetDate,
                                 s.siteId,
                                 s.siteTitle,
                                 s.technologyId,
                                 s.technologyTitle,
                                 s.resultId,
                                 s.tataInvolvement,
                                 s.thirdPartyInterface,
                                 statusId = processStatusId(s.targetDate, (int)s.statusId),
                                 //statusTitle= processStatusTitle((int)statusId),
                                 s.reviewerComment,
                                 s.isReviewed,
                                 s.actionComment,
                                 s.rework


                             }).ToList();

                var dataAb = (from s in dataA
                              select new
                              {
                                  s.actionId,
                                  s.actionTitle,
                                  s.priorityId,
                                  s.priorityTitle,
                                  s.programId,
                                  s.programTitle,
                                  s.techAccountabilityId,
                                  s.taTitle,
                                  s.comments,
                                  s.objectiveOutcome,
                                  s.targetDate,
                                  s.siteId,
                                  s.siteTitle,
                                  s.technologyId,
                                  s.technologyTitle,
                                  s.resultId,
                                  s.tataInvolvement,
                                  s.thirdPartyInterface,
                                  s.statusId,
                                  statusTitle= processStatusTitle((int)s.statusId),
                                  s.reviewerComment,
                                  s.isReviewed,
                                  s.actionComment,
                                  s.rework


                              }).ToList().Where(a => ((reg.filter.status == -1) || a.statusId == reg.filter.status));

                return Ok(dataAb);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public static int processStatusId(DateTime? targetDate, int statusId)
        {
           if(statusId == 1 || statusId == 2 || statusId == 4 || statusId == 5)
            {
                return statusId;
            }
           else if (targetDate == null)
            {
                return 3;
            }
            else
            {
                var todayDate = DateTime.Now;
                if (targetDate > todayDate)
                {
                    return 3;
                }
                return 6;
            }
            
        }
        public static string processStatusTitle(int statusId)
        {

            if (statusId == 1)
                return "Implemented";
            else if (statusId == 2)
                return "In Progress";
            else if (statusId == 3)
                return "Not Due Pending";

            else if (statusId == 4)
                return "Not Applicable";

            else if (statusId == 5)
                return "Completed";
            else
                return "Over Due Pending";

        }
        [Authorize]
        [HttpPost("saveMitigationAction")]
        public async Task<IActionResult> SaveMitigationAction(MitigationActionUserDto reg)
        {
            try
            {
                if (reg.mitigationActionObj.mitigationAction.actionId == -1)
                {
                    OMA_MitigationAction action = new OMA_MitigationAction();
                    action.actionTitle = reg.mitigationActionObj.mitigationAction.actionTitle;
                    action.programId = reg.mitigationActionObj.mitigationAction.programId;
                    action.priorityId = reg.mitigationActionObj.mitigationAction.priorityId;
                    action.comments = reg.mitigationActionObj.mitigationAction.comments;
                    action.objectiveOutcome = reg.mitigationActionObj.mitigationAction.objectiveOutcome;
                    //action.targetDate = reg.mitigationActionObj.mitigationAction.targetDate;
                    action.techAccountabilityId = reg.mitigationActionObj.mitigationAction.techAccountabilityId;

                    action.createdOn = DateTime.Now;
                    action.createdBy = reg.userId;
                    _context.Add(action);
                    _context.SaveChanges();
                    reg.mitigationActionObj.mitigationAction.actionId = action.actionId;

                    for (var i = 0; i < reg.mitigationActionObj.keyPhase.Length; i++)
                    {
                        OMA_MitigationActionKeyPhases eq = new OMA_MitigationActionKeyPhases();
                        eq.keyPhaseId = reg.mitigationActionObj.keyPhase[i].keyPhaseId;
                        eq.actionId = action.actionId;
                        eq.isDeleted = 0;
                        _context.Add(eq);
                        _context.SaveChanges();
                    }


                }
                else
                {
                    OMA_MitigationAction action = await (from a in _context.OMA_MitigationAction.Where(a => a.actionId == reg.mitigationActionObj.mitigationAction.actionId)
                                                         select a).FirstOrDefaultAsync();
                    if (action != null)
                    {
                        action.actionTitle = reg.mitigationActionObj.mitigationAction.actionTitle;
                        action.programId = reg.mitigationActionObj.mitigationAction.programId;
                        action.priorityId = reg.mitigationActionObj.mitigationAction.priorityId;
                        action.comments = reg.mitigationActionObj.mitigationAction.comments;
                        action.objectiveOutcome = reg.mitigationActionObj.mitigationAction.objectiveOutcome;
                        //action.targetDate = reg.mitigationActionObj.mitigationAction.targetDate;
                        action.techAccountabilityId = reg.mitigationActionObj.mitigationAction.techAccountabilityId;

                        action.modifiedOn = DateTime.Now;
                        action.modifiedBy = reg.userId;
                        _context.SaveChanges();
                        _context.Database.ExecuteSqlCommand("DELETE FROM OMA_MitigationActionKeyPhases WHERE actionId = @tapID", new SqlParameter("tapID", reg.mitigationActionObj.mitigationAction.actionId));
                        for (var i = 0; i < reg.mitigationActionObj.keyPhase.Length; i++)
                        {
                            OMA_MitigationActionKeyPhases eq = new OMA_MitigationActionKeyPhases();
                            eq.keyPhaseId = reg.mitigationActionObj.keyPhase[i].keyPhaseId;
                            eq.actionId = action.actionId;
                            eq.isDeleted = 0;
                            _context.Add(eq);
                            _context.SaveChanges();
                        }
                    }
                }

                return Ok(reg.mitigationActionObj.mitigationAction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveMitigationResultReview")]
        public async Task<IActionResult> SaveMitigationResultReview(mitigationResultUserDto reg)
        {
            try
            {
                DateTime todayDate = DateTime.Now;
                var mr = await (from a in _context.OMA_MitigationResult.Where(a => a.actionId == reg.mitigationResult.actionId && a.siteId == reg.mitigationResult.siteId && a.technologyId == reg.mitigationResult.technologyId)
                                select a).FirstOrDefaultAsync();
                if (mr == null)
                {
                    OMA_MitigationResult mmr = new OMA_MitigationResult();

                    mmr.actionId = reg.mitigationResult.actionId;
                    mmr.siteId = reg.mitigationResult.siteId;
                    mmr.technologyId = reg.mitigationResult.technologyId;
                    mmr.targetDate = reg.mitigationResult.targetDate;
                    mmr.reviewerComment = reg.mitigationResult.reviewerComment;
                    if (mmr.targetDate > todayDate)
                    {
                        mmr.statusId = 3;
                    }
                    else
                    {
                        mmr.statusId = 6;
                    }
                    if (reg.mitigationResult.isReviewed == true)
                    {
                        mmr.isReviewed = 1;
                        mmr.statusId = 5;
                    }
                    else
                    {
                        mmr.isReviewed = 0;
                        //mmr.statusId = 1;
                    }
                    if (reg.mitigationResult.rework == true)
                    {
                        mmr.rework = 1;
                        mmr.statusId = 2;
                    }
                    else
                    {
                        mmr.rework = 0;

                    }
                    _context.Add(mmr);
                    _context.SaveChanges();
                    reg.mitigationResult.resultId = mmr.resultId;

                }
                else
                {
                    mr.targetDate = reg.mitigationResult.targetDate;
                    mr.reviewerComment = reg.mitigationResult.reviewerComment;
                    if (mr.targetDate > todayDate && mr.statusId == 6)
                    {
                        mr.statusId = 3;
                    }
                    if (reg.mitigationResult.isReviewed == true)
                    {
                        mr.isReviewed = 1;
                        mr.statusId = 5;
                    }
                    else
                    {
                        mr.isReviewed = 0;
                        //mr.statusId = 1;
                    }
                    if (reg.mitigationResult.rework == true)
                    {
                        mr.rework = 1;
                        mr.statusId = 2;
                    }
                    else
                    {
                        mr.rework = 0;

                    }
                    _context.SaveChanges();
                    reg.mitigationResult.resultId = mr.resultId;

                }




                return Ok(reg.mitigationResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveMitigationResult")]
        public async Task<IActionResult> SaveMitigationResult(mitigationResultUserDto reg)
        {
            try
            {

                var mr = await (from a in _context.OMA_MitigationResult.Where(a => a.actionId == reg.mitigationResult.actionId && a.siteId == reg.mitigationResult.siteId && a.technologyId == reg.mitigationResult.technologyId)
                                select a).FirstOrDefaultAsync();
                if (mr == null)
                {
                    OMA_MitigationResult mmr = new OMA_MitigationResult();
                    mmr.thirdPartyInterface = reg.mitigationResult.thirdPartyInterface;
                    if (reg.mitigationResult.tataInvolvement == true)
                    {
                        mmr.tataInvolvement = 1;
                    }
                    else
                    {
                        mmr.tataInvolvement = 0;
                    }
                    mmr.actionId = reg.mitigationResult.actionId;
                    mmr.siteId = reg.mitigationResult.siteId;
                    mmr.technologyId = reg.mitigationResult.technologyId;
                    mmr.statusId = (int)reg.mitigationResult.statusId;
                    //mmr.reviewerComment = reg.mitigationResult.reviewerComment;
                    mmr.actionComment = reg.mitigationResult.actionComment;
                    //if (reg.mitigationResult.isReviewed == true)
                    //{
                    //    mmr.isReviewed = 1;
                    //}
                    //else
                    //{
                    //    mmr.isReviewed = 0;
                    //}
                    //if (reg.mitigationResult.rework == true)
                    //{
                    //    mmr.rework = 1;
                    //}
                    //else
                    //{
                    //    mmr.rework = 0;
                    //}
                    _context.Add(mmr);
                    _context.SaveChanges();
                    reg.mitigationResult.resultId = mmr.resultId;
                }
                else
                {
                    mr.thirdPartyInterface = reg.mitigationResult.thirdPartyInterface;
                    if (reg.mitigationResult.tataInvolvement == true)
                    {
                        mr.tataInvolvement = 1;
                    }
                    else
                    {
                        mr.tataInvolvement = 0;
                    }
                    mr.statusId = (int)reg.mitigationResult.statusId;
                    mr.rework = 0;
                    //mr.reviewerComment = reg.mitigationResult.reviewerComment;
                    mr.actionId = reg.mitigationResult.actionId;
                    mr.siteId = reg.mitigationResult.siteId;
                    mr.actionComment = reg.mitigationResult.actionComment;
                    mr.technologyId = reg.mitigationResult.technologyId;
                    //if (reg.mitigationResult.rework == true)
                    //{
                    //    mr.rework = 1;
                    //}
                    //else
                    //{
                    //    mr.rework = 0;
                    //}
                    //if (reg.mitigationResult.isReviewed == true)
                    //{
                    //    mr.isReviewed = 1;
                    //}
                    //else
                    //{
                    //    mr.isReviewed = 0;
                }
                _context.SaveChanges();


                return Ok(reg.mitigationResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("editMitigationAction")]
        public async Task<IActionResult> EditMitigationAction(editMitigationDto reg)
        {
            try
            {

                var keyPhase = await (from u in _context.OMA_IKeyPhases.Where(a => a.isDeleted == 0)
                                      join mk in _context.OMA_MitigationActionKeyPhases.Where(a => a.actionId == reg.actionId) on u.keyPhaseId equals mk.keyPhaseId
                                      select new
                                      {
                                          u.keyPhaseId,
                                          u.keyPhaseTitle,
                                          u.keyPhaseCode
                                      }).ToListAsync();


                return Ok(keyPhase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteMitigationAction")]
        public async Task<IActionResult> DeleteMitigationAction(MititgationDTODelete reg)
        {
            try
            {

                OMA_MitigationAction action = await (from a in _context.OMA_MitigationAction.Where(a => a.actionId == reg.mitigationAction.actionId)
                                                     select a).FirstOrDefaultAsync();
                if (action != null)
                {
                    action.isDeleted = 1;
                    _context.SaveChanges();
                }


                return Ok(reg.mitigationAction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
