using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TIlActionReportController : ControllerBase
    {
        private readonly DAL _context;
        public TIlActionReportController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto rege)
        {
            try
            {

                var timing = await (from a in _context.TILOEMTimimgCode.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.timingId,
                                        a.timingCode,
                                    }).ToListAsync();
                    var focus = await (from a in _context.TILFocus.Where(a => a.isDeleted == 0)
                                       select new
                                       {
                                           a.focusId,
                                           a.focusTitle,
                                       }).ToListAsync();
                var severity = await (from a in _context.TILOEMSeverity.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          a.oemSeverityId,
                                          a.oemSeverityTitle,
                                      }).ToListAsync();
                var status = await (from a in _context.TAStatus.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.tasId,
                                        a.title,
                                    }).ToListAsync();
                var unit = await (from a in _context.SiteEquipment.Where(a => a.isDeleted == 0)
                                    select new
                                    {
                                        a.equipmentId,
                                        unit = a.siteUnit,
                                    }).ToListAsync();
                var obj = new
                {
                    timing,
                    focus,
                    severity,
                    status,
                    unit
                };

                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getReport")]
        public async Task<IActionResult> GetReports(TilReportDto rege)
        {
            try
            {
                List<int> RegionsIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.regionList))
                    RegionsIds = (rege.regionList.Split(',').Select(Int32.Parse).ToList());
                List<int> SiteIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.siteList))
                    SiteIds = (rege.siteList.Split(',').Select(Int32.Parse).ToList());
                List<int> ClusterIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.clusterList))
                    ClusterIds = (rege.clusterList.Split(',').Select(Int32.Parse).ToList());
                List<int> UnitIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.unitList))
                    UnitIds = (rege.unitList.Split(',').Select(Int32.Parse).ToList());
                List<int> StatusIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.statusList))
                    StatusIds = (rege.statusList.Split(',').Select(Int32.Parse).ToList());
                List<int> TimingIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.timmingList))
                    TimingIds = (rege.timmingList.Split(',').Select(Int32.Parse).ToList());
                List<int> FocusIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.focusList))
                    FocusIds = (rege.focusList.Split(',').Select(Int32.Parse).ToList());
                List<int> SeverityIds = new List<int>();
                if (!string.IsNullOrEmpty(rege.severityList))
                    SeverityIds = (rege.severityList.Split(',').Select(Int32.Parse).ToList());
                List<TilActionReportDto> results = new List<TilActionReportDto>();
                results = await _context.TilActionReportDto.FromSqlRaw(
               "select * from GetActionTrackerDetails()")
               .ToListAsync();
                var filtered = results.Where(a => ((RegionsIds.Count == 0) || RegionsIds.Contains((int)a.regionId))
                    && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))
                    && ((SiteIds.Count == 0) || SiteIds.Contains((int)a.siteId))
                    && ((UnitIds.Count == 0) || UnitIds.Contains((int)a.unitId))
                    && ((StatusIds.Count == 0) || StatusIds.Contains((int)a.statusId))
                    && ((TimingIds.Count == 0) || TimingIds.Contains((int)a.timingId))
                    && ((FocusIds.Count == 0) || TimingIds.Contains((int)a.focusId))
                    && ((SeverityIds.Count == 0) || TimingIds.Contains((int)a.severityId))
                    );
                var groupedBy = filtered.GroupBy(u => new
                {
                    u.equipmentId,
                    u.unit,
                    u.siteId,
                    u.siteName,
                    u.regionId,
                    u.regionTitle,
                    u.clusterId,
                    u.clusterTitle,
                    u.tilNumber,
                    u.focusTitle,
                    u.timingCode,
                    u.oemSeverityTitle,
                }).Select(group => new 
                {
                    equipmentId = group.Key.equipmentId,
                    unit = group.Key.unit,
                    siteId = group.Key.siteId,
                    siteName = group.Key.siteName,
                    regionId = group.Key.regionId,
                    regionTitle = group.Key.regionTitle,
                    clusterId = group.Key.clusterId,
                    clusterTitle = group.Key.clusterTitle,
                    tilNumber = group.Key.tilNumber,
                    focusTitle = group.Key.focusTitle,
                    timingCode = group.Key.timingCode,
                    oemSeverityTitle = group.Key.oemSeverityTitle,
                    statusId = group.All(u => u.statusId == 3) ? 3 : 2,
                    statusTitle = group.All(u => u.statusTitle == "Closed") ? "Closed" : "Open",
                }).ToList();

             

                return Ok(groupedBy);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
