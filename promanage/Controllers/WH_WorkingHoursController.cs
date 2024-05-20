using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class WH_WorkingHoursController : BaseAPIController
    {
        private readonly DAL _context;
        public WH_WorkingHoursController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("getWorkingHour")]
        public async Task<IActionResult> GetWorkingHour(WH_WorkingHoursUserDto reg)
        {
            try
            {
                var workingEq = await (from a in _context.WH_StartingHours.Where(a => a.isDeleted == 0)
                                       join se in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0 && (reg.filter.equipmentId == -1 || a.equipmentId == reg.filter.equipmentId)) on a.unitId equals se.equipmentId
                                       join s in _context.Sites.Where(a => (reg.filter.siteId == -1 || a.siteId == reg.filter.siteId) && (reg.filter.clusterId == -1 || a.clusterId == reg.filter.clusterId)) on se.siteId equals s.siteId
                                       join r in _context.Regions.Where(a => reg.filter.regionId == -1 || a.regionId == reg.filter.regionId) on s.regionId equals r.regionId
                                       join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0) on s.siteId equals ts.siteId
                                       join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                       join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                       join clus in _context.Cluster on s.clusterId equals clus.clusterId
                                       select new
                                       {
                                           a.startingId,
                                           r.regionId,
                                           regionTitle = r.title,
                                           s.siteId,
                                           siteTitle = s.siteName,
                                           se.equipmentId,
                                           se.unit,
                                           se.eqType,
                                           a.startHours,
                                           a.startDate,
                                           s.onmContractExpiry,
                                           a.wceHours,
                                           cluster = clus.clusterTitle
                                       }).Distinct().OrderByDescending(a => a.startingId).ToListAsync();
                return Ok(workingEq);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("deleteWorkingHour")]
        public async Task<IActionResult> DeleteWorkingHour(WH_UserDto reg)
        {
            try
            {
                var workingEq = await (from a in _context.WH_StartingHours.Where(a => a.startingId == reg.result.startingId)
                                       select a
                                       ).FirstOrDefaultAsync();
                workingEq.isDeleted = 1;
                _context.SaveChanges();
                return Ok(workingEq);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public string RemoveSpaces(string input)
        {
            string trimmed = input.Trim();
            string cleaned = string.Join("", trimmed.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            return cleaned;
        }
        [HttpPost("saveCoeData")]
        public async Task<IActionResult> SaveCoeHour(List<CoeDto> dataArray)
        {
            try
            {
                foreach (var data in dataArray)
                {
                    var cleanedEquipment = RemoveSpaces(data.equipment);

                    var equipment = await (from a in _context.WH_SiteEquipment.Where(a => a.unit == cleanedEquipment && a.siteId == data.siteId)
                                           select a)
                                           .FirstOrDefaultAsync();
                    if (equipment == null)
                    {
                        WH_SiteEquipment siteEq = new WH_SiteEquipment();
                        siteEq.isDeleted = 0;
                        siteEq.createdDate = DateTime.Now;
                        siteEq.createdBy = 1;
                        siteEq.siteId = data.siteId;
                        siteEq.fleetEquipmentId = 1;
                        siteEq.unitSN = cleanedEquipment;
                        siteEq.details = "";
                        siteEq.responsible = 1;
                        siteEq.unitCOD = DateTime.Now;
                        siteEq.unit = cleanedEquipment;
                        siteEq.eqType = cleanedEquipment?.Substring(0, Math.Min(2, cleanedEquipment.Length));
                        _context.Add(siteEq);
                        _context.SaveChanges();
                        var sites = await (from a in _context.Sites.Where(a => a.siteId == data.siteId)
                                           select a).FirstOrDefaultAsync();
                        if (sites != null)
                        {
                            var equipmentIdParameter = new SqlParameter("@equipmentId", siteEq.equipmentId);
                            var endOfContractDateParameter = new SqlParameter("@endOfContractDate", sites.onmContractExpiry);
                            var userIdParameter = new SqlParameter("@userId", 1);
                            //var coePara = new SqlParameter("@fromCoe", 0);
                            _context.Database.ExecuteSqlRaw("EXEC InsertOrUpdateRunningHours @equipmentId, @endOfContractDate, @userId", equipmentIdParameter, endOfContractDateParameter, userIdParameter);
                        }
                        WH_StartingHours sh = new WH_StartingHours();
                        sh.startHours = 0;
                        sh.startDate = new DateTime(2023, 12, 31);
                        sh.unitId = siteEq.equipmentId;
                        sh.isDeleted = 0;
                        sh.createdOn = DateTime.Now;
                        sh.createdBy = 1;
                        _context.Add(sh);
                        _context.SaveChanges();

                        WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a => a.monthId == data.monthId && a.yearId == data.yearId && a.unitId == siteEq.equipmentId)
                                                    select a).FirstOrDefaultAsync();
                        if (ys != null)
                        {
                            ys.runningHour = (decimal)data.woh;
                            ys.wce_runningHour = (decimal)data.wce;
                            ys.createdBy = 1;
                            ys.fromCoe = 1;
                            ys.createdOn = DateTime.Now;
                            _context.SaveChanges();
                        }

                    }
                    else
                    {
                        var cv = await (from a in _context.WH_StartingHours.Where(a => a.unitId == equipment.equipmentId)
                                        select a
               ).FirstOrDefaultAsync();
                        if (cv == null)
                        {
                            WH_StartingHours sh = new WH_StartingHours();
                            sh.startHours = 0;
                            sh.startDate = new DateTime(2023, 12, 31);
                            sh.unitId = equipment.equipmentId;
                            sh.isDeleted = 0;
                            sh.createdOn = DateTime.Now;
                            sh.createdBy = 1;
                            _context.Add(sh);
                            _context.SaveChanges();
                        }
                        WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a => a.monthId == data.monthId && a.yearId == data.yearId && a.unitId == equipment.equipmentId)
                                                    select a).FirstOrDefaultAsync();
                        if (ys != null)
                        {
                            ys.runningHour = (decimal)data.woh;
                            ys.wce_runningHour = (decimal)data.wce;
                            ys.fromCoe = 1;
                            ys.createdOn = DateTime.Now;
                            _context.SaveChanges();
                        }
                    }
                }

                return Ok("Saved");
            }
            catch (Exception e)
            {
                return BadRequest("Error");
            }
        }
        [Authorize]
        [HttpPost("saveWorkingHour")]
        public async Task<IActionResult> SaveWorkingHour(WH_UserDto reg)
        {
            try
            {
                if (reg.result.startingId == -1)
                {
                    WH_StartingHours sh = new WH_StartingHours();
                    sh.startHours = reg.result.startHours;
                    sh.startDate = Convert.ToDateTime(reg.result.startDate);
                    sh.unitId = reg.result.equipmentId;
                    sh.isDeleted = 0;
                    sh.createdOn = DateTime.Now;
                    sh.createdBy = reg.userId;
                    sh.wceHours = reg.result.wceHours;
                    _context.Add(sh);
                    _context.SaveChanges();
                    return Ok(sh);
                }
                else
                {
                    var sh = await (from a in _context.WH_StartingHours.Where(a => a.startingId == reg.result.startingId)
                                    select a
                       ).FirstOrDefaultAsync();
                    sh.startHours = reg.result.startHours;
                    sh.startDate = Convert.ToDateTime(reg.result.startDate);
                    sh.unitId = reg.result.equipmentId;
                    sh.modifiedOn = DateTime.Now;
                    sh.modifiedBy = reg.userId;
                    sh.wceHours = reg.result.wceHours;
                    _context.SaveChanges();
                    return Ok(sh);

                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getYearlyWorkingHour")]
        public async Task<IActionResult> GetYearlyWorkingHour(WH_YearlyDro reg)
        {
            try
            {
                var monthlyHours = await (from a in _context.WH_MonthlyHours.Where(a => a.unitId == reg.result.equipmentId && a.yearId <= reg.yearId)
                                          select new
                                          {
                                              yearId = a.yearId,
                                              runningHours = a.runningHour,
                                              a.wce_runningHour,
                                              a.reduceHours,
                                              a.fromCoe,
                                              monthId = a.monthId
                                          }
                                         ).OrderBy(a => a.monthId).ToListAsync();
                var calcStartingHours = monthlyHours.Select(a => new
                {
                    a.runningHours,
                    a.reduceHours,
                    a.yearId,
                    a.fromCoe,
                }).Where(a => a.yearId < reg.yearId).ToList();
                decimal monthlyTotal = reg.result.startHours;
                for (var i = 0; i < calcStartingHours.Count; i++)
                {
                    monthlyTotal += calcStartingHours[i].runningHours - calcStartingHours[i].reduceHours;
                }
                var startHours = monthlyTotal;

                var monthlyHours2 = monthlyHours.Select(a => new
                {
                    a.runningHours,
                    a.reduceHours,
                    a.yearId,
                    a.monthId,
                    a.fromCoe,
                }).Where(a => a.yearId == reg.yearId).ToList();
                List<WH_MonthlyModel> monthlyList = new List<WH_MonthlyModel>();
                decimal yearltCount = 0;
                if (monthlyHours2.Count == 0)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        WH_MonthlyModel monthlyModel = new WH_MonthlyModel();
                        monthlyModel.runningHours = 0;
                        monthlyModel.monthId = i + 1;
                        monthlyModel.yearId = reg.yearId;
                        monthlyTotal += 0;
                        monthlyModel.monthlyTotal = monthlyTotal;
                        monthlyList.Add(monthlyModel);

                    }
                }
                else
                {
                    for (var i = 0; i < monthlyHours2.Count; i++)
                    {
                        WH_MonthlyModel monthlyModel = new WH_MonthlyModel();
                        monthlyModel.runningHours = Math.Round(monthlyHours2[i].runningHours, 2);
                        monthlyModel.reduceHours = Math.Round(monthlyHours2[i].reduceHours, 2);
                        monthlyModel.fromCoe = monthlyHours2[i].fromCoe;
                        monthlyModel.monthId = monthlyHours2[i].monthId;
                        monthlyModel.yearId = reg.yearId;
                        monthlyTotal += monthlyHours2[i].runningHours - monthlyHours2[i].reduceHours;
                        monthlyModel.monthlyTotal = monthlyTotal;
                        monthlyList.Add(monthlyModel);

                    }
                }

                var obj = new
                {
                    yealyList = monthlyList,
                    startHours = startHours
                };
                return Ok(obj);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSites")]
        public async Task<IActionResult> GetSites(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join eq in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0) on s.siteId equals eq.siteId
                                   join dd in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on eq.equipmentId equals dd.unitId
                                   join cc in _context.WH_MonthlyHours on dd.unitId equals cc.unitId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }
                                 ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveHours")]
        public async Task<IActionResult> SaveWorkingHour(WH_SaveHours reg)
        {
            try
            {
                    for (var i = 0; i < reg.yearlyResult.Length; i++)
                    {
                    WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a=>a.monthId == reg.yearlyResult[i].monthId && a.yearId == reg.yearlyResult[i].yearId && a.unitId == reg.result.equipmentId)
                                                select a).FirstOrDefaultAsync();
                        ys.monthId = reg.yearlyResult[i].monthId;
                        ys.unitId = reg.result.equipmentId;
                        ys.yearId = reg.yearlyResult[i].yearId;
                        ys.reduceHours = Math.Abs(reg.yearlyResult[i].reduceHours);
                        ys.createdBy = reg.userId;
                        ys.createdOn = DateTime.Now;
                        _context.SaveChanges();

                    }
                    return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("getTimeLine")]
        public async Task<IActionResult> GetTimeLine(WH_TimeLineDto reg)
        {
            try
            {
                var timeline = await (from r in _context.Regions2.Where(a => a.isDeleted == 0 && (reg.regionId == -1 || a.regionId == reg.regionId))
                                      join s in _context.Sites.Where(a => a.isDeleted == 0 && (reg.siteId == -1|| a.siteId == reg.siteId) && (reg.clusterId == -1 || a.clusterId == reg.clusterId)) on r.regionId equals s.region2
                                      join e in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0 && (reg.unitId == -1 || a.equipmentId == reg.unitId)) on s.siteId equals e.siteId
                                      join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on e.equipmentId equals sh.unitId
                                      join mh in _context.WH_MonthlyHours on sh.unitId equals mh.unitId
                                      select new
                                      {
                                          e.siteId,
                                          e.equipmentId,
                                          unit = s.siteName + " " + e.unit,
                                          sh.startDate,
                                          sh.startHours,
                                          startHoursWce = sh.wceHours,
                                          mh.yearId,
                                          mh.runningHour,
                                          wceRunningHour = mh.wce_runningHour,
                                          e.eqType,
                                          mh.monthId,
                                          s.onmContractExpiry,
                                      }).ToListAsync();


                var outages = await (from a in _context.WH_SiteEquipment                                      
                                     join ot in _context.WH_SiteNextOutages.Where(a => a.isDeleted == 0) on a.equipmentId equals ot.equipmentId
                                     join it in _context.WH_ISiteOutages on ot.outageId equals it.outageId
                                     select new
                                     {
                                         a.siteId,
                                         a.equipmentId,
                                         a.unit,
                                         ot.outageId,
                                         it.outageTitle,
                                         ot.runningHours,
                                         ot.wceHours,
                                         ot.nextOutageDate,
                                         it.colorCode,
                                         counter = -1,
                                         counterWce = -1
                                     }).ToListAsync();

                var obj = new
                {
                    timeline,
                    outages
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
