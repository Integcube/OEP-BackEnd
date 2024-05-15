using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace ActionTrakingSystem.Controllers
{

    public class KPIConfiguratorController : BaseAPIController
    {
        private readonly DAL _context;
        public KPIConfiguratorController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getIndicatorGroup")]
        public async Task<IActionResult> GetIndicatorGroup(UserIdDto reg)
        {
            try
            {

                var kpi = await (from a in _context.KPI_IndicatorGroup.Where(a => a.isDeleted == 0)

                                 select new
                                 {
                                     a.groupId,
                                     a.groupTitle,
                                     a.groupCode,
                                     groupWeight = a.weight,
                                 }).Distinct().ToListAsync();

                return Ok(kpi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getFormula")]
        public async Task<IActionResult> GetFormula(UserIdDto reg)
        {
            try
            {

                var kpi = await (from a in _context.KPI_FormulaType.Where(a => a.isDeleted == 0)

                                 select new
                                 {
                                     a.formulaTypeId,
                                     a.formulaTitle,
                                     a.formulaCode,
                                 }).Distinct().ToListAsync();

                return Ok(kpi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getIndicator")]
        public async Task<IActionResult> GetIndicator(GetKPIIndicatorDto reg)
        {
            try
            {

                var kpi = await (from a in _context.KPI_Indicator.Where(a => a.isDeleted == 0 && (reg.groupId == -1 || a.groupId == reg.groupId))

                                 select new
                                 {
                                     a.groupId,
                                     a.indicatorId,
                                     a.indicatorTitle,
                                     a.indicatorCode,
                                     a.isParent,
                                     a.isDisplay,
                                 }).Distinct().ToListAsync();

                return Ok(kpi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveIndicator")]
        public async Task<IActionResult> SaveIndicator(SaveKPIIndicatorDto reg)
        {
            try
            {
                KPI_Indicator indicator = new KPI_Indicator();
                indicator.groupId = reg.groupId;
                indicator.indicatorTitle = reg.indicatorTitle;
                indicator.indicatorCode = reg.indicatorCode;
                indicator.isParent = reg.isParent == null ? 0 : reg.isParent == true ? 1 : 0;
                indicator.isDisplay = reg.isParent == null ? 0 : reg.isParent == true ? 1 : 0;
                indicator.isDeleted = 0;
                _context.Add(indicator);
                _context.SaveChanges();
                return Ok(indicator);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getKPIsConfigurations")]
        public async Task<IActionResult> GetKPIConfigurations(GetKPIConfigListDto reg)
        {
            try
            {

                var kpi = await (from a in _context.KPI_IndicatorGroup.Where(a => a.isDeleted == 0)
                                 join ag in _context.KPI_UserGroup.Where(a => a.userId == reg.userId) on a.groupId equals ag.groupId
                                 join b in _context.KPI_Indicator on a.groupId equals b.groupId
                                 join sd in _context.KPI_SiteInfo.Where(a => a.siteId == reg.siteId && a.isDeleted == 0) on new { b.indicatorId } equals new { sd.indicatorId }
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
                                     infoWeight = sd.weight,
                                     sd.factor,
                                     sd.unit,
                                     sd.formulaType,
                                     sd.siteId,
                                     sd.measurementTitle,
                                     sd.annualTargetTitle,
                                     sd.classificationTitle,
                                     isParent = b.isParent==0?false:true,
                                     isDisplay = b.isDisplay== 0 ? false : true,
                                 }).Distinct().OrderBy(a => a.indicatorCode).ToListAsync();

                return Ok(kpi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveKpi")]
        public async Task<IActionResult> SaveKPI(KPIConfiguratorDto reg)
        {
            try
            {

                if (reg.infoId != -1)
                {
                    KPI_SiteInfo info = await (from a in _context.KPI_SiteInfo.Where(a => a.infoId == reg.infoId)
                                               select a).FirstOrDefaultAsync();
                    info.indicatorId = reg.indicatorId;
                    info.measurementTitle = reg.measurementTitle;
                    info.annualTargetTitle = reg.annualTargetTitle;
                    info.factor = reg.factor;
                    info.weight = reg.infoWeight;
                    info.unit = reg.unit;
                    info.classificationTitle = reg.classificationTitle;
                    info.formulaType = reg.formulaType;
                    _context.SaveChanges();
                }
                else
                {
                    KPI_SiteInfo info = new KPI_SiteInfo();
                    info.siteId = reg.siteId;
                    info.indicatorId = reg.indicatorId;
                    info.measurementTitle = reg.measurementTitle;
                    info.annualTargetTitle = reg.annualTargetTitle;
                    info.factor = reg.factor;
                    info.unit = reg.unit;
                    info.weight = reg.infoWeight;
                    info.classificationTitle = reg.classificationTitle;
                    info.formulaType = reg.formulaType;
                    info.isDeleted = 0;
                    _context.Add(info);
                    _context.SaveChanges();
                }

                return Ok(reg);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteKpi")]
        public async Task<IActionResult> DeleteKPI(KPIConfiguratorDto reg)
        {
            try
            {


                KPI_SiteInfo info = await (from a in _context.KPI_SiteInfo.Where(a => a.infoId == reg.infoId)
                                           select a).FirstOrDefaultAsync();
                info.isDeleted = 1;
                _context.SaveChanges();



                return Ok(reg);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
