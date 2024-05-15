using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WH_GTHoursController : ControllerBase
    {
        private readonly DAL _context;
        public WH_GTHoursController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getWorkingHours")]
        public async Task<IActionResult> GetWorkingHours(UserIdDto reg)
        {
            try
            {
               
                var workingHours = await (from r in _context.Regions2.Where(a => a.isDeleted == 0)
                                          join s in _context.Sites.Where(a => a.isDeleted == 0) on r.regionId equals s.region2
                                          join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                          join e in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0 && a.eqType == "GT") on s.siteId equals e.siteId                     
                                          join w in _context.WH_StartingHours on e.equipmentId equals w.unitId
                                          select new
                                          {
                                              regionId = r.regionId,
                                              regionTitle = r.title,
                                              siteId = s.siteId,
                                              siteTitle = s.siteName,
                                              unitTitle = e.unit,
                                              unitId = w.unitId,
                                              endOfContract = s.onmContractExpiry,
                                              startingHours = w.startDate,
                                          }).Distinct().ToListAsync();
                return Ok(workingHours);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("saveHours")]
        public async Task<IActionResult> SaveWorkingHour(WH_SaveGTHours reg)
        {
            try
            {
                for (var i = 0; i < reg.data.Count; i++)
                {
                    WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a => a.monthId == reg.data[i].monthId && a.yearId == reg.data[i].yearId && a.unitId == reg.data[i].unitId)
                                                select a).FirstOrDefaultAsync();
                    if (ys!=null)
                    {
                        ys.runningHour = reg.data[i].runningHour;
                        ys.wce_runningHour = reg.data[i].wce_runningHour;
                        ys.createdBy = reg.userId;
                        ys.createdOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                  

                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("getMonthlyHours")]
        public async Task<IActionResult> GetMonthlyHours(WH_GTHourDto reg)
        {
            try
            {
                var workingHours = await (from w in _context.WH_MonthlyHours.Where(a=>a.yearId == reg.yearId && a.unitId == reg.unitId)
                                          select new
                                          {
                                              w.monthId,
                                              w.yearId,
                                              w.unitId,
                                              w.monthlyHourId,
                                              w.runningHour,
                                              w.wce_runningHour
                                          }).OrderBy(a=>a.monthId).ToListAsync();
                return Ok(workingHours);              
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
