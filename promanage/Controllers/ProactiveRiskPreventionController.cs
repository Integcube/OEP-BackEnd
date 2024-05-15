using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class ProactiveRiskPreventionController : BaseAPIController
    {
        private readonly DAL _context;
        public ProactiveRiskPreventionController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("saveProactive")]
        public async Task<IActionResult> SaveProactive(saveProactiveDto reg)
        {
            try
            {
                if(reg.obj.proactive.proactiveId == -1)
                {
                    ProactiveRiskPrevention pv = new ProactiveRiskPrevention();
                    pv.proactivetitle = reg.obj.proactive.proactivetitle;
                    pv.proactiveReference = reg.obj.proactive.proactiveReference ?? "";
                    pv.criticalityId = reg.obj.proactive.criticalityId;
                    pv.categoryId = reg.obj.proactive.categoryId;
                    pv.exposureId = reg.obj.proactive.exposureId;
                    pv.recommendations = reg.obj.proactive.recommendations;
                    pv.guidelines = reg.obj.proactive.guidelines;
                    pv.details = reg.obj.proactive.details;
                    pv.sourceId = reg.obj.proactive.sourceId;
                    pv.themeId = reg.obj.proactive.themeId;
                    pv.approachStatusId= reg.obj.proactive.approachStatusId;
                    pv.auditPreperatoryChecklist = reg.obj.proactive.auditPreperatoryChecklist;
                    pv.createdOn = DateTime.Now;
                    pv.createdBy = reg.userId;
                    pv.isDeleted = 0;
                    _context.Add(pv);
                    _context.SaveChanges();
                    reg.obj.proactive.proactiveId = pv.proactiveId;
                    for (var a = 0; a < reg.obj.projectPhase.Count; a++)
                    {
                        proactiveProjectPhaseDetail c = new proactiveProjectPhaseDetail();
                        c.proactiveId = pv.proactiveId;
                        c.ppId = reg.obj.projectPhase[a].projectPhaseId;
                        c.createdOn = DateTime.Now;
                        c.createdBy = reg.userId;
                        _context.Add(c);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    var pv = await (from a in _context.ProactiveRiskPrevention.Where(a => a.proactiveId == reg.obj.proactive.proactiveId)
                                           select a).FirstOrDefaultAsync();
                    pv.proactivetitle = reg.obj.proactive.proactivetitle;
                    pv.proactiveReference = reg.obj.proactive.proactiveReference?? "";
                    pv.criticalityId = reg.obj.proactive.criticalityId;
                    pv.categoryId = reg.obj.proactive.categoryId;
                    pv.exposureId = reg.obj.proactive.exposureId;
                    pv.recommendations = reg.obj.proactive.recommendations;
                    pv.guidelines = reg.obj.proactive.guidelines;
                    pv.approachStatusId = reg.obj.proactive.approachStatusId;
                    pv.details = reg.obj.proactive.details;
                    pv.sourceId = reg.obj.proactive.sourceId;
                    pv.themeId = reg.obj.proactive.themeId;
                    pv.auditPreperatoryChecklist = reg.obj.proactive.auditPreperatoryChecklist;
                    pv.modifiedOn = DateTime.Now;
                    pv.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM proactiveProjectPhaseDetail WHERE proactiveId = @regionId", new SqlParameter("regionId", reg.obj.proactive.proactiveId));
                    for (var a = 0; a < reg.obj.projectPhase.Count; a++)
                    {
                        proactiveProjectPhaseDetail c = new proactiveProjectPhaseDetail();
                        c.proactiveId = pv.proactiveId;
                        c.ppId = reg.obj.projectPhase[a].projectPhaseId;
                        c.createdOn = DateTime.Now;
                        c.createdBy = reg.userId;
                        _context.Add(c);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.obj.proactive);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetIntefaces(UserDto reg)
        {
            try
            {
                var proactiveCriticality = await (from a in _context.ProactiveCriticality.Where(a=>a.isDeleted == 0)
                                                  select a).ToListAsync();
                var proactiveCategory = await (from a in _context.ProactiveCategory.Where(a => a.isDeleted == 0)
                                               select a).ToListAsync();
                var proactiveExposure = await (from a in _context.ProactiveExposure.Where(a => a.isDeleted == 0)
                                               select a).ToListAsync();
                var proactiveApproachStatus = await (from a in _context.ProactiveApproachStatus.Where(a => a.isDeleted == 0)
                                               select a).ToListAsync();
                var proactiveProjectPhase = await (from a in _context.ProactiveProjectPhase.Where(a => a.isDeleted == 0)
                                               select a).ToListAsync();
                var proactiveProactiveSource = await (from a in _context.ProactiveSource.Where(a => a.isDeleted == 0)
                                                         select a).ToListAsync();
                var proactiveProactiveTheme = await (from a in _context.ProactiveTheme.Where(a => a.isDeleted == 0)
                                                      select a).ToListAsync();

                var obj = new
                {
                    proactiveCriticality,
                    proactiveCategory,
                    proactiveExposure,
                    proactiveApproachStatus,
                    proactiveProjectPhase,
                    proactiveProactiveSource,
                    proactiveProactiveTheme
                };
                
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSpecificObject")]
        public async Task<IActionResult> GetSpecificObject(ProactiveRiskPreventionDto reg)
        {
            try
            {
                var proactiveProjectPhaseDetail = await (from a in _context.proactiveProjectPhaseDetail.Where(a => a.proactiveId == reg.proactiveId)
                                                         join p in _context.ProactiveProjectPhase on a.ppId equals p.projectPhaseId
                                                         select new
                                                         {
                                                             p.projectPhaseId,
                                                             p.projectPhaseTitle
                                                         }).ToListAsync();
                return Ok(proactiveProjectPhaseDetail);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getProactives")]
        public async Task <IActionResult> GetProactives(UserDto reg)
        {
            try
            {
                var proactives = await (from a in _context.ProactiveRiskPrevention.Where(a => a.isDeleted == 0)
                                        join b in _context.ProactiveCriticality on a.criticalityId equals b.criticalityId into alll1
                                        from bb in alll1.DefaultIfEmpty()
                                        join c in _context.ProactiveCategory on a.categoryId equals c.categoryId into alll2
                                        from cc in alll2.DefaultIfEmpty()
                                        join d in _context.ProactiveExposure on a.exposureId equals d.exposureId into alll3
                                        from dd in alll3.DefaultIfEmpty()
                                        join zx in _context.ProactiveApproachStatus on a.approachStatusId equals zx.approachStatusId into alllX
                                        from zxx in alllX.DefaultIfEmpty()
                                        join f in _context.ProactiveSource on a.sourceId equals f.sourceId into alll4
                                        from ff in alll4.DefaultIfEmpty()
                                        join g in _context.ProactiveTheme on a.themeId equals g.themeId into alll5
                                        from gg in alll5.DefaultIfEmpty()
                                        join i in _context.InsuranceRecommendations on a.proactiveId equals i.proactiveId into all
                                        from ii in all.DefaultIfEmpty()
                                        join s in _context.Sites on ii.siteId equals s.siteId into all2
                                        from ss in all2.DefaultIfEmpty()

                                        select new
                                        {
                                            a.proactiveId,
                                            a.proactiveReference,
                                            a.proactivetitle,
                                            a.recommendations,
                                            a.details,
                                            a.guidelines,
                                            a.approachStatusId,
                                            zxx.approachStatusTitle,
                                            a.auditPreperatoryChecklist,
                                            a.criticalityId,
                                            bb.criticalityTitle,
                                            a.categoryId,
                                            cc.categoryTitle,
                                            a.exposureId,
                                            dd.exposureTitle,
                                            a.sourceId,
                                            ff.sourceTitle,
                                            a.themeId, gg.themeTitle,
                                            ii.siteId,
                                            ss.siteName
                                        }).OrderByDescending(a=>a.proactiveId).ToListAsync();
                var groupedData = proactives.GroupBy(b => b.proactiveId).Select(a => new
                {
                    a.FirstOrDefault().proactiveId,
                    a.FirstOrDefault().proactiveReference,
                    a.FirstOrDefault().proactivetitle,
                    a.FirstOrDefault().recommendations,
                    a.FirstOrDefault().guidelines,
                    a.FirstOrDefault().auditPreperatoryChecklist,
                    a.FirstOrDefault().criticalityId,
                    a.FirstOrDefault().criticalityTitle,
                    a.FirstOrDefault().categoryId,
                    a.FirstOrDefault().categoryTitle,
                    a.FirstOrDefault().exposureId,
                    a.FirstOrDefault().exposureTitle,
                    a.FirstOrDefault().sourceId,
                    a.FirstOrDefault().approachStatusId,
                    a.FirstOrDefault().approachStatusTitle,
                    a.FirstOrDefault().sourceTitle,
                    a.FirstOrDefault().themeId,
                    a.FirstOrDefault().themeTitle,
                    a.FirstOrDefault().details,
                    sites = string.Join(", ", a.Select(u => u.siteName))
                }).ToList();

                return Ok(groupedData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("deleteProactive")]
        public async Task<IActionResult> DeleteProactive(ProactiveRiskPreventionDto reg)
        {
            try
            {
                var proactive = await (from a in _context.ProactiveRiskPrevention.Where(a => a.proactiveId == reg.proactiveId)
                                       select a).FirstOrDefaultAsync();
                proactive.isDeleted = 1;
                _context.SaveChanges();
                return Ok(reg);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
