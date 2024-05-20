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

    public class WH_ContractOutagesController : BaseAPIController
    {
        private readonly DAL _context;
        public WH_ContractOutagesController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getSiteNextOutages")]
        public async Task<IActionResult> GetContractOutages(WH_ContractOutageFilterData reg)
        {
            try
            {
                var siteOutages = await (from a in _context.WH_ContractOutages.Where(a => a.isDeleted == 0 && reg.filter.outageId == -1 || a.outageId == reg.filter.outageId)
                                         join e in _context.WH_SiteEquipment.Where(a=> a.isDeleted == 0 && (reg.filter.equipmentId == -1 || a.equipmentId == reg.filter.equipmentId)) on a.equipmentId equals e.equipmentId                             
                                         join s in _context.Sites.Where(a => (reg.filter.siteId == -1 || a.siteId == reg.filter.siteId) && ( reg.filter.clusterId == -1 || a.clusterId == reg.filter.clusterId)) on e.siteId equals s.siteId
                                         join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0) on s.siteId equals ts.siteId
                                         join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                         join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                         join ot in _context.WH_ISiteOutages on a.outageId equals ot.outageId
                                         join r in _context.Regions.Where(a => reg.filter.regionId == -1 || a.regionId == reg.filter.regionId) on s.regionId equals r.regionId
                                         join clus in _context.Cluster on s.clusterId equals clus.clusterId
                                         select new
                                         {
                                             a.contractOutageId,
                                             s.siteId,
                                             s.regionId,
                                             regionTitle = r.title,
                                             siteTitle = s.siteName,
                                             e.equipmentId,
                                             e.unit,
                                             e.siteUnit,
                                             ot.outageId,
                                             outageTitle = ot.outageTitle,
                                             nextOutageDate = a.nextOutageDate,
                                             cluster=clus.clusterTitle,
                                         }
                                        ).Distinct().OrderByDescending(a => a.contractOutageId).ToListAsync();
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

                var equipments = await (from a in _context.WH_SiteEquipment.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId))
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
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {
                var outageTypes = await (from a in _context.WH_ISiteOutages.Where(a => a.isDeleted == 0)
                                         select new
                                         {
                                             a.outageId,
                                             outageTitle = a.outageTitle,
                                         }
                                      ).OrderBy(a => a.outageTitle).ToListAsync();



                return Ok(outageTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveSiteNextOutages")]
        public async Task<IActionResult> SaveSiteNextOutages(WH_ContractOutageUser reg)
        {
            try
            {
                if (reg.contractOutage.contractOutageId == -1)
                {
                    WH_ContractOutages sn = new WH_ContractOutages();
                    sn.equipmentId = reg.contractOutage.equipmentId;
                    sn.outageId = reg.contractOutage.outageId;
                    sn.nextOutageDate = reg.contractOutage.nextOutageDate;
                    sn.createdOn = DateTime.Now;
                    sn.createdBy = reg.userId;
                    _context.Add(sn);
                    _context.SaveChanges();
                    reg.contractOutage.contractOutageId = sn.contractOutageId;

                    return Ok(reg.contractOutage);
                }
                else
                {
                    WH_ContractOutages sn = await (from a in _context.WH_ContractOutages.Where(a => a.contractOutageId == reg.contractOutage.contractOutageId)
                                                   select a).FirstOrDefaultAsync();
                    sn.equipmentId = reg.contractOutage.equipmentId;
                    sn.outageId = reg.contractOutage.outageId;
                    sn.nextOutageDate = reg.contractOutage.nextOutageDate;
                    sn.modifiedOn = DateTime.Now;
                    sn.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    return Ok(reg.contractOutage);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteSiteNextOutages")]
        public async Task<IActionResult> DeleteSiteNextOutages(WH_ContractOutageUser reg)
        {
            try
            {                
                    WH_ContractOutages sn = await (from a in _context.WH_ContractOutages.Where(a => a.contractOutageId == reg.contractOutage.contractOutageId)
                                                   select a).FirstOrDefaultAsync();
                    sn.isDeleted = 1;
                    _context.SaveChanges();
                    return Ok(reg.contractOutage);  
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
