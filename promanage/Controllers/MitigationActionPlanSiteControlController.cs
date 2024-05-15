using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class MitigationActionPlanSiteControlController : BaseAPIController
    {
        private readonly DAL _context;
        public MitigationActionPlanSiteControlController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getSiteControl")]
        public async Task<IActionResult> GetSiteControl(UserIdDto reg)
        {
            try
            {
                var site = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                  join st in _context.SitesTechnology on s.siteId equals st.siteId
                                  join r in _context.Regions on s.regionId equals r.regionId
                                  join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                  join pt in _context.OMA_ProgramTechnologies on st.techId equals pt.technologyId
                                  join p in _context.OMA_IProgram on pt.programId equals p.programId
                                  join sc in _context.OMA_SiteControl on st.siteId equals sc.siteId into all
                                  from scc in all.DefaultIfEmpty()
                                  select new
                                  {
                                      r.regionId,
                                      regionTitle = r.title,
                                      s.siteId,
                                      siteTitle = s.siteName,
                                      p.programTitle,
                                  }
                                  ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();
                var siteData = site.GroupBy(u => u.siteId).Select(group => new
                {
                    siteId = group.Key,
                    regionId = group.FirstOrDefault().regionId,
                    regionTitle = group.FirstOrDefault().regionTitle,
                    siteTitle = group.FirstOrDefault().siteTitle,
                    programTitle = string.Join(", ", group.Select(u => u.programTitle))
                }).ToList();
                return Ok(siteData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSelectedSites")]
        public async Task<IActionResult> GetSelectedSites(OMA_GetSiteControlDto reg)
        {
            try
            {
                var site = await (from s in _context.Sites.Where(a => a.siteId == reg.siteId)
                                  join st in _context.SitesTechnology on s.siteId equals st.siteId
                                  join t in _context.Technology.Where(a => a.isDeleted == 0) on st.techId equals t.techId
                                  join pt in _context.OMA_ProgramTechnologies on st.techId equals pt.technologyId
                                  join p in _context.OMA_IProgram.Where(a=>a.isDeleted == 0) on pt.programId equals p.programId
                                  join sc in _context.OMA_SiteControl on new { st.siteId, pt.programId, pt.technologyId } equals new { sc.siteId, sc.programId, sc.technologyId } into all
                                  from scc in all.DefaultIfEmpty()
                                  select new
                                  {
                                      //st.techId,
                                      //technologyTitle = t.name,
                                      s.siteId,
                                      siteTitle = s.siteName,
                                      pt.programId,
                                      p.programTitle,
                                      siteControlId = scc == null?-1: scc.siteControlId,
                                      selected = (scc.siteControlId == null )?false:true
                                  }
                                  ).Distinct().OrderBy(a => a.programTitle).ToListAsync();

                return Ok(site);



            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveSiteControl")]
        public async Task<IActionResult> SaveSiteControl(OMA_SaveSiteControlDto reg)
        {
            try
            {
               
                _context.Database.ExecuteSqlCommand("DELETE FROM OMA_SiteControl WHERE siteId = @siteId", new SqlParameter("siteId", reg.siteId));
                for(var a = 0; a<reg.siteControl.Length; a++)
                {
                    if (reg.siteControl[a].selected == true)
                    {
                        SaveSiteRow(_context, reg.siteControl[a].programId, reg.siteId);

                    }

                }


                return Ok(reg.siteControl);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public static void SaveSiteRow(DAL dal, int programId, int siteId)
        {

            dal.Database.ExecuteSqlRaw("EXEC udpOMA_SiteControlSave @ProgramId, @SiteId",
        new SqlParameter("@ProgramId", programId),
        new SqlParameter("@SiteId", siteId));

        }
    }
}
