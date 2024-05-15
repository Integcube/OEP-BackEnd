using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class SiteNextOutagesController : BaseAPIController
    {
        private readonly DAL _context;
        public SiteNextOutagesController(DAL context)
        {
            _context= context;
        }
        [Authorize]
        [HttpPost("getEquipments")]
        public async Task<IActionResult> GetEquipments(OT_GetEquipments reg)
        {
            try
            {

                var equipments = await (from a in _context.SiteEquipment.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId))
                                        join s in _context.Sites on a.siteId equals s.siteId
                                        select new
                                        {

                                            a.equipmentId,
                                            a.unit,
                                            a.siteUnit,
                                            s.siteId,
                                            siteTitle = s.siteName
                                        }
                                    ).OrderBy(a => a.siteTitle).ToListAsync();


                return Ok(equipments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSiteNextOutages")]
        public async Task<IActionResult> GetSiteNextOutages(UserIdDto reg)
        {
            try
            {
                

                var siteOutages = await (from a in _context.SiteNextOutages.Where(a => a.isDeleted == 0)
                                         join e in _context.SiteEquipment on a.equipmentId equals e.equipmentId
                                         join s in _context.Sites on e.siteId equals s.siteId
                                         join ot in _context.OutageTypes on a.outageId equals ot.outageTypeId
                                         select new
                                         {
                                             a.snoId,
                                             s.siteId,
                                             siteTitle = s.siteName,
                                             e.equipmentId,
                                             e.unit,
                                             e.siteUnit,
                                             ot.outageTypeId,
                                             outageTitle = ot.title,
                                             outageLevel = ot.levelOutage,
                                             nextOutageDate = a.nextOutageDate
                                         }
                                       ).OrderByDescending(a => a.snoId).ToListAsync();
                return Ok(siteOutages);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var outageTypes = await (from a in _context.OutageTypes.Where(a=>a.isDeleted == 0)
                                         select new
                                         {
                                            
                                             a.outageTypeId,
                                             outageTitle = a.title,
                                             outageLevel = a.levelOutage

                                         }
                                      ).ToListAsync();
                return Ok(outageTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveSiteNextOutages")]
        public async Task<IActionResult> SaveSiteNextOutages(SiteNextOutageUser reg)
        {
            try
            {
                if (reg.siteNextOutage.snoId == -1)
                {
                    SiteNextOutages sn = new SiteNextOutages();
                    sn.equipmentId = reg.siteNextOutage.equipmentId;
                    sn.outageId = reg.siteNextOutage.outageTypeId;
                    sn.nextOutageDate = reg.siteNextOutage.nextOutageDate;
                    sn.createdOn = DateTime.Now;
                    sn.createdBy = reg.userId;
                    _context.Add(sn);
                    _context.SaveChanges();
                    reg.siteNextOutage.snoId = sn.snoId;

                    return Ok(reg.siteNextOutage);
                }
                else
                {
                    SiteNextOutages sn = await (from a in _context.SiteNextOutages.Where(a => a.snoId == reg.siteNextOutage.snoId)
                                                   select a).FirstOrDefaultAsync();
                    sn.equipmentId = reg.siteNextOutage.equipmentId;
                    sn.outageId = reg.siteNextOutage.outageTypeId;
                    sn.nextOutageDate = reg.siteNextOutage.nextOutageDate;
                    sn.modifiedOn = DateTime.Now;
                    sn.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    return Ok(reg.siteNextOutage);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteSiteNextOutages")]
        public async Task<IActionResult> DeleteSiteNextOutages(SiteNextOutageUser reg)
        {
            try
            {
               
                {
                    SiteNextOutages sn = await (from a in _context.SiteNextOutages.Where(a => a.snoId == reg.siteNextOutage.snoId)
                                                select a).FirstOrDefaultAsync();
                    sn.isDeleted = 1;
                    _context.SaveChanges();
                    return Ok(reg.siteNextOutage);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
