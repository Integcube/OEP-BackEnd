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
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class ChemistryActionPlanController : BaseAPIController
    {
        private readonly DAL _context;
        public ChemistryActionPlanController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getActions")]
        public async Task<IActionResult> GetActions(CAPIdsDto reg)
        {
            try
            {
               var todayDate = DateTime.Now;

                var actions = await (from o in _context.CAP_WSCObservation.Where(a => a.wscId == reg.observationId)
                                     join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0 && (a.wscActionId == reg.actionId || reg.actionId ==-1)) on o.wscId equals ac.obsId
                                     join d in _context.CAP_IWSCDept on ac.deptId equals d.wscDeptId into all1
                                     from dd in all1.DefaultIfEmpty()
                                     join p in _context.CAP_IWSCPriority on ac.priorityId equals p.wscPriorityId into all2
                                     from pp in all2.DefaultIfEmpty()
                                     join u in _context.CAP_IWSCStatus on ac.statusId equals u.wscStatusId into all3
                                     from uu in all3.DefaultIfEmpty()
                                     join au in _context.AppUser on ac.userId equals au.userId
                                     select new
                                     {
                                         observationId = o.wscId,
                                         observationTitle = o.observation,
                                         actionId = ac.wscActionId,
                                         actionTitle = ac.action,
                                         priorityId = ac.priorityId,
                                         priorityTitle = pp.priorityTitle,
                                         startDate = ac.startDate,
                                         targetDate = ac.targetDate,
                                         remarks = ac.remarks,
                                         deptId = ac.deptId,
                                         deptTitle = dd.deptTitle,
                                         userId = ac.userId,
                                         userName = au.userName,
                                         statusId = ac.statusId,
                                         statusTitle = uu.statusTitle,
                                         referenceNumber = ac.referenceNumber,
                                         suggestion = ac.suggestion,
                                         closureDate = ac.closureDate,
                                         closureStatus = ac.closureDate!=null?(ac.closureDate > ac.targetDate && ac.statusId ==1 ?1:0): (todayDate > ac.targetDate && ac.statusId == 1 ? 1 : 0)
                                     }
                                         ).FirstOrDefaultAsync();
                var dept = await (from d in _context.CAP_IWSCDept.Where(a => a.isDeleted == 0)
                                  select new
                                  {
                                      deptId = d.wscDeptId,
                                      deptTitle = d.deptTitle,
                                  }).ToListAsync();
                var priority = await (from d in _context.CAP_IWSCPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          priorityId = d.wscPriorityId,
                                          priorityTitle = d.priorityTitle,
                                      }).ToListAsync();
                var status = await (from d in _context.CAP_IWSCStatus.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        statusId = d.wscStatusId,
                                        statusTitle = d.statusTitle,
                                    }).ToListAsync();
                var appUser = await (from d in _context.AppUser.Where(a => a.isDeleted == 0)
                                     join aus in _context.AUSite on d.userId equals aus.userId
                                     join s in _context.Sites on aus.siteId equals s.siteId
                                     join o in _context.CAP_WSCObservation.Where(a => a.wscId == reg.observationId) on s.siteId equals o.plantId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aut in _context.AUTechnology on stech.techId equals aut.technologyId
                                     select new
                                     {
                                         userName = d.userName,
                                         userId = d.userId,
                                         email = d.email,
                                         fullName = d.lastName + " " + d.middleName + " " + d.lastName
                                     }).Distinct().ToListAsync();
                var observation = await (from o in _context.CAP_WSCObservation.Where(a => a.wscId == reg.observationId)
                                         select new
                                         {
                                             observationId = o.wscId,
                                             observationTitle = o.observation,
                                         }).FirstOrDefaultAsync();
                var obj = new
                {
                    actions,
                    dept,
                    priority,
                    status,
                    appUser,
                    observation
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveAction")]
        public async Task<IActionResult> SaveAction(CAPActionUserDto reg)
        {
            try
            {
                if (reg.action.actionId != -1)
                {
                    CAP_IWSCActions ins = await (from i in _context.CAP_IWSCActions.Where(a => a.wscActionId == reg.action.actionId)
                                                 select i).FirstOrDefaultAsync();
                    if (ins != null)
                    {
                        ins.action = reg.action.actionTitle;
                        ins.obsId = reg.action.observationId;
                        ins.priorityId = reg.action.priorityId;
                        ins.targetDate = reg.action.targetDate;
                        //ins.remarks=reg.action.remarks;
                        ins.deptId = reg.action.deptId;
                        ins.userId = reg.action.userId;
                        //ins.statusId=reg.action.statusId;
                        ins.referenceNumber = reg.action.referenceNumber;
                        //ins.suggestion=reg.action.suggestion;
                        ins.modifiedDate = DateTime.Now;
                        ins.modifiedBy = reg.userId;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    CAP_IWSCActions ins = new CAP_IWSCActions();

                    ins.action = reg.action.actionTitle;
                    ins.obsId = reg.action.observationId;
                    ins.priorityId = reg.action.priorityId;
                    ins.targetDate = reg.action.targetDate;
                    //ins.remarks = reg.action.remarks;
                    ins.deptId = reg.action.deptId;
                    ins.userId = reg.action.userId;
                    //ins.statusId = reg.action.statusId;
                    ins.referenceNumber = reg.action.referenceNumber;
                    //ins.suggestion = reg.action.suggestion;
                    ins.createdDate = DateTime.Now;
                    ins.createdBy = reg.userId;
                    _context.Add(ins);
                    await _context.SaveChangesAsync();
                    reg.action.actionId = ins.wscActionId;
                }

                return Ok(reg.action);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
  
        [HttpPost("getObservations")]
        public async Task<IActionResult> GetObservations(CAPGetObservation reg)
        {
            try
            {
                var todayDate = DateTime.Now;
                List<int> SiteIds = new List<int>();
              if (!string.IsNullOrEmpty(reg.siteList))
                    SiteIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                var observation = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0)
                                         join s in _context.Sites.Where(a => a.isDeleted == 0&& (SiteIds.Count == 0) || SiteIds.Contains(a.siteId)) on o.plantId equals s.siteId
                                         join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                         join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                         join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                         join r in _context.Regions.Where(a => a.isDeleted == 0 && (RegionIds.Count == 0) || RegionIds.Contains(a.regionId)) on s.regionId equals r.regionId
                                         join ac in _context.CAP_IWSCActions.Where(a=>a.isDeleted == 0) on o.wscId equals ac.obsId into all5
                                         from acc in all5.DefaultIfEmpty()
                                         join d in _context.CAP_IWSCDept on acc.deptId equals d.wscDeptId into all1
                                         from dd in all1.DefaultIfEmpty()
                                         join p in _context.CAP_IWSCPriority on acc.priorityId equals p.wscPriorityId into all2
                                         from pp in all2.DefaultIfEmpty()
                                         join u in _context.CAP_IWSCStatus on acc.statusId equals u.wscStatusId into all3
                                         from uu in all3.DefaultIfEmpty()
                                         join au in _context.AppUser on acc.userId equals au.userId
                                         select new
                                         {
                                             observationId = o.wscId,
                                             observationTitle = o.observation,
                                             plantId = o.plantId,
                                             plantTitle = s.siteName,
                                             regionId = s.regionId,
                                             regionTitle = r.title,
                                             o.createdDate,
                                             actionId = acc.wscActionId,
                                             actionTitle = acc.action,
                                             priorityId = acc.priorityId,
                                             priorityTitle = pp.priorityTitle,
                                             startDate = acc.startDate,
                                             targetDate = acc.targetDate,
                                             remarks = acc.remarks,
                                             deptId = acc.deptId,
                                             deptTitle = dd.deptTitle,
                                             userId = acc.userId,
                                             userName = au.userName,
                                             statusId = acc.statusId,
                                             statusTitle = uu.statusTitle,
                                             referenceNumber = acc.referenceNumber,
                                             suggestion = acc.suggestion,
                                             closureDate = acc.closureDate,
                                             closureStatus = acc.closureDate != null ? (acc.closureDate > acc.targetDate && acc.statusId == 1 ? 1 : 0) : (todayDate > acc.targetDate && acc.statusId == 1 ? 1 : 0)

                                         }
                                         ).Distinct().OrderByDescending(a => a.createdDate).ToListAsync();

                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }).Distinct().ToListAsync();
                var regions = await (from r in _context.Regions.Where(a => a.isDeleted == 0)
                                          join s in _context.Sites on r.regionId equals s.regionId
                                          join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                          join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                          join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                          select new
                                          {
                                              regionTitle = r.title,
                                              r.regionId,
                                          }).Distinct().ToListAsync();
                var dept = await (from d in _context.CAP_IWSCDept.Where(a => a.isDeleted == 0)
                                  select new
                                  {
                                      deptId = d.wscDeptId,
                                      deptTitle = d.deptTitle,
                                  }).ToListAsync();
                var priority = await (from d in _context.CAP_IWSCPriority.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          priorityId = d.wscPriorityId,
                                          priorityTitle = d.priorityTitle,
                                      }).ToListAsync();
                var status = await (from d in _context.CAP_IWSCStatus.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        statusId = d.wscStatusId,
                                        statusTitle = d.statusTitle,
                                    }).ToListAsync();
                var appUser = await (from d in _context.AppUser.Where(a => a.isDeleted == 0)
                                     join aus in _context.AUSite on d.userId equals aus.userId
                                     join s in _context.Sites on aus.siteId equals s.siteId
                                     join o in _context.CAP_WSCObservation on s.siteId equals o.plantId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aut in _context.AUTechnology on stech.techId equals aut.technologyId
                                     select new
                                     {
                                         userName = d.userName,
                                         userId = d.userId,
                                         email = d.email,
                                         fullName = d.lastName + " " + d.middleName + " " + d.lastName
                                     }).Distinct().OrderBy(a=>a.userName).ToArrayAsync();

                var obj = new
                {
                    observation,
                    sites,
                    regions,
                    dept,
                    priority,
                    status,
                    appUser,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveObservations")]
        public async Task<IActionResult> SaveObservation(CAPObservationUserDto reg)
        {
            try
            {
                if (reg.observation.observationId != -1)
                {
                    CAP_WSCObservation ins = await (from i in _context.CAP_WSCObservation.Where(a => a.wscId == reg.observation.observationId)
                                                    select i).FirstOrDefaultAsync();
                    if (ins != null)
                    {
                        ins.observation = reg.observation.observationTitle;
                        ins.plantId = reg.observation.plantId;
                        ins.modifiedDate = DateTime.Now;
                        ins.modifiedBy = reg.userId;
                         _context.SaveChanges();
                        CAP_IWSCActions inss = await (from i in _context.CAP_IWSCActions.Where(a => a.wscActionId == reg.observation.actionId)
                                                     select i).FirstOrDefaultAsync();
                        if (inss != null)
                        {
                            inss.action = reg.observation.actionTitle;
                            inss.obsId = reg.observation.observationId;
                            inss.priorityId = reg.observation.priorityId;
                            inss.targetDate = reg.observation.targetDate;
                            
                            inss.deptId = reg.observation.deptId;
                            
                           
                            inss.userId = reg.observation.userId;
                           
                           
                            inss.referenceNumber = reg.observation.referenceNumber;
                            if (reg.observation.startDate != null)
                            {
                                inss.startDate = reg.observation.startDate;
                            }
                            if (reg.observation.statusId != null)
                            {
                                inss.statusId = reg.observation.statusId;
                            }
                            else
                            {
                                inss.statusId = 1;
                            }
                            if (reg.observation.suggestion != "")
                            {
                                inss.suggestion = reg.observation.suggestion;
                            }
                            if (reg.observation.remarks != "")
                            {
                                inss.remarks = reg.observation.remarks;
                            }
                            inss.modifiedDate = DateTime.Now;
                            inss.modifiedBy = reg.userId;
                             _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    CAP_WSCObservation ins = new CAP_WSCObservation();

                    ins.observation = reg.observation.observationTitle;
                    ins.plantId = reg.observation.plantId;
                    ins.createdDate = DateTime.Now;
                    ins.createdBy = reg.userId;
                    _context.Add(ins);
                     _context.SaveChanges();
                    reg.observation.observationId = ins.wscId;
                    CAP_IWSCActions inss = new CAP_IWSCActions();

                    inss.action = reg.observation.actionTitle;
                    inss.obsId = ins.wscId;
                    inss.priorityId = reg.observation.priorityId;
                    inss.targetDate = reg.observation.targetDate;
                    inss.deptId = reg.observation.deptId;
                    inss.userId = reg.observation.userId;
                    inss.referenceNumber = reg.observation.referenceNumber;
                    if (reg.observation.startDate != null)
                    {
                        inss.startDate = reg.observation.startDate;
                    }
                    if (reg.observation.statusId != null)
                    {
                        inss.statusId = reg.observation.statusId;
                    }
                    else
                    {
                        inss.statusId = 1;
                    }
                    if (reg.observation.suggestion != "")
                    {
                        inss.suggestion = reg.observation.suggestion;
                    }
                    if (reg.observation.remarks != "")
                    {
                        inss.remarks = reg.observation.remarks;
                    }
                    inss.createdDate = DateTime.Now;
                    inss.createdBy = reg.userId;
                    _context.Add(inss);
                     _context.SaveChanges();
                    reg.observation.actionId = inss.wscActionId;
                }

                return Ok(reg.observation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteObservations")]
        public async Task<IActionResult> DeleteObservation(CAPObservationUserDto reg)
        {
            try
            {
                CAP_WSCObservation obs = await (from r in _context.CAP_WSCObservation.Where(a => a.wscId == reg.observation.observationId)
                                                select r).FirstOrDefaultAsync();
                if (obs != null)
                {
                    obs.isDeleted = 1;
                    await _context.SaveChangesAsync();
                }

                return Ok(reg.observation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteAction")]
        public async Task<IActionResult> DeleteAction(CAPActionUserDto reg)
        {
            try
            {
                CAP_IWSCActions act = await (from r in _context.CAP_IWSCActions.Where(a => a.wscActionId == reg.action.actionId)
                                             select r).FirstOrDefaultAsync();
                if (act != null)
                {
                    act.isDeleted = 1;
                    await _context.SaveChangesAsync();
                }

                return Ok(reg.action);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSites")]
        public async Task<IActionResult> GetSites(CAPRegionUserDto reg)
        {
            try
            {
                var sites = await (from r in _context.Regions.Where(a => a.regionId == reg.regionId)
                                   join s in _context.Sites.Where(a => a.isDeleted == 0) on r.regionId equals s.regionId
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }).Distinct().ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
