using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ActionTrakingSystem.Controllers
{

    public class OT_NextOutagesController : BaseAPIController
    {
        private readonly DAL _context;
        public OT_NextOutagesController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("getSiteNextOutages")]
        public async Task<IActionResult> GetSiteNextOutages(OT_GetNextOutageDto reg)
        {
            try
            {
                DateTime filterDate = new DateTime(2023, 12, 31);
                var siteOutages = await (from a in _context.OT_SiteNextOutages.Where(a => a.isDeleted == 0 && a.nextOutageDate > filterDate && (reg.filter.startDate == null || a.nextOutageDate >= reg.filter.startDate) && (reg.filter.endDate == null || a.nextOutageDate <= reg.filter.endDate))
                                         join e in _context.OT_SiteEquipment.Where(a=>reg.filter.equipmentId == -1 || a.equipmentId == reg.filter.equipmentId) on a.equipmentId equals e.equipmentId
                                         join s in _context.Sites.Where(a=>(a.otValid== 1)&& (reg.filter.siteId == -1 || a.siteId == reg.filter.siteId)) on e.siteId equals s.siteId
                                         join ot in _context.OT_ISiteOutages.Where(a=>reg.filter.outageId == -1 || a.outageId == reg.filter.outageId) on a.outageId equals ot.outageId
                                         join c in _context.Country on s.countryId equals c.countryId
                                         join cc in _context.Cluster.Where(a => reg.filter.clusterId == -1 || a.clusterId == reg.filter.clusterId) on s.clusterId equals cc.clusterId
                                         join r in _context.Regions2.Where(a => reg.filter.regionId == -1 || a.regionId == reg.filter.regionId) on cc.regionId equals r.regionId
                                         join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                         select new
                                         {
                                             a.snoId,
                                             a.outageDurationInDays,
                                             s.siteId,
                                             s.regionId,
                                             regionTitle = r.title,
                                             siteTitle = s.siteName,
                                             e.equipmentId,
                                             e.unit,
                                             cc.clusterId,
                                             cc.clusterTitle,
                                             e.siteUnit,
                                             ot.outageId,
                                             outageTitle = ot.outageTitle,
                                             nextOutageDate = a.nextOutageDate
                                         }
                                        ).OrderBy(a => a.nextOutageDate).ToListAsync();
                return Ok(siteOutages);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getEquipments")]
        public async Task<IActionResult> GetEquipments(OT_GetEquipments reg)
        {
            try
            {

                var equipments = await (from a in _context.OT_SiteEquipment.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId))
                                        join s in _context.Sites.Where(a=>(a.otValid == 1)) on a.siteId equals s.siteId
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
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var outageTypes = await (from a in _context.OT_ISiteOutages.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             a.outageId,
                                             outageTitle = a.outageTitle,
                                         }
                                      ).OrderBy(a=>a.outageTitle).ToListAsync();

                return Ok(outageTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveSiteNextOutages")]
        public async Task<IActionResult> SaveSiteNextOutages(OT_SiteNextOutageUser reg)
        {
            try
            {
                if (reg.siteNextOutage.snoId == -1)
                {
                    OT_SiteNextOutages sn = new OT_SiteNextOutages();
                    sn.equipmentId = reg.siteNextOutage.equipmentId;
                    sn.outageId = reg.siteNextOutage.outageId;
                    sn.nextOutageDate = DateTime.Parse(reg.siteNextOutage.nextOutageDate); 
                    sn.createdOn = DateTime.Now;
                    sn.outageDurationInDays = reg.siteNextOutage.outageDurationInDays;
                    sn.createdBy = reg.userId;
                    _context.Add(sn);
                    _context.SaveChanges();
                    reg.siteNextOutage.snoId = sn.snoId;

                    return Ok(reg.siteNextOutage);
                }
                else
                {
                    OT_SiteNextOutages sn = await (from a in _context.OT_SiteNextOutages.Where(a => a.snoId == reg.siteNextOutage.snoId)
                                                select a).FirstOrDefaultAsync();
                    sn.equipmentId = reg.siteNextOutage.equipmentId;
                    sn.outageId = reg.siteNextOutage.outageId;
                    sn.nextOutageDate = DateTime.Parse(reg.siteNextOutage.nextOutageDate);
                    sn.outageDurationInDays = reg.siteNextOutage.outageDurationInDays;
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
        public async Task<IActionResult> DeleteSiteNextOutages(OT_SiteNextOutageUser reg)
        {
            try
            {

                {
                    OT_SiteNextOutages sn = await (from a in _context.OT_SiteNextOutages.Where(a => a.snoId == reg.siteNextOutage.snoId)
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
