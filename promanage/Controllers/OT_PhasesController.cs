using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class OT_PhasesController : BaseAPIController
    {
        private readonly DAL _context;
        public OT_PhasesController(DAL context) 
        { 
            _context = context;
        }
        //Interface
        [Authorize]
        [HttpPost("getInterface")]
        public async Task<IActionResult> GetInterface(UserIdDto reg)
        {
            try
            {
                var owner = await (from a in _context.OT_IActionOwner.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                      a.actionOwnerId,
                                      a.actionOwnerTitle
                                    }).ToListAsync();
                return Ok(owner);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //Phases
        [Authorize]
        [HttpPost("getPhases")]
        public async Task<IActionResult> GetPhases(UserIdDto reg)
       {
            try
            {
                var phases = await (from a in _context.OT_Phase.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.phaseId,
                                        a.phaseNumber,
                                        a.phaseTitle,
                                        a.phaseDescription,
                                    }).OrderBy(a=>a.phaseNumber).ToListAsync();
                return Ok(phases);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSelectedOutages")]
        public async Task<IActionResult> GetSelectedOutages(GetSelectedOutagesDto reg)
        {
            try
            {
                var selectedOutages = await (from a in _context.OT_ISiteOutages
                                             join d in _context.OT_PhaseDuration.Where(a => a.phaseId == reg.phaseId) on a.outageId equals d.outageId into all
                                             from dd in all.DefaultIfEmpty()
                                             select new
                                             {                                                
                                                 a.outageId,
                                                 a.outageTitle,
                                                 phaseId = dd.phaseId == null?-1: dd.phaseId,
                                                 phaseDurId = dd.phaseDurId == null?-1:dd.phaseId,
                                                 dd.durationMonths,
                                             }).ToListAsync();
                return Ok(selectedOutages);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("savePhases")]
        public async Task<IActionResult> SavePhases(SaveOT_PhaseUserDto reg)
        {
            try
            {
                if(reg.phase.phaseId == -1)
                {
                    OT_Phase phase = new OT_Phase();
                    phase.phaseNumber = Convert.ToInt32(reg.phase.phaseNumber);
                    phase.phaseTitle = reg.phase.phaseTitle;
                    phase.phaseDescription= reg.phase.phaseDescription;
                    phase.createdBy = reg.userId;
                    phase.createdOn = DateTime.Now;
                    _context.Add(phase);
                    _context.SaveChanges();
                    reg.phase.phaseId = phase.phaseId;
                }
                else
                {
                    var phase = await (from a in _context.OT_Phase.Where(a => a.phaseId == reg.phase.phaseId)
                                        select a).FirstOrDefaultAsync();
                    if(phase == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        phase.phaseNumber = Convert.ToInt32(reg.phase.phaseNumber);
                        phase.phaseTitle = reg.phase.phaseTitle;
                        phase.phaseDescription = reg.phase.phaseDescription;
                        phase.modifiedBy = reg.userId;
                        phase.modifiedOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                }

                return Ok(reg.phase);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deletePhases")]
        public async Task<IActionResult> DeletePhases(SaveOT_PhaseUserDto reg)
        {
            try
            {
                var phase = await (from a in _context.OT_Phase.Where(a => a.phaseId == reg.phase.phaseId)
                                   select a).FirstOrDefaultAsync();
                if(phase == null)
                {
                    return NotFound();
                }
                else
                {
                    phase.isDeleted = 1;
                    _context.SaveChanges();
                }
                return Ok(reg.phase);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Durations
        [Authorize]
        [HttpPost("saveDurations")]
        public async Task<IActionResult> SaveDurations(SaveOT_PhaseDurationUserDto reg)
        {
            try
            {
                
                _context.Database.ExecuteSqlCommand("DELETE FROM OT_PhaseDuration WHERE phaseId = @tapId", new SqlParameter("tapId", Convert.ToInt32(reg.PhaseDuration[0].phaseId)));
                for(var a = 0; a<reg.PhaseDuration.Length; a++)
                {
                    OT_PhaseDuration pd = new OT_PhaseDuration();
                    pd.phaseId = reg.PhaseDuration[a].phaseId;
                    pd.outageId= reg.PhaseDuration[a].outageId;
                    pd.durationMonths  = reg.PhaseDuration[a].durationMonths;
                    pd.createdOn = DateTime.Now;
                    pd.createdBy = reg.userId;
                    _context.Add(pd);
                    _context.SaveChanges();
                }


                return Ok(reg.PhaseDuration);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getDurations")]
        public async Task<IActionResult> GetDurations(GetOT_DurationDto reg)
        {
            try
            {
                var duration = await(from a in _context.SiteNextOutages
                                     select new
                                     {

                                     }).ToListAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Description
        [Authorize]
        [HttpPost("getSelectedOutageOwners")]
        public async Task<IActionResult> GetSelectedOutageOwners(GetSelectedSiteOwner reg)
        {
            try
            {
                var phases = await (from a in _context.OT_PhaseReadinessDescriptionAO.Where(a => a.phaseReadId == reg.phaseReadId)
                                    join b in _context.OT_IActionOwner.Where(a=>a.isDeleted ==0) on a.actionOwnerId equals b.actionOwnerId
                                    select new
                                    {
                                        a.actionOwnerId,
                                        b.actionOwnerTitle
                                    }).ToListAsync();
                return Ok(phases);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getOutageRediness")]
        public async Task<IActionResult> GetOutageRediness(UserIdDto reg)
        {
            try
            {
                var phases = await (from a in _context.OT_PhaseReadinessDescription.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.phaseId,
                                        a.phaseReadId,
                                        a.phaseReadDesc,
                                        a.phaseReadNum,
                                    }).OrderBy(a=>a.phaseReadNum).ToListAsync();
                return Ok(phases);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteOutageRediness")]
        public async Task<IActionResult> DeleteOutageRediness(DeleteOT_PhaseDescUserDto reg)
        {
            try
            {
                var phase = await (from a in _context.OT_PhaseReadinessDescription.Where(a => a.phaseReadId == reg.PhaseDesc.phaseReadId)
                                   select a).FirstOrDefaultAsync();
                if (phase == null)
                {
                    return NotFound();
                }
                else
                {
                    phase.isDeleted = 1;
                    _context.SaveChanges();
                }
                return Ok(reg.PhaseDesc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveOutageRediness")]
        public async Task<IActionResult> SaveOutageRediness(SaveOT_PhaseDescUserDto reg)
        {
            try
            {
                if(reg.PhaseDesc.outageReadDesc.phaseReadId == -1)
                {
                    OT_PhaseReadinessDescription rd = new OT_PhaseReadinessDescription();
                    rd.phaseReadNum = reg.PhaseDesc.outageReadDesc.phaseReadNum;
                    rd.phaseId = reg.PhaseDesc.outageReadDesc.phaseId;
                    rd.phaseReadDesc= reg.PhaseDesc.outageReadDesc.phaseReadDesc;
                    rd.createdOn = DateTime.Now;
                    rd.createdBy = reg.userId;
                    _context.Add(rd);
                    _context.SaveChanges();
                    for(var a= 0; a<reg.PhaseDesc.ownerList.Length; a++)
                    {
                        OT_PhaseReadinessDescriptionAO tr = new OT_PhaseReadinessDescriptionAO();
                        tr.phaseReadId = rd.phaseReadId;
                        tr.actionOwnerId = reg.PhaseDesc.ownerList[a].actionOwnerId;
                        _context.Add(tr);
                        _context.SaveChanges();
                    }

                }
                else
                {
                    var rd = await(from a in _context.OT_PhaseReadinessDescription.Where(a=>a.phaseReadId == reg.PhaseDesc.outageReadDesc.phaseReadId)
                                   select a).FirstAsync();
                    rd.phaseReadNum = reg.PhaseDesc.outageReadDesc.phaseReadNum;
                    rd.phaseReadDesc = reg.PhaseDesc.outageReadDesc.phaseReadDesc;
                    rd.phaseId = reg.PhaseDesc.outageReadDesc.phaseId;

                    rd.modifiedOn = DateTime.Now;
                    rd.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM OT_PhaseReadinessDescriptionAO WHERE phaseReadId = @siteId", new SqlParameter("siteId", reg.PhaseDesc.outageReadDesc.phaseReadId));
                    for (var a = 0; a < reg.PhaseDesc.ownerList.Length; a++)
                    {
                        OT_PhaseReadinessDescriptionAO tr = new OT_PhaseReadinessDescriptionAO();
                        tr.phaseReadId = rd.phaseReadId;
                        tr.actionOwnerId = reg.PhaseDesc.ownerList[a].actionOwnerId;
                        _context.Add(tr);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.PhaseDesc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
