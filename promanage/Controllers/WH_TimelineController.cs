//using ActionTrakingSystem.DTOs;
//using ActionTrakingSystem.Model;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;

//namespace ActionTrakingSystem.Controllers
//{
//    public class WH_TimelineController : BaseAPIController
//    {
//        private readonly DAL _context;
//        public WH_TimelineController(DAL context)
//        {
//            _context = context;
//        }
//        [HttpGet("getTimeLine")]
//        public async Task<IActionResult> GetTimeLine()

//        {
//            try
//            {


//                var timeline = await (from a in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0)
//                                      join s in _context.Sites.Where(a => a.isDeleted == 0
//                                      //&& (reg.siteId == -1 || a.siteId == reg.siteId)
//                                      ) on a.siteId equals s.siteId
//                                      join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on a.equipmentId equals sh.unitId
//                                      join mh in _context.WH_MonthlyHours on sh.unitId equals mh.unitId
//                                      select new
//                                      {
//                                          a.siteId,
//                                          s.siteName,
//                                          a.eqType,
//                                          a.equipmentId,
//                                          a.unit,
//                                          sh.startDate,
//                                          sh.startHours,
//                                          sh.wceHours,
//                                          mh.yearId,
//                                          mh.runningHour,
//                                          mh.wce_runningHour,
//                                          mh.monthId,
//                                          s.onmContractExpiry,
//                                      }).ToListAsync();

//                var outages = await (from a in _context.WH_SiteEquipment
//                                     join ot in _context.WH_SiteNextOutages.Where(a => a.isDeleted == 0) on a.equipmentId equals ot.equipmentId
//                                     join it in _context.WH_ISiteOutages on ot.outageId equals it.outageId
//                                     select new
//                                     {
//                                         a.siteId,
//                                         a.equipmentId,
//                                         a.unit,
//                                         ot.outageId,
//                                         it.outageTitle,
//                                         ot.runningHours,
//                                         ot.wceHours,
//                                         ot.nextOutageDate,
//                                         it.colorCode,
//                                         counter = -1,
//                                         counter2 = -1,
//                                         validate = "",
//                                         ot.outageDurationInDays,
//                                     }).ToListAsync();
//                var uniqueSites = timeline.Select(a => new
//                {
//                    a.siteId,
//                    a.siteName,
//                    a.onmContractExpiry,
//                }).Distinct().ToList();


//                List<FlattenedSite> finalList = new List<FlattenedSite>();

//                foreach (var site in uniqueSites)
//                {

//                    var uniqueEquipments = timeline.Select(a => new
//                    {
//                        a.siteId,
//                        a.equipmentId,
//                        a.unit,
//                        a.startDate,
//                        a.startHours,
//                        a.wceHours,
//                        a.onmContractExpiry,
//                    }).Distinct().Where(a => a.siteId == site.siteId).ToList();
//                    foreach (var equipment in uniqueEquipments.Where(e => e.siteId == site.siteId))
//                    {

//                        var outagesList = outages.Select(a => new imutableProps
//                        {
//                            outageTitle = a.outageTitle,
//                            outageId = a.outageId,
//                            runningHours = (decimal)a.runningHours,
//                            nextOutageDate = a.nextOutageDate,
//                            colorCode = a.colorCode,
//                            counter = a.counter,
//                            equipmentId = a.equipmentId,
//                            validate = a.validate,
//                            outageDurationInDays = (decimal)a.outageDurationInDays,
//                        }).Distinct().Where(a => a.equipmentId == equipment.equipmentId).ToList();
//                        int differenceInYears = equipment.onmContractExpiry.Year - equipment.startDate.Year;
//                        List<int> yearList = new List<int>();
//                        var d = 0;
//                        for (var za = 0; za < differenceInYears; za++)
//                        {
//                            d = equipment.startDate.Year + za + 1;
//                            yearList.Add(d);
//                            d = 0;
//                        }

//                        var yearlyCounter = equipment.startHours;
//                        for (var yearIndex = 0; yearIndex < yearList.Count; yearIndex++)
//                        {
//                            var year = yearList[yearIndex];

//                            var yearlyTotalList = timeline
//                                .Where(a => a.yearId == year && a.equipmentId == equipment.equipmentId)
//                                .OrderBy(a => a.monthId)
//                                .ToList();
//                            FlattenedSite fs = new FlattenedSite();
//                            fs.siteId = (int)site.siteId;
//                            fs.siteName = site.siteName;
//                            fs.onmContractExpiry = site.onmContractExpiry;
//                            fs.yearId = fs.startDate.Year;
//                            fs.yearlyTotal = fs.startHours;
//                            fs.outageTitle = "";
//                            fs.outageId = null;
//                            fs.yearId = year;
//                            fs.equipmentId = (int)equipment.equipmentId;
//                            fs.unit = equipment.unit;
//                            fs.startDate = equipment.startDate;
//                            fs.startHours = equipment.startHours;
//                            foreach (var item in yearlyTotalList)
//                            {
//                                yearlyCounter += item.runningHour;
//                                foreach (var outage in outagesList.Where(o => o.equipmentId == equipment.equipmentId))
//                                {
//                                    if (outage.counter == -1)
//                                    {
//                                        outage.counter = yearlyCounter;
//                                    }
//                                    else
//                                    {
//                                        outage.counter += item.runningHour;
//                                    }

//                                    if ((outage.runningHours != null && outage.runningHours <= outage.counter && item.monthId == 12) ||
//                                        (outage.nextOutageDate.Year == year && item.monthId == 12 && outage.validate != "NoValidate"))
//                                    {
//                                        fs.outageId = outage.outageId;
//                                        fs.outageTitle = outage.outageTitle;

//                                        if (outage.nextOutageDate.Year == year)
//                                        {
//                                            outage.validate = "NoValidate";
//                                        }

//                                        outage.counter = 0;
//                                    }
//                                }

//                            }
//                            fs.yearlyTotal = yearlyCounter;
//                            finalList.Add(fs);
//                        }

//                    }
//                }


//                return Ok(finalList);
//            }
//            catch (Exception e)
//            {
//                return BadRequest(e.Message);
//            }
//        }
//    }
//}
