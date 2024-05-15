using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Security.Policy;

namespace ActionTrakingSystem.Controllers
{

    public class KPIMonthlyBusinessProcessController : BaseAPIController
    {
        private readonly DAL _context;
        public KPIMonthlyBusinessProcessController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(getKPIBusinessDto reg)
        {
            try
            {
                var kpi = await (from sdd in _context.KPI_SiteInfo.Where(a => a.isDeleted == 0 && a.siteId == reg.siteId)
                                 select new
                                 {
                                     sdd.siteId,
                                     sdd.indicatorId,
                                     sdd.measurementTitle,
                                     sdd.annualTargetTitle,
                                     sdd.classificationTitle
                                 }).ToListAsync();
                return Ok(kpi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getKPIsIndicator")]
        public async Task<IActionResult> GetIndicator(getKPIBusinessDto reg)
        {
            try
            {
                int sMonth = reg.monthId;
                int sYear = reg.yearId;
                var monthNumber = Convert.ToInt32(sMonth);
                var yearNumber = Convert.ToInt32(sYear);
                //var kpi = await (from a in _context.KPI_IndicatorGroup.Where(a => a.isDeleted == 0)
                //                 join ag in _context.KPI_UserGroup.Where(a => a.userId == reg.userId) on a.groupId equals ag.groupId
                //                 join b in _context.KPI_Indicator on a.groupId equals b.groupId
                //                 join c in _context.KPI_IndicatorWeightage.Where(a => a.siteId == reg.siteId && a.month == monthNumber && a.year == yearNumber) on b.indicatorId equals c.indicatorId into all
                //                 from cc in all.DefaultIfEmpty()
                //                 join d in _context.Sites on cc.siteId equals d.siteId into all2
                //                 from dd in all2.DefaultIfEmpty()
                //                 join sd in _context.KPI_SiteInfo on new { dd.siteId, b.indicatorId } equals new { sd.siteId, sd.indicatorId } into all3
                //                 from sdd in all3.DefaultIfEmpty()
                //                 select new
                //                 {
                //                     a.groupId,
                //                     a.groupTitle,
                //                     a.groupCode,
                //                     groupWeight = a.weight,
                //                     b.indicatorId,
                //                     b.indicatorCode,
                //                     b.indicatorTitle,
                //                     indicatorWeight = sdd.weight,
                //                     sdd.unit,
                //                     cc.weightageId,
                //                     cc.siteId,
                //                     siteTitle = dd.siteName,
                //                     cc.value,
                //                     cc.month,
                //                     cc.year,
                //                     cc.comment,
                //                     sdd.measurementTitle,
                //                     sdd.annualTargetTitle,
                //                     sdd.classificationTitle
                //                 }).Distinct().ToListAsync();
                var kpi = await (from a in _context.KPI_IndicatorGroup.Where(a => a.isDeleted == 0)
                                 join ag in _context.KPI_UserGroup.Where(a => a.userId == reg.userId) on a.groupId equals ag.groupId
                                 join b in _context.KPI_Indicator on a.groupId equals b.groupId
                                 join sd in _context.KPI_SiteInfo.Where(a => a.siteId == reg.siteId && a.isDeleted == 0) on new { b.indicatorId } equals new { sd.indicatorId }
                                 join s in _context.Sites on sd.siteId equals s.siteId
                                 //join sss in _context.KPI_SiteList on s.siteId equals sss.KPISiteId
                                 join c in _context.KPI_IndicatorWeightage.Where(a => a.month == monthNumber && a.year == yearNumber) on sd.infoId equals c.siteInfoId into all
                                 from cc in all.DefaultIfEmpty()
                                 select new
                                 {
                                     a.groupId,
                                     a.groupTitle,
                                     a.groupCode,
                                     groupWeight = a.weight,
                                     sd.indicatorId,
                                     sd.infoId,
                                     b.indicatorCode,
                                     b.indicatorTitle,
                                     indicatorWeight = sd.weight,
                                     sd.factor,
                                     sd.unit,
                                     a.color,
                                     sd.formulaType,
                                     cc.weightageId,
                                     sd.siteId,
                                     siteTitle = s.siteName,
                                     cc.value,
                                     cc.month,
                                     cc.year,
                                     cc.comment,
                                     sd.measurementTitle,
                                     sd.annualTargetTitle,
                                     sd.classificationTitle,
                                     notApplicable = sd.applicable == 0 ? false : true,
                                     isParent = b.isParent == 0 ? false : true,
                                     isDisplay = b.isDisplay == 0 ? false : true,
                                 }).Distinct().ToListAsync();

                var result = kpi.Select(a => new
                {
                    a.groupId,
                    a.groupTitle,
                    a.groupCode,
                    a.groupWeight,
                    a.indicatorId,
                    a.infoId,
                    a.indicatorCode,
                    a.indicatorTitle,
                    a.indicatorWeight,
                    a.unit,
                    a.weightageId,
                    a.siteId,
                    a.siteTitle,
                    a.value,
                    a.month,
                    a.year,
                    a.comment,
                    a.measurementTitle,
                    a.factor,
                    a.formulaType,
                    a.color,
                    a.isParent,
                    a.isDisplay,
                    a.annualTargetTitle,
                    a.classificationTitle,
                    a.notApplicable,
                    weightedScore = calculateFactor((decimal)a.value, (decimal)a.factor, a.indicatorWeight, a.formulaType)
                }).OrderBy(a => a.groupId).OrderBy(a => a.indicatorCode).ToList();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveKPIIndicator")]
        public async Task<IActionResult> saveIndicator(KPIBusinessUserDto reg)
        {
            try
            {
                string sMonth = reg.monthId.ToString();
                string sYear = reg.yearId.ToString();
                var monthNumber = Convert.ToInt32(sMonth);
                var yearNumber = Convert.ToInt32(sYear);
                _context.Database.ExecuteSqlCommand("DELETE FROM KPI_IndicatorWeightage WHERE month = @addedDate AND siteId = @siteId AND year = @yearS",
                    new SqlParameter("addedDate", monthNumber),
                    new SqlParameter("siteId", reg.siteId),
                    new SqlParameter("yearS", yearNumber));
                for (var i = 0; i < reg.kpi.Length; i++)
                {
                    KPI_IndicatorWeightage eq = new KPI_IndicatorWeightage();
                    eq.siteInfoId = reg.kpi[i].infoId;
                    eq.month = monthNumber;
                    eq.year = yearNumber;
                    eq.value = (decimal)reg.kpi[i].value;
                    eq.annual = reg.kpi[i].annual;
                    eq.comment = reg.kpi[i].comment;
                    eq.siteId = reg.siteId;
                    _context.Add(eq);
                    var e = (from a in _context.KPI_SiteInfo.Where(a => a.infoId == reg.kpi[i].infoId)
                             select a).FirstOrDefault();

                    if (reg.kpi[i].notApplicable == true)
                    {
                        e.applicable = 1;
                    }
                    else if (reg.kpi[i].notApplicable == false)
                    {
                        e.applicable = 0;
                    }
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public decimal calculateFactor(decimal score, decimal factor, decimal weight, int formulaType)
        {
            try
            {
                decimal cc = 0;
                if (formulaType == 3)
                {
                    if (score > 0)
                    {
                        return cc;
                    }
                    else
                    {
                        return weight;
                    }
                }
                else
                {
                    var facCalc = (score / factor) * weight;
                    //if (facCalc > weight)
                    //{
                    //    cc = weight;
                    //}
                    //else
                    //{
                    cc = facCalc;
                    //}
                    return cc;
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during factor calculation.", e);
            }
        }
    }
}

