using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ActionTrakingSystem.Controllers
{

    public class PBIRunningHoursController : BaseAPIController
    {
        private readonly DAL _context;
        public PBIRunningHoursController(DAL context)
        {
            _context = context;
        }
        [HttpGet("getTimeLine")]
         public async Task<IActionResult> GetTimeLine()
            {
                try
                {
                    List<PBIRunningHoursSitesDto> t = new List<PBIRunningHoursSitesDto>();

                    var timeline = await (from a in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0)
                                          join s in _context.Sites.Where(a => a.isDeleted == 0) on a.siteId equals s.siteId
                                          join r in _context.Regions2.Where(a => a.isDeleted == 0) on s.region2 equals r.regionId
                                          join c in _context.Cluster.Where(a => a.isDeleted == 0) on s.clusterId equals c.clusterId
                                          join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on a.equipmentId equals sh.unitId
                                          join mh in _context.WH_MonthlyHours on sh.unitId equals mh.unitId
                                          select new
                                          {
                                              a.siteId,
                                              s.siteName,
                                              a.equipmentId,
                                              a.unit,
                                              r.regionId,
                                              a.eqType,
                                              regionTitle = r.title,
                                              c.clusterId,
                                              c.clusterTitle,
                                              sh.startDate,
                                              sh.startHours,
                                              wceStartHours = sh.wceHours,
                                              mh.yearId,
                                              mh.runningHour,
                                              mh.wce_runningHour,
                                              mh.monthId,
                                              s.onmContractExpiry,
                                              mh.fromCoe
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
                                             outageWOHHours = ot.runningHours,
                                             outageWCEHours = ot.wceHours,
                                             ot.nextOutageDate,
                                             it.colorCode,
                                             counter = -1,
                                             counter2 = -1,
                                             validate = "",
                                             ot.outageDurationInDays,
                                             a.eqType,
                                         }).ToListAsync();

                    var uniqueSites = timeline.Select(a => new
                    {
                        a.siteId,
                        a.siteName,
                        a.regionId,
                        a.regionTitle,
                        a.clusterId,
                        a.clusterTitle,
                        a.eqType,
                    }).Distinct().ToList();

                    for (var i = 0; i < uniqueSites.Count; i++)
                    {
                        //t = new List<PBIRunningHoursSitesDto>();
                        PBIRunningHoursSitesDto rh = new PBIRunningHoursSitesDto();
                        rh.siteId = (int)uniqueSites[i].siteId;
                        rh.siteName = uniqueSites[i].siteName;
                        rh.regionId = (int)uniqueSites[i].regionId;
                        rh.regionTitle = uniqueSites[i].regionTitle;
                        rh.clusterId = (int)uniqueSites[i].clusterId;
                        rh.clusterTitle = uniqueSites[i].clusterTitle;
                        rh.eqType = uniqueSites[i].eqType;
                        rh.eqps = new List<PBIRunningHoursEqpDto>();
                        List<PBIRunningHoursEqpDto> eqList = new List<PBIRunningHoursEqpDto>();
                        var uniqueEquipments = timeline.Select(a => new
                        {
                            a.siteId,
                            a.equipmentId,
                            a.unit,
                            a.startDate,
                            a.startHours,
                            a.wceStartHours,
                            a.onmContractExpiry,
                        }).Distinct().Where(a => a.siteId == (int)uniqueSites[i].siteId).ToList();

                        for (var j = 0; j < uniqueEquipments.Count; j++)
                        {
                            var outagesList = outages.Select(a => new imutableProps
                            {
                                outageTitle = a.outageTitle,
                                outageId = a.outageId,
                                outageWOHHours = (decimal)a.outageWOHHours,
                                outageWCEHours = (decimal)a.outageWCEHours,
                                nextOutageDate = a.nextOutageDate,
                                colorCode = a.colorCode,
                                counter = a.counter,
                                counter2 = a.counter2,
                                equipmentId = a.equipmentId,
                                validate = a.validate,
                                outageDurationInDays = (decimal)a.outageDurationInDays,
                                eqType = a.eqType,
                            }).Distinct().Where(a => a.equipmentId == uniqueEquipments[j].equipmentId).ToList();
                            PBIRunningHoursEqpDto eq = new PBIRunningHoursEqpDto();
                            eq.equipmentId = uniqueEquipments[j].equipmentId;
                            eq.unit = uniqueEquipments[j].unit;
                            eq.startDate = uniqueEquipments[j].startDate;
                            eq.startHours = uniqueEquipments[j].startHours;
                            eq.wceStartHours = uniqueEquipments[j].wceStartHours;
                            eq.onmContractExpiry = uniqueEquipments[j].onmContractExpiry;
                            eq.yearly = new List<PBIRunningHoursYearlyDto>();
                            PBIRunningHoursYearlyDto yd = new PBIRunningHoursYearlyDto();
                            yd.outages = new List<PBIRunningHoursOuatgesDto>();
                            yd.yearlyTotal = uniqueEquipments[j].startHours;
                            yd.yearId = uniqueEquipments[j].startDate.Year;
                            eq.yearly.Add(yd);
                            int differenceInYears = uniqueEquipments[j].onmContractExpiry.Year - uniqueEquipments[j].startDate.Year;
                            List<int> yearList = new List<int>();
                            var d = 0;
                            for (var za = 0; za < differenceInYears; za++)
                            {
                                d = uniqueEquipments[j].startDate.Year + za + 1;
                                yearList.Add(d);
                                d = 0;
                            }

                            decimal yearlyCounter = uniqueEquipments[j].startHours;
                            decimal yearlyCounter2 = uniqueEquipments[j].wceStartHours;
                            for (var k = 0; k < yearList.Count; k++)
                            {
                                yd = new PBIRunningHoursYearlyDto();
                                yd.outages = new List<PBIRunningHoursOuatgesDto>();
                                var yearlyTotalList = timeline.Select(a => new
                                {
                                    a.equipmentId,
                                    a.yearId,
                                    a.runningHour,
                                    a.wce_runningHour,
                                    a.monthId,
                                    a.fromCoe,
                                }).Distinct().Where(a => a.yearId == (int)yearList[k] && a.equipmentId == uniqueEquipments[j].equipmentId).OrderBy(a => a.monthId).ToList();

                            // Find the maximum monthId where fromCoe == 1
                            int maxMonthId = yearlyTotalList.Any(a => a.fromCoe == 1)? yearlyTotalList.Where(a => a.fromCoe == 1).Max(a => a.monthId): 1; 
                            yearlyTotalList = yearlyTotalList.Where(a => a.monthId >= maxMonthId).ToList();

                            for (var l = 0; l < yearlyTotalList.Count; l++)
                                {
                                    yearlyCounter += yearlyTotalList[l].runningHour;
                                    yearlyCounter2 += yearlyTotalList[l].wce_runningHour;

                                    for (var p = 0; p < outagesList.Count; p++)
                                    {
                                        if (outagesList[p].counter == -1)
                                        {
                                            outagesList[p].counter = yearlyCounter;
                                        }
                                        else
                                        {
                                            outagesList[p].counter += yearlyTotalList[l].runningHour;
                                        }
                                        if (outagesList[p].counter2 == -1)
                                        {
                                            outagesList[p].counter2 = yearlyCounter2;
                                        }
                                        else
                                        {
                                            outagesList[p].counter2 += yearlyTotalList[l].wce_runningHour;
                                        }
                                        if ((outagesList[p].outageWCEHours != null && outagesList[p].eqType == "GT" && outagesList[p].outageWCEHours <= outagesList[p].counter2 && yearlyTotalList[l].monthId == 12) || (outagesList[p].outageWOHHours != null && outagesList[p].outageWOHHours <= outagesList[p].counter && yearlyTotalList[l].monthId == 12) || (outagesList[p].nextOutageDate.Year == yearList[k] && yearlyTotalList[l].monthId == 12 && outagesList[p].validate != "NoValidate"))
                                        {
                                            PBIRunningHoursOuatgesDto o = new PBIRunningHoursOuatgesDto();
                                            o.outageTitle = outagesList[p].outageTitle;
                                            o.outageId = outagesList[p].outageId;
                                            o.startDate = new DateTime(yearlyTotalList[l].yearId, yearlyTotalList[l].monthId, 1);
                                            o.endDate = o.startDate.AddDays((double)outagesList[p].outageDurationInDays);
                                            if (yd.outages.Count == 0)
                                            {
                                                yd.outages.Add(o);
                                            }
                                            else
                                            {
                                                var xcx = yd.outages.Select(a => a.outageTitle).Where(b => b == "C/Major").ToList();
                                                if (xcx.Count <= 0)
                                                {
                                                    yd.outages = new List<PBIRunningHoursOuatgesDto>();
                                                    yd.outages.Add(o);
                                                }
                                            }
                                            if (outagesList[p].nextOutageDate.Year == yearList[k])
                                            {
                                                outagesList[p].validate = "NoValidate";
                                            }

                                            outagesList[p].counter = 0;
                                            outagesList[p].counter2 = 0;
                                        }
                                    }
                                }
                                yd.yearId = (int)yearList[k];
                                yd.yearlyTotal = yearlyCounter;
                                eq.yearly.Add(yd);
                            }
                            rh.eqps.Add(eq);
                        }
                        t.Add(rh);
                    }

                var at = t;
                   
                return Ok(t);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
       
   


    //public async Task<IActionResult> GetTimeLine()
    //{
    //    try
    //    {
    //        List<PBIRunningHoursSitesDto> t = new List<PBIRunningHoursSitesDto>();

    //        var timeline = await (from a in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0)
    //                              join s in _context.Sites.Where(a => a.isDeleted == 0 ) on a.siteId equals s.siteId
    //                              join r in _context.Regions2.Where(a => a.isDeleted == 0) on s.region2 equals r.regionId
    //                              join c in _context.Cluster.Where(a=>a.isDeleted == 0) on s.clusterId equals c.clusterId
    //                              join sh in _context.WH_StartingHours.Where(a => a.isDeleted == 0) on a.equipmentId equals sh.unitId
    //                              join mh in _context.WH_MonthlyHours on sh.unitId equals mh.unitId
    //                              select new
    //                              {
    //                                  a.siteId,
    //                                  s.siteName,
    //                                  a.equipmentId,
    //                                  a.unit,
    //                                  r.regionId,
    //                                  a.eqType,
    //                                  regionTitle = r.title,
    //                                  c.clusterId,
    //                                  c.clusterTitle,
    //                                  sh.startDate,
    //                                  sh.startHours,
    //                                  wceStartHours = sh.wceHours,
    //                                  mh.yearId,
    //                                  mh.runningHour,
    //                                  mh.wce_runningHour,
    //                                  mh.monthId,
    //                                  s.onmContractExpiry,
    //                                  mh.fromCoe
    //                              }).ToListAsync();

    //        var outages = await (from a in _context.WH_SiteEquipment
    //                             join ot in _context.WH_SiteNextOutages.Where(a => a.isDeleted == 0) on a.equipmentId equals ot.equipmentId
    //                             join it in _context.WH_ISiteOutages on ot.outageId equals it.outageId
    //                             select new
    //                             {
    //                                 a.siteId,
    //                                 a.equipmentId,
    //                                 a.unit,
    //                                 ot.outageId,
    //                                 it.outageTitle,
    //                                 outageWOHHours = ot.runningHours,
    //                                 outageWCEHours = ot.wceHours,
    //                                 ot.nextOutageDate,
    //                                 it.colorCode,
    //                                 counter = -1,
    //                                 counter2 = -1,
    //                                 validate = "",
    //                                 ot.outageDurationInDays,
    //                                 a.eqType,

    //                             }).ToListAsync();

    //        var uniqueSites = timeline.Select(a => new
    //        {
    //            a.siteId,
    //            a.siteName,
    //            a.regionId,
    //            a.regionTitle,
    //            a.clusterId,
    //            a.clusterTitle,
    //            a.eqType,
    //            a.fromCoe,
    //        }).Distinct().ToList();

    //        for (var i = 0; i < uniqueSites.Count; i++)
    //        {
    //            //t = new List<PBIRunningHoursSitesDto>();
    //            PBIRunningHoursSitesDto rh = new PBIRunningHoursSitesDto();
    //            rh.siteId = (int)uniqueSites[i].siteId;
    //            rh.siteName = uniqueSites[i].siteName;
    //            rh.regionId = (int)uniqueSites[i].regionId;
    //            rh.regionTitle = uniqueSites[i].regionTitle;
    //            rh.clusterId = (int)uniqueSites[i].clusterId;
    //            rh.clusterTitle = uniqueSites[i].clusterTitle;
    //            rh.eqType = uniqueSites[i].eqType;
    //            rh.eqps = new List<PBIRunningHoursEqpDto>();
    //            List<PBIRunningHoursEqpDto> eqList = new List<PBIRunningHoursEqpDto>();
    //            var uniqueEquipments = timeline.Select(a => new
    //            {
    //                a.siteId,
    //                a.equipmentId,
    //                a.unit,
    //                a.startDate,
    //                a.startHours,
    //                a.wceStartHours,
    //                a.onmContractExpiry,
    //                a.fromCoe
    //            }).Distinct().Where(a => a.siteId == (int)uniqueSites[i].siteId).ToList();

    //            for (var j = 0; j < uniqueEquipments.Count; j++)
    //            {
    //                var outagesList = outages.Select(a => new imutableProps
    //                {
    //                    outageTitle = a.outageTitle,
    //                    outageId = a.outageId,
    //                    outageWOHHours = (decimal)a.outageWOHHours,
    //                    outageWCEHours = (decimal)a.outageWCEHours,
    //                    nextOutageDate = a.nextOutageDate,
    //                    colorCode = a.colorCode,
    //                    counter = a.counter,
    //                    counter2 = a.counter2,
    //                    equipmentId = a.equipmentId,
    //                    validate = a.validate,
    //                    outageDurationInDays = (decimal)a.outageDurationInDays,
    //                    eqType = a.eqType,
    //                }).Distinct().Where(a => a.equipmentId == uniqueEquipments[j].equipmentId).ToList();
    //                PBIRunningHoursEqpDto eq = new PBIRunningHoursEqpDto();
    //                eq.equipmentId = uniqueEquipments[j].equipmentId;
    //                eq.unit = uniqueEquipments[j].unit;
    //                eq.startDate = uniqueEquipments[j].startDate;
    //                eq.startHours = uniqueEquipments[j].startHours;
    //                eq.wceStartHours = uniqueEquipments[j].wceStartHours;
    //                eq.onmContractExpiry = uniqueEquipments[j].onmContractExpiry;
    //                eq.yearly = new List<PBIRunningHoursYearlyDto>();
    //                PBIRunningHoursYearlyDto yd = new PBIRunningHoursYearlyDto();
    //                yd.outages = new List<PBIRunningHoursOuatgesDto>();
    //                yd.yearlyTotal = uniqueEquipments[j].startHours;
    //                yd.yearId = uniqueEquipments[j].startDate.Year;
    //                eq.yearly.Add(yd);
    //                int differenceInYears = uniqueEquipments[j].onmContractExpiry.Year - uniqueEquipments[j].startDate.Year;
    //                List<int> yearList = new List<int>();
    //                var d = 0;
    //                for (var za = 0; za < differenceInYears; za++)
    //                {
    //                    d = uniqueEquipments[j].startDate.Year + za + 1;
    //                    yearList.Add(d);
    //                    d = 0;
    //                }

    //                decimal yearlyCounter = uniqueEquipments[j].startHours;
    //                decimal yearlyCounter2 = uniqueEquipments[j].wceStartHours;
    //                for (var k = 0; k < yearList.Count; k++)
    //                {
    //                    yd = new PBIRunningHoursYearlyDto();
    //                    yd.outages = new List<PBIRunningHoursOuatgesDto>();
    //                    var yearlyTotalList = timeline.Select(a => new
    //                    {
    //                        a.equipmentId,
    //                        a.yearId,
    //                        a.runningHour,
    //                        a.wce_runningHour,
    //                        a.monthId,
    //                        a.fromCoe
    //                    }).Distinct().Where(a => a.yearId == (int)yearList[k] && a.equipmentId == uniqueEquipments[j].equipmentId).OrderBy(a => a.monthId).ToList();

    //                    var maxRunningHour = yearlyTotalList.Where(y => y.fromCoe == 1).Select(y => y.runningHour).DefaultIfEmpty(0).Max();  
    //                    yearlyCounter += maxRunningHour;

    //                        for (var l = 0; l < yearlyTotalList.Count; l++)
    //                        {

    //                        if (yearlyTotalList[l].fromCoe == 0)
    //                        {
    //                            yearlyCounter += yearlyTotalList[l].runningHour;

    //                        }

    //                        yearlyCounter2 += yearlyTotalList[l].wce_runningHour;

    //                        for (var p = 0; p < outagesList.Count; p++)
    //                        {
    //                            if (outagesList[p].counter == -1)
    //                            {
    //                                outagesList[p].counter = yearlyCounter;
    //                            }
    //                            else
    //                            {
    //                                outagesList[p].counter += yearlyTotalList[l].runningHour;
    //                            }
    //                            if (outagesList[p].counter2 == -1)
    //                            {
    //                                outagesList[p].counter2 = yearlyCounter2;
    //                            }
    //                            else
    //                            {
    //                                outagesList[p].counter2 += yearlyTotalList[l].wce_runningHour;
    //                            }
    //                            if ((outagesList[p].outageWCEHours != null && outagesList[p].eqType =="GT" &&
    //                                outagesList[p].outageWCEHours <= outagesList[p].counter2 && yearlyTotalList[l].monthId == 12) ||
    //                                (outagesList[p].outageWOHHours != null && outagesList[p].outageWOHHours <= outagesList[p].counter &&
    //                                yearlyTotalList[l].monthId == 12) || (outagesList[p].nextOutageDate.Year == yearList[k] &&
    //                                yearlyTotalList[l].monthId == 12 && outagesList[p].validate != "NoValidate"))
    //                            {
    //                                PBIRunningHoursOuatgesDto o = new PBIRunningHoursOuatgesDto();
    //                                o.outageTitle = outagesList[p].outageTitle;
    //                                o.outageId = outagesList[p].outageId;
    //                                o.startDate = new DateTime(yearlyTotalList[l].yearId, yearlyTotalList[l].monthId, 1);
    //                                o.endDate = o.startDate.AddDays((double)outagesList[p].outageDurationInDays);
    //                                if (yd.outages.Count == 0)
    //                                {
    //                                    yd.outages.Add(o);
    //                                }
    //                                else
    //                                {
    //                                    var xcx = yd.outages.Select(a => a.outageTitle).Where(b => b == "C/Major").ToList();
    //                                    if (xcx.Count <= 0)
    //                                    {
    //                                        yd.outages = new List<PBIRunningHoursOuatgesDto>();
    //                                        yd.outages.Add(o);
    //                                    }
    //                                }
    //                                if (outagesList[p].nextOutageDate.Year == yearList[k])
    //                                {
    //                                    outagesList[p].validate = "NoValidate";
    //                                }

    //                                outagesList[p].counter = 0;
    //                                outagesList[p].counter2 = 0;
    //                            }
    //                        }
    //                    }
    //                    yd.yearId = (int)yearList[k];
    //                    yd.yearlyTotal = yearlyCounter;
    //                    eq.yearly.Add(yd);
    //                }
    //                rh.eqps.Add(eq);
    //            }
    //            t.Add(rh);
    //        }
    //        return Ok(t);
    //    }
    //    catch (Exception e)
    //    {
    //        return BadRequest(e.Message);
    //    }
    //}
}
}
