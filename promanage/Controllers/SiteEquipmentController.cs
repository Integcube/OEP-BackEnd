using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class SiteEquipmentController : BaseAPIController
    {
        private readonly DAL _context;
        public SiteEquipmentController(DAL context)
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
                                            }).ToListAsync();
                var equipFleetType = await (from r in _context.ModelEquipmentType.Where(a => a.isDeleted == 0)
                                            select new
                                            {
                                                r.typeTitle,
                                                r.typeId,
                                            }).ToListAsync();
                var equipFleetoem = await (from r in _context.ModelEquipmentOEM.Where(a => a.isDeleted == 0)
                                            select new
                                            {
                                                r.oemTitle,
                                                r.oemId,
                                            }).ToListAsync();
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
        public async Task<IActionResult> GetSiteEquipment(getSiteEquiomentFilterDto reg)
        {
            try
            {
                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());

                List<int> ClusterIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.clusterList))
                    ClusterIds = (reg.clusterList.Split(',').Select(Int32.Parse).ToList());


                List<int> SitesIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.siteList))
                    SitesIds = (reg.siteList.Split(',').Select(Int32.Parse).ToList());


                List<int> ModelIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.modelList))
                    ModelIds = (reg.modelList.Split(',').Select(Int32.Parse).ToList());

                List<int> EqIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.eqTypeList))
                    EqIds = (reg.eqTypeList.Split(',').Select(Int32.Parse).ToList());

                List<int> OemIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.oemList))
                    OemIds = (reg.oemList.Split(',').Select(Int32.Parse).ToList());

                DateTime totdayDate = DateTime.Now;
                var outages = await (from a in _context.SiteNextOutages.Where(a => a.isDeleted == 0 && a.nextOutageDate.Date >= totdayDate.Date)
                                     join e in _context.SiteEquipment on a.equipmentId equals e.equipmentId
                                     join s in _context.Sites on e.siteId equals s.siteId
                                     join o in _context.OutageTypes on a.outageId equals o.outageTypeId
                                     select new
                                     {
                                         a.snoId,
                                         s.siteId,
                                         e.equipmentId,
                                         o.outageTypeId,
                                         a.nextOutageDate,
                                         o.title
                                     }).OrderBy(a => a.nextOutageDate).ToListAsync();

                var groupedOutages = outages.GroupBy(s => s.equipmentId).Select(g => new
                {
                    g.FirstOrDefault().equipmentId,
                    siteId = g.FirstOrDefault().siteId,
                    g.FirstOrDefault().outageTypeId,
                    g.FirstOrDefault().nextOutageDate,
                    g.FirstOrDefault().title
                }).ToList();

                var siteEquipment = await (from se in _context.SiteEquipment.Where(a => a.isDeleted == 0 && ((ModelIds.Count == 0) || ModelIds.Contains((int)a.fleetEquipmentId)) && ((SitesIds.Count == 0) || SitesIds.Contains((int)a.siteId)))
                                           join s in _context.Sites on se.siteId equals s.siteId
                                           join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                           join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                           join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                           join r in _context.Regions2.Where(a => (RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId)) on s.region2 equals r.regionId
                                           join f in _context.ModelEquipment on se.fleetEquipmentId equals f.fleetEquipId into allFleet
                                           from md in allFleet.DefaultIfEmpty()
                                           join ft in _context.ModelEquipmentType.Where(a => ((EqIds.Count == 0) || EqIds.Contains((int)a.typeId))) on md.equipmentTypeId equals ft.typeId
                                           join fe in _context.ModelEquipmentOEM.Where(a => ((OemIds.Count == 0) || OemIds.Contains((int)a.oemId))) on md.oemId equals fe.oemId
                                           join u in _context.AppUser on se.responsible equals u.userId into all5
                                           from ee in all5.DefaultIfEmpty()
                                           join cll in _context.Cluster.Where(a => a.isDeleted == 0 && ((ClusterIds.Count == 0) || ClusterIds.Contains((int)a.clusterId))) on s.clusterId equals cll.clusterId
                                           select new
                                           {
                                               equipmentId = se.equipmentId,
                                               regionId = r.regionId,
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
                                               cll.clusterTitle,
                                               s.clusterId,
                                           }).Distinct().OrderByDescending(z => z.equipmentId).ToListAsync();


                var dataA = (from s in siteEquipment
                             //.Where(a => reg.filterObj.modelId == -1 || a.modelId == reg.filterObj.modelId)
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
                SiteEquipment equip = await (from r in _context.SiteEquipment.Where(a => a.equipmentId == reg.equipment.equipmentId)
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
                    SiteEquipment siteEq = new SiteEquipment();
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
                    siteEq.siteUnit = reg.equipment.siteTitle + "-" + reg.equipment.unit;
                    _context.Add(siteEq);
                    _context.SaveChanges();
                    reg.equipment.equipmentId = siteEq.equipmentId;
                    return Ok(reg.equipment);
                }
                else
                {
                    SiteEquipment siteEq = await (from r in _context.SiteEquipment.Where(a => a.equipmentId == reg.equipment.equipmentId)
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
                    siteEq.siteUnit = reg.equipment.siteTitle + "-" + reg.equipment.unit;
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
