using ActionTrakingSystem.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using ActionTrakingSystem.Model;
using System.Linq;
using System.Collections.Generic;

namespace ActionTrakingSystem.Controllers
{

    public class ChemistryActionPlanActionController : BaseAPIController
    {
        private readonly DAL _context;
        public ChemistryActionPlanActionController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getActionsReport")]
        public async Task<IActionResult> GetActionsReport(UserIdDto reg)
        {

            try
       {
                var actions = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0)
                                     join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0) on o.wscId equals ac.obsId
                                     join s in _context.Sites.Where(a => a.isDeleted == 0) on o.plantId equals s.siteId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join d in _context.CAP_IWSCDept on ac.deptId equals d.wscDeptId into all1
                                     from dd in all1.DefaultIfEmpty()
                                     join p in _context.CAP_IWSCPriority on ac.priorityId equals p.wscPriorityId into all2
                                     from pp in all2.DefaultIfEmpty()
                                     join u in _context.CAP_IWSCStatus on ac.statusId equals u.wscStatusId into all3
                                     from uu in all3.DefaultIfEmpty()
                                     join au in _context.AppUser on ac.userId equals au.userId
                                     select new
                                     {
                                         s.siteName,
                                         s.siteId,
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

                                     }
                                         ).Distinct().ToListAsync();
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
                var appUser = await (from d in _context.AppUser.Where(a => a.isDeleted == 0)
                                     join aus in _context.AUSite on d.userId equals aus.userId
                                     join s in _context.Sites on aus.siteId equals s.siteId
                                     join o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0) on s.siteId equals o.plantId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aut in _context.AUTechnology on stech.techId equals aut.technologyId
                                     select new
                                     {
                                         userName = d.userName,
                                         userId = d.userId,
                                         email = d.email,
                                         fullName = d.lastName + " " + d.middleName + " " + d.lastName
                                     }).Distinct().ToListAsync();
                var observation = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0)
                                         join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0) on o.wscId equals ac.obsId

                                         select new
                                         {
                                             observationId = o.wscId,
                                             observationTitle = o.observation,
                                         }).ToListAsync();
                var obj = new
                {
                    actions,
                    status,
                    sites,
                    regions,
                    priority,
                    observation,
                    dept,
                    appUser
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getFilterActionsReport")]
        public async Task<IActionResult> GetFilterActionsReport(CAPActionFilterUserDto reg)
        {
            List<int> SiteIds = new List<int>();
            if (!string.IsNullOrEmpty(reg.siteList))
                SiteIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

            List<int> RegionIds = new List<int>();
            if (!string.IsNullOrEmpty(reg.regionList))
                RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());
            List<int> statusIds = new List<int>();
            if (!string.IsNullOrEmpty(reg.statusList))
                statusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());
            try
            {
                var actions = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0 && (SiteIds.Count == 0) || SiteIds.Contains((int)a.plantId))
                                     join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0 && (statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) on o.wscId equals ac.obsId
                                     join s in _context.Sites.Where(a => a.isDeleted == 0 &&  (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on o.plantId equals s.siteId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join d in _context.CAP_IWSCDept on ac.deptId equals d.wscDeptId into all1
                                     from dd in all1.DefaultIfEmpty()
                                     join p in _context.CAP_IWSCPriority on ac.priorityId equals p.wscPriorityId into all2
                                     from pp in all2.DefaultIfEmpty()
                                     join u in _context.CAP_IWSCStatus on ac.statusId equals u.wscStatusId into all3
                                     from uu in all3.DefaultIfEmpty()
                                     join au in _context.AppUser on ac.userId equals au.userId
                                     select new
                                     {
                                         s.siteName,
                                         s.siteId,
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

                                     }
                                         ).Distinct().ToListAsync();
                var sites = await (from r in _context.Regions.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId))
                                   join s in _context.Sites.Where(a => a.isDeleted == 0) on r.regionId equals s.regionId
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }).Distinct().ToListAsync();
                var obj = new { actions, sites };


                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getActions")]
        public async Task<IActionResult> GetActions(UserIdDto reg)
        {

            try
            {
                var todayDate = DateTime.Now;
                var actions = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0)
                                     join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0 && (a.userId == reg.userId || a.createdBy == reg.userId))on o.wscId equals ac.obsId
                                     join s in _context.Sites.Where(a => a.isDeleted == 0) on o.plantId equals s.siteId
                                     join r in _context.Regions.Where(a => a.isDeleted == 0 ) on s.regionId equals r.regionId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join d in _context.CAP_IWSCDept on ac.deptId equals d.wscDeptId into all1
                                     from dd in all1.DefaultIfEmpty()
                                     join p in _context.CAP_IWSCPriority on ac.priorityId equals p.wscPriorityId into all2
                                     from pp in all2.DefaultIfEmpty()
                                     join u in _context.CAP_IWSCStatus on ac.statusId equals u.wscStatusId into all3
                                     from uu in all3.DefaultIfEmpty()
                                     join au in _context.AppUser on ac.userId equals au.userId
                                     select new
                                     {
                                         plantId = o.plantId,
                                         plantTitle = s.siteName,
                                         regionId = s.regionId,
                                         regionTitle = r.title,
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
                                         actionAdmin = ac.createdBy,
                                     }
                                         ).Distinct().ToListAsync();
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
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }).Distinct().OrderBy(o => o.siteTitle).ToListAsync();

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
                var appUser = await (from d in _context.AppUser.Where(a => a.isDeleted == 0)
                                     join aus in _context.AUSite on d.userId equals aus.userId
                                     join s in _context.Sites on aus.siteId equals s.siteId
                                     join o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0) on s.siteId equals o.plantId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aut in _context.AUTechnology on stech.techId equals aut.technologyId
                                     select new
                                     {
                                         userName = d.userName,
                                         userId = d.userId,
                                         email = d.email,
                                         fullName = d.lastName + " " + d.middleName + " " + d.lastName
                                     }).Distinct().OrderBy(o => o.userName).ToListAsync();

                var observation = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0)
                                         join s in _context.Sites.Where(a => a.isDeleted == 0 ) on o.plantId equals s.siteId
                                         join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                         join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                         join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                         join r in _context.Regions.Where(a => a.isDeleted == 0 ) on s.regionId equals r.regionId
                                         join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0) on o.wscId equals ac.obsId into all5
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
                var obj = new
                {
                    actions,
                    status,
                    sites,
                    regions,
                    priority,
                    observation,
                    dept,
                    appUser
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("filterAction")]
        public async Task<IActionResult> FilterAction(CAPActionFilterUserDto reg)
        {

            try
            {
                List<int> SiteIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SiteIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());

                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());
                List<int> statusIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.statusList))
                    statusIds = (reg.statusList.Split(',').Select(Int32.Parse).ToList());
                var actions = await (from o in _context.CAP_WSCObservation.Where(a => a.isDeleted == 0 && (SiteIds.Count == 0) || SiteIds.Contains((int)a.plantId))
                                     join ac in _context.CAP_IWSCActions.Where(a => a.isDeleted == 0 && (a.userId == reg.userId || a.createdBy == reg.userId) && (statusIds.Count == 0) || statusIds.Contains((int)a.statusId)) on o.wscId equals ac.obsId
                                     join s in _context.Sites.Where(a => a.isDeleted == 0 && (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on o.plantId equals s.siteId
                                     join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                     join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                     join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                     join d in _context.CAP_IWSCDept on ac.deptId equals d.wscDeptId into all1
                                     from dd in all1.DefaultIfEmpty()
                                     join p in _context.CAP_IWSCPriority on ac.priorityId equals p.wscPriorityId into all2
                                     from pp in all2.DefaultIfEmpty()
                                     join u in _context.CAP_IWSCStatus on ac.statusId equals u.wscStatusId into all3
                                     from uu in all3.DefaultIfEmpty()
                                     join au in _context.AppUser on ac.userId equals au.userId
                                     select new
                                     {
                                         s.siteId,
                                         s.siteName,
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

                                     }
                                         ).Distinct().ToListAsync();

                var sites = await (from r in _context.Regions.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId))
                                   join s in _context.Sites.Where(a => a.isDeleted == 0) on r.regionId equals s.regionId
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }).Distinct().ToListAsync();
                var obj = new { actions, sites };

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

                        ins.remarks = reg.action.remarks;
                        ins.statusId = reg.action.statusId;
                        ins.suggestion = reg.action.suggestion;
                        ins.startDate = reg.action.startDate;
                        if (ins.statusId == 2)
                        {
                            ins.closureDate = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    CAP_IWSCActions ins = new CAP_IWSCActions();

                    ins.remarks = reg.action.remarks;
                    ins.statusId = reg.action.statusId;
                    ins.suggestion = reg.action.suggestion;
                    ins.startDate = reg.action.startDate;
                    if (ins.statusId == 2)
                    {
                        ins.closureDate = DateTime.Now;
                    }
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
    }
}
