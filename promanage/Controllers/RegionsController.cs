using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using ActionTrakingSystem.DTOs;
using Microsoft.Data.SqlClient;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly DAL _context;
        public RegionsController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("getRegions")]
        public async Task<ActionResult<IEnumerable<Regions>>> GetRegions()
        {
            try
            {
                var regions = await (from r in _context.Regions.Where(a => a.isDeleted == 0)
                                     join us in _context.AppUser on r.executiveDirector equals us.userId into all2
                                     from ed in all2.DefaultIfEmpty()
                                     join rvp in _context.RegionsExecutiveVp on r.regionId equals rvp.regionId into all3
                                     from vp in all3.DefaultIfEmpty()
                                     join u in _context.AppUser on vp.userId equals u.userId into all1
                                     from vpu in all1.DefaultIfEmpty()
                                     select new
                                     {
                                         regionId =r.regionId,
                                         title = r.title,
                                         TILsSummary = r.TILsSummary,
                                         insuranceSummary = r.insuranceSummary,
                                         executiveDirectorId = r.executiveDirector,
                                         executiveVPId = vp.executiveVpId,
                                         executiveDirectorTitle = ed.userName,
                                         executiveVPTitle = vpu.userName,
                                     }).OrderByDescending(z=>z.regionId).ToListAsync();

                var groupedVPS = regions.GroupBy(a => a.regionId).Select(a => new
                {
                    regionId = a.Key,
                    title =a.FirstOrDefault().title,
                    TILsSummary = a.FirstOrDefault().TILsSummary,
                    insuranceSummary = a.FirstOrDefault().insuranceSummary,
                    executiveDirectorId = a.FirstOrDefault().executiveDirectorId,
                    executiveDirectorTitle =a.FirstOrDefault().executiveDirectorTitle,
                    executiveVPId = string.Join(", ", a.Select(u => u.executiveVPId)),
                    executiveVPTitle = string.Join(", ", a.Select(u => u.executiveVPTitle)),
                }).ToList();
               
                return Ok(groupedVPS);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("getSelecteddata")]
        public async Task<IActionResult> GetSelecteddata(SelectedRegionDto reg)
        {
            try
            {
                var selectedVps = await (from vp in _context.RegionsExecutiveVp.Where(a => a.regionId == reg.regionId)
                                         join s in _context.AppUser.Where(a=>a.isDeleted ==0) on vp.userId equals s.userId
                                         select new
                                         {
                                             s.userId,
                                             s.userName,
                                             name = s.firstName + " " + s.lastName,
                                             s.email,
                                         }).ToListAsync();



                return Ok(selectedVps);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteRegions")]
        public async Task<IActionResult> DeleteRegion(RegionsDto reg)
        {
            try
            {
                Regions regions = await (from r in _context.Regions.Where(a => a.regionId == reg.regionId )
                                     select r).FirstOrDefaultAsync();
                regions.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(regions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveRegions")]
        public async Task<IActionResult> saveRegion(RegionUserDto reg)
        {
            try
            {
                if (reg.data.region.regionId == -1)
                {
                    Regions region = new Regions();
                    region.isDeleted = 0;
                    region.title = reg.data.region.title;
                    region.TILsSummary = reg.data.region.TILsSummary;
                    region.insuranceSummary = reg.data.region.insuranceSummary;
                    region.executiveDirector= reg.data.region.executiveDirectorId;
                    region.createdBy = reg.userId;
                    region.createdOn = DateTime.Now;
                    _context.Add(region);
                    _context.SaveChanges();
                    reg.data.region.regionId=region.regionId;
                    for (var a = 0; a < reg.data.userList.Length; a++)
                    {
                        RegionsExecutiveVp vp = new RegionsExecutiveVp();
                        vp.regionId = region.regionId;
                        vp.userId = reg.data.userList[a].userId;
                        vp.createdOn = DateTime.Now;
                        vp.createdBy = reg.userId;
                        _context.Add(vp);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    Regions region = await (from r in _context.Regions.Where(a => a.regionId == reg.data.region.regionId)
                                            select r).FirstOrDefaultAsync();
                    region.title = reg.data.region.title;
                    region.TILsSummary = reg.data.region.TILsSummary;
                    region.insuranceSummary = reg.data.region.insuranceSummary;
                    region.executiveDirector = reg.data.region.executiveDirectorId;
                    region.modifiedBy = reg.userId;
                    region.modifiedOn = DateTime.Now;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM RegionsExecutiveVp WHERE regionId = @regionId", new SqlParameter("regionId", reg.data.region.regionId));
                    for (var a = 0; a < reg.data.userList.Length; a++)
                    {
                        RegionsExecutiveVp vp = new RegionsExecutiveVp();
                        vp.regionId = region.regionId;
                        vp.userId = reg.data.userList[a].userId;
                        vp.createdOn = DateTime.Now;
                        vp.createdBy = reg.userId;
                        _context.Add(vp);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.data.region);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
