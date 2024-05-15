using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ActionTrakingSystem.Controllers
{

    public class TilDetailReportController : BaseAPIController
    {
        private readonly DAL _context;
        public TilDetailReportController(DAL context)
        {
            _context = context;
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
                List<TilDetailReportDto> results = new List<TilDetailReportDto>();
                    results = await _context.TilDetailReport.FromSqlRaw(
                   "EXEC dbo.GetDetailedCountsAndTotal")
                   .ToListAsync();
                return Ok(results.Where(a => ((RegionsIds.Count == 0) || RegionsIds.Contains((int)a.regionId))
                    && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))
                    && ((SiteIds.Count == 0) || SiteIds.Contains((int)a.siteId))
                    && ((UnitIds.Count == 0) || UnitIds.Contains((int)a.unitId))
                    && ((StatusIds.Count == 0) || StatusIds.Contains((int)a.statusId))
                    && ((TimingIds.Count == 0) || TimingIds.Contains((int)a.timingId))
                    && ((FocusIds.Count == 0) || TimingIds.Contains((int)a.focusId))
                    && ((SeverityIds.Count == 0) || TimingIds.Contains((int)a.severityId))
                    ));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
