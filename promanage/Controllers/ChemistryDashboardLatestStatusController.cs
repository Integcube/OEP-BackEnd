using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;

namespace ActionTrakingSystem.Controllers
{

    public class ChemistryDashboardLatestStatusController : BaseAPIController
    {
        private readonly DAL _context;
        public ChemistryDashboardLatestStatusController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("savelatestStatus")]
        public async Task<IActionResult> SaveAction(chemistryLatestUser reg)
        {
            try
            {
                if (reg.latestStatus.updateId != -1)
                {
                    CAP_LatestUpdate ins = await (from i in _context.CAP_LatestUpdate.Where(a => a.updateId == reg.latestStatus.updateId)
                                                 select i).FirstOrDefaultAsync();
                    if (ins != null)
                    {

                        ins.remarks = reg.latestStatus.remarks;
                        if (reg.latestStatus.modelId == 1 && reg.latestStatus.liveApplication == "Yes")
                        {
                            ins.statusId = 2;
                        }
                        else if (reg.latestStatus.modelId == 2 && reg.latestStatus.liveApplication == "No")
                        {
                            ins.statusId = 7;
                        }
                        else if (reg.latestStatus.modelId == 1 && reg.latestStatus.liveApplication == "No")
                        {
                            ins.statusId = 9;
                        }
                        else
                        {
                            ins.statusId = 9;
                        }
                        ins.modelId = reg.latestStatus.modelId;
                        ins.liveApplication = reg.latestStatus.liveApplication;
                        ins.siteId = reg.latestStatus.siteId;
                        ins.target = reg.latestStatus.target;
                        ins.modifiedBy = reg.userId;
                        ins.modifiedOn = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    CAP_LatestUpdate ins = new CAP_LatestUpdate();

                    ins.remarks = reg.latestStatus.remarks;
                    if (reg.latestStatus.modelId == 1 && reg.latestStatus.liveApplication == "Yes")
                    {
                        ins.statusId = 2;
                    }
                    else if (reg.latestStatus.modelId == 2 && reg.latestStatus.liveApplication == "No")
                    {
                        ins.statusId = 7;
                    }
                    else if (reg.latestStatus.modelId == 1 && reg.latestStatus.liveApplication == "No")
                    {
                        ins.statusId = 9;
                    }
                    else
                    {
                        ins.statusId = 9;
                    }
                    ins.target = reg.latestStatus.target;
                    ins.modelId = reg.latestStatus.modelId;
                    ins.liveApplication = reg.latestStatus.liveApplication;
                    ins.siteId = reg.latestStatus.siteId;
                    ins.createdBy = reg.userId;
                    ins.createdOn = DateTime.Now;
                    _context.Add(ins);
                    await _context.SaveChangesAsync();
                    reg.latestStatus.updateId = ins.updateId;
                    reg.latestStatus.statusId = ins.statusId;
                }
                var latestStatus = await (from ap in _context.CAP_LatestUpdate.Where(a => a.isDeleted == 0)
                                          join r in _context.Sites on ap.siteId equals r.siteId
                                          join s in _context.CAP_LatestUpdateStatus on ap.statusId equals s.lusId
                                          join u in _context.CAP_ModelStatus on ap.modelId equals u.modelId
                                          select new
                                          {
                                              ap.target,
                                              ap.updateId,
                                              ap.siteId,
                                              siteTitle = r.siteName,
                                              ap.remarks,
                                              ap.liveApplication,
                                              ap.modelId,
                                              modelStatus = u.status,
                                              ap.statusId,
                                              s.lusId,
                                              status = s.status == "Completed" ? s.status : ap.target,
                                              s.score,
                                          }).OrderByDescending(a => a.updateId).ToListAsync();
                return Ok(latestStatus);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deletelatestStatus")]
        public async Task<IActionResult> DeleteObservation(chemistryLatestUser reg)
        {
            try
            {
                CAP_LatestUpdate obs = await (from r in _context.CAP_LatestUpdate.Where(a => a.updateId == reg.latestStatus.updateId)
                                                select r).FirstOrDefaultAsync();
                if (obs != null)
                {
                    obs.isDeleted = 1;
                    await _context.SaveChangesAsync();
                }

                return Ok(reg.latestStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getLatestStatus")]
        public async Task<IActionResult> GetLatestStatus(UserIdDto reg)
        {
            try
            {
                var latestStatus = await (from ap in _context.CAP_LatestUpdate.Where(a => a.isDeleted == 0)
                                   join r in _context.Sites on ap.siteId equals r.siteId
                                   join s in _context.CAP_LatestUpdateStatus on ap.statusId equals s.lusId
                                   join u in _context.CAP_ModelStatus on ap.modelId equals u.modelId
                                   select new
                                   {
                                       ap.target,
                                       ap.updateId,
                                       ap.siteId,
                                       siteTitle = r.siteName,
                                       ap.remarks,
                                       ap.liveApplication,
                                       ap.modelId,
                                       modelStatus = u.status,
                                      ap.statusId,
                                       s.lusId,
                                       status=s.status == "Completed"?s.status: ap.target,
                                       s.score,
                                   }).OrderByDescending(a => a.updateId).ToListAsync();
                var status = await (from a in _context.CAP_LatestUpdateStatus
                                    select new
                                    {
                                        a.score,
                                        a.status,
                                        a.lusId
                                    }).ToListAsync();
                var model = await (from b in _context.CAP_ModelStatus
                                   select new
                                   {
                                       b.modelId,
                                       b.status
                                   }).ToListAsync();
                var site = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                  join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                  join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                  join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                  select new
                                  {
                                      s.siteId,
                                      siteTitle = s.siteName
                                  }
                                  ).Distinct().ToListAsync();
                var obj = new
                {
                    latestStatus,
                    status,
                    model,
                    site
                };
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
