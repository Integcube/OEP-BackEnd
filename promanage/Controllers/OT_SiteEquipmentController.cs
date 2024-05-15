using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ActionTrakingSystem.Controllers
{

    public class OT_SiteEquipmentController : BaseAPIController
    {
        private readonly DAL _context;
        public OT_SiteEquipmentController(DAL context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {


                var equipFleetList = await (from r in _context.ModelEquipment.Where(a => a.isDeleted == 0)
                                            select new
                                            {
                                                r.fleetEquipId,
                                                r.title,
                                            }).OrderBy(a => a.title).ToListAsync();
                var equipFleetType = await (from r in _context.ModelEquipmentType.Where(a => a.isDeleted == 0)
                                            select new
                                            {
                                                r.typeTitle,
                                                r.typeId,
                                            }).OrderBy(a => a.typeTitle).ToListAsync();
                var equipFleetoem = await (from r in _context.ModelEquipmentOEM.Where(a => a.isDeleted == 0)
                                           select new
                                           {
                                               r.oemTitle,
                                               r.oemId,
                                           }).OrderBy(a => a.oemTitle).ToListAsync();
                var obj = new
                {

                    equipFleetList,
                    equipFleetType,
                    equipFleetoem
                };
                return Ok(obj);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getSiteEquipment")]
        public async Task<IActionResult> GetSiteEquipment(getSiteEquiomentDto reg)
        {
            try
            {
                DateTime totdayDate = DateTime.Now;
                var outages = await (from a in _context.OT_SiteNextOutages.Where(a => a.isDeleted == 0 && a.nextOutageDate.Date >= totdayDate.Date)
                                     join e in _context.OT_SiteEquipment on a.equipmentId equals e.equipmentId
                                     join s in _context.Sites.Where(a=>(a.otValid == 1)) on e.siteId equals s.siteId
                                     join o in _context.OT_ISiteOutages on a.outageId equals o.outageId
                                     select new
                                     {
                                         a.snoId,
                                         s.siteId,
                                         e.equipmentId,
                                         outageTypeId = o.outageId,
                                         a.nextOutageDate,
                                         title = o.outageTitle
                                     }).OrderBy(a => a.nextOutageDate).ToListAsync();

                var groupedOutages = outages.GroupBy(s => s.equipmentId).Select(g => new
                {
                    g.FirstOrDefault().equipmentId,
                    siteId = g.FirstOrDefault().siteId,
                    g.FirstOrDefault().outageTypeId,
                    g.FirstOrDefault().nextOutageDate,
                    g.FirstOrDefault().title
                }).ToList();

                var siteEquipment = await (from se in _context.OT_SiteEquipment.Where(a => a.isDeleted == 0 && (reg.filterObj.modelId == -1 || a.fleetEquipmentId == reg.filterObj.modelId) && (reg.filterObj.siteId == -1 || a.siteId == reg.filterObj.siteId))
                                           join s in _context.Sites.Where(a=> (a.isDeleted == 0)) on se.siteId equals s.siteId
                                           join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                           join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                           join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                           join c in _context.Country on s.countryId equals c.countryId
                                           join cc in _context.Cluster on s.clusterId equals cc.clusterId
                                           join r in _context.Regions2.Where(a => reg.filterObj.regionId == -1 || a.regionId == reg.filterObj.regionId) on cc.regionId equals r.regionId
                                           join f in _context.ModelEquipment on se.fleetEquipmentId equals f.fleetEquipId into allFleet
                                           from md in allFleet.DefaultIfEmpty()
                                           join ft in _context.ModelEquipmentType.Where(a => (reg.filterObj.equipmentTypeId == -1 || a.typeId == reg.filterObj.equipmentTypeId)) on md.equipmentTypeId equals ft.typeId
                                           join fe in _context.ModelEquipmentOEM.Where(a => (reg.filterObj.oemId == -1 || a.oemId == reg.filterObj.oemId)) on md.oemId equals fe.oemId
                                           join u in _context.AppUser on se.responsible equals u.userId into all5
                                           from ee in all5.DefaultIfEmpty()
                                           select new
                                           {
                                               equipmentId = se.equipmentId,
                                               regionId = r.regionId,
                                               cc.clusterId,
                                               cc.clusterTitle,
                                               regionTitle = r.title,
                                               siteId = se.siteId,
                                               siteTitle = s.siteName,
                                               model = md.title,
                                               modelId = md == null ? -1 : md.fleetEquipId,
                                               modelEquipmentType = ft.typeTitle,
                                               unitSN = se.unitSN,
                                               details = se.details,
                                               oemId = md.oemId,
                                               oemTitle = fe.oemTitle,
                                               responsible = se.responsible,
                                               responsibleName = ee.userName,
                                               unitCOD = se.unitCOD,
                                               unit = se.unit,
                                           }).Distinct().OrderByDescending(z => z.equipmentId).ToListAsync();


                var dataA = (from s in siteEquipment.Where(a => reg.filterObj.modelId == -1 || a.modelId == reg.filterObj.modelId)
                             join o in groupedOutages on s.equipmentId equals o.equipmentId into allOutage
                             from ot in allOutage.DefaultIfEmpty()
                             select new
                             {
                                 s.equipmentId,
                                 s.regionId,
                                 s.regionTitle,
                                 s.siteId,
                                 s.siteTitle,
                                 s.model,
                                 s.modelId,
                                 s.modelEquipmentType,
                                 s.unitSN,
                                 s.details,
                                 s.responsible,
                                 s.clusterId,
                                 s.clusterTitle,
                                 s.responsibleName,
                                 s.unitCOD,
                                 s.unit,
                                 s.oemId,
                                 s.oemTitle,
                                 outageTypeId = ot == null ? -1 : ot.outageTypeId,
                                 nextOutage = ot?.nextOutageDate,
                                 outageType = ot == null ? "" : ot.title


                             }).ToList();

                var obj = new
                {
                    siteEquipment = dataA,

                };
                return Ok(obj);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteSiteEquipment")]
        public async Task<IActionResult> DeleteEquipment(SiteEquipmentSaveDto reg)
        {
            try
            {
                OT_SiteEquipment equip = await (from r in _context.OT_SiteEquipment.Where(a => a.equipmentId == reg.equipment.equipmentId)
                                                select r).FirstOrDefaultAsync();
                equip.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(reg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("saveSiteEquipment")]
        public async Task<IActionResult> SaveEquipment(SiteEquipmentSaveDto reg)
        {
            try
            {
                if (reg.equipment.equipmentId == -1)
                {
                    OT_SiteEquipment siteEq = new OT_SiteEquipment();
                    siteEq.isDeleted = 0;
                    siteEq.createdDate = DateTime.Now;
                    siteEq.createdBy = reg.userId;
                    siteEq.siteId = reg.equipment.siteId;
                    siteEq.fleetEquipmentId = reg.equipment.modelId;
                    siteEq.unitSN = reg.equipment.unitSN;
                    //siteEq.nextOutage = reg.equipment.nextOutage;
                    //siteEq.outageTypeId = reg.equipment.outageTypeId;
                    siteEq.details = reg.equipment.details;
                    siteEq.responsible = reg.equipment.responsible;
                    siteEq.unitCOD = reg.equipment.unitCOD;
                    siteEq.unit = reg.equipment.unit;
                    _context.Add(siteEq);
                    _context.SaveChanges();
                    reg.equipment.equipmentId = siteEq.equipmentId;
                    return Ok(reg.equipment);
                }
                else
                {
                    OT_SiteEquipment siteEq = await (from r in _context.OT_SiteEquipment.Where(a => a.equipmentId == reg.equipment.equipmentId)
                                                     select r).FirstOrDefaultAsync();
                    siteEq.isDeleted = 0;
                    siteEq.modifiedDate = DateTime.Now;
                    siteEq.modifiedBy = reg.userId;
                    siteEq.siteId = reg.equipment.siteId;
                    siteEq.fleetEquipmentId = reg.equipment.modelId;
                    siteEq.unitSN = reg.equipment.unitSN;
                    //siteEq.nextOutage = reg.equipment.nextOutage;
                    //siteEq.outageTypeId = reg.equipment.outageTypeId;
                    siteEq.details = reg.equipment.details;
                    siteEq.responsible = reg.equipment.responsible;
                    siteEq.unitCOD = reg.equipment.unitCOD;
                    siteEq.unit = reg.equipment.unit;
                    _context.SaveChanges();
                    return Ok(reg.equipment);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
