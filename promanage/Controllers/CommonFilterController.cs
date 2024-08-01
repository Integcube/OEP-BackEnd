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
    public class CommonFilterController : BaseAPIController
    {
        public readonly  DAL _context;
        public CommonFilterController(DAL context) 
        { 
            _context = context;
        }

        [Authorize]
        [HttpPost("getUsers")]
        public async Task<IActionResult> GetUsers(CommonFilterDto reg)
        {
            try
            {
                var users = await (from s in _context.AppUser.Where(a => a.isDeleted == 0)
                                   join aus in _context.AUSite on s.userId equals aus.userId
                                   join a in _context.Sites.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId)) on aus.siteId equals a.siteId
                             
                                   select new
                                   {
                                       s.userId,
                                       s.userName,
                                       name =s.firstName + " " + s.lastName,
                                       s.email,
                                   }
                                  ).Distinct().OrderBy(a=>a.name).ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getRegionsOM")]
        public async Task<IActionResult> GetRegionsOM(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from r in _context.Regions.Where(a => a.isDeleted == 0)
                                          join s in _context.Sites on r.regionId equals s.regionId
                                          join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                          join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                          select new
                                          {
                                              regionTitle = r.title,
                                              r.regionId,
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getAllRegions")]
        public async Task<IActionResult> GetAllRegions(UserIdDto reg)
        {
            try
            {

                var equipRegions = await (from r in _context.Regions.Where(a => a.isDeleted == 0)
                                         
                                          select new
                                          {
                                              regionTitle = r.title,
                                              r.regionId,
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSitesOM")]
        public async Task<IActionResult> GetSitesOM(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))
                                  
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }
                                  ).Distinct().OrderBy(a=>a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSitesOMSpecific")]
        public async Task<IActionResult> GetSitesOMSpecific(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))

                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join ss in _context.OMA_SiteControl on s.siteId equals ss.siteId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }
                                  ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSitesOMSpecificReviewer")]
        public async Task<IActionResult> GetSitesOMSpecificReviewer(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))

                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join ss in _context.OMA_SiteControl on s.siteId equals ss.siteId
                                   join sb in _context.OMA_MitigationAction on ss.programId equals sb.programId
                                   join r in _context.OMA_MitigationResult.Where(a=>a.statusId == 4 || a.statusId == 1 || a.statusId == 5) on sb.actionId equals r.actionId
                                   select new
                                   {
                                       s.siteId,
                                       siteTitle = s.siteName,
                                   }
                                  ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSites")]
        public async Task<IActionResult> GetSites(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1) && (a.clusterId == reg.clusterId || reg.clusterId == -1))
                                   join r in _context.Regions on s.regionId equals r.regionId
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId || reg.userId == -1) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       r.regionId,
                                       regionTitle = r.title,
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }
                                  ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        [Authorize]
        [HttpPost("getKPISites")]
        public async Task<IActionResult> GetKPISites(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))
                           
                                   join r in _context.Regions on s.regionId equals r.regionId
                                   join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId || reg.userId == -1) on stech.techId equals aut.technologyId
                                   select new
                                   {
                                       r.regionId,
                                       regionTitle = r.title,
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }
                                  ).Distinct().OrderBy(a => a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getRegions")]
        public async Task<IActionResult> GetRegions(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from r in _context.Regions.Where(a => a.isDeleted == 0)
                                          join s in _context.Sites on r.regionId equals s.regionId
                                          join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                          join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                          join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                          select new
                                          {
                                              regionTitle = r.title,
                                              r.regionId,
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getClusters")]
        public async Task<IActionResult> GetCluster(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Regions2.Where(a => a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1))
                                   join c in _context.Cluster.Where(a=>a.isDeleted == 0 &&(a.clusterId == reg.clusterId || reg.clusterId == -1)) on s.regionId equals c.regionId
                                   
                                   select new
                                   {
                                       c.clusterId,
                                       c.clusterTitle
                                   }
                                  ).Distinct().OrderBy(a => a.clusterTitle).ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getUpdatedSites")]
        public async Task<IActionResult> GetUpdatedSites(CommonFilterDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                   join cc in _context.Cluster.Where(a => a.isDeleted == 0 && (a.clusterId == reg.clusterId || reg.clusterId == -1)) on s.clusterId equals cc.clusterId
                                   join r in _context.Regions2.Where(a=>a.isDeleted == 0 && (a.regionId == reg.regionId || reg.regionId == -1)) on cc.regionId equals r.regionId
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId == -1) on s.siteId equals aus.siteId
                                   select new
                                   {
                                       r.regionId,
                                       regionTitle = r.title,
                                       s.siteId,
                                       siteTitle = s.siteName
                                   }
                                  ).Distinct().OrderBy(a=>a.siteTitle).ToListAsync();

                return Ok(sites);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("getUpdatedRegions")]
        public async Task<IActionResult> GetUpdatedRegions(CommonFilterDto reg)
        {
            try
            {
     
            var equipRegions = await (from r in _context.Regions2.Where(a => a.isDeleted == 0)
                                      join s in _context.Sites.Where(a => (a.otValid == 1)) on r.regionId equals s.region2
                                      join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                      join aus in _context.AUSite.Where(a => a.userId == reg.userId || reg.userId==-1) on s.siteId equals aus.siteId
                                      join aut in _context.AUTechnology.Where(a => a.userId == reg.userId || reg.userId == -1) on stech.techId equals aut.technologyId
                                      select new
                                      {
                                          regionTitle = r.title,
                                          r.regionId,
                                      }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getCountries")]
        public async Task<IActionResult> GetCountries(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from c in _context.Country.Where(a=>a.isDeleted == 0)
                                          join s in _context.Sites on c.countryId equals s.countryId
                                          join stech in _context.SitesTechnology on s.siteId equals stech.siteId
                                          join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                          join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on stech.techId equals aut.technologyId
                                          select new
                                          {
                                              countryTitle = c.title,
                                              c.countryId,
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getAllCountries")]
        public async Task<IActionResult> GetAllCountries(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from c in _context.Country.Where(a => a.isDeleted == 0)
                                         
                                          select new
                                          {
                                              countryTitle = c.title,
                                              c.countryId,
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getTechnologiesOM")]
        public async Task<IActionResult> getTechnologiesOM(CommonFilterDto reg)
        {
            try
            {
                var equipRegions = await (from s in _context.Technology.Where(a => a.isDeleted == 0)
                                          join st in _context.SitesTechnology.Where(a=>a.isDeleted == 0 &&(a.siteId == reg.siteId || reg.siteId == -1)) on s.techId equals st.techId
                                          select new
                                          {
                                              technologyTitle = s.name,
                                              technologyId = s.techId
                                          }).Distinct().ToListAsync();
                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getTechnologies")]
        public async Task<IActionResult> getTechnologies(CommonFilterDto reg)
        {
            try
            {
                var equipRegions = await (from s in _context.AUTechnology.Where(a => a.isDeleted == 0 && a.userId == reg.userId)
                                          join at in _context.Technology.Where(a => a.isDeleted == 0) on s.technologyId equals at.techId
                                          select new
                                          {
                                              technologyTitle = at.name,
                                              technologyId = at.techId
                                          }).Distinct().ToListAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("getFleetEquipment")]
        public async Task<IActionResult> getFleetEquipment(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from s in _context.ModelEquipment.Where(a => a.isDeleted == 0 && a.fleetEquipId == reg.fleetEquipmentId)
                                          join sp in _context.ModelEquipmentType.Where(a=>a.isDeleted == 0) on s.equipmentTypeId equals sp.typeId
                                          join se in _context.ModelEquipmentOEM.Where(a=>a.isDeleted == 0) on s.oemId equals se.oemId
                                          select new
                                          {
                                              fleetEquipmentId = s.fleetEquipId,
                                              fleetEquipmentTtile = s.title,
                                              fleetEquipmentType = sp.typeTitle,
                                              fleetEquipmentOEM = se.oemTitle
                                              ,
                                          }).Distinct().FirstOrDefaultAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("getEquipments")]
        public async Task<IActionResult> getEquipment(CommonFilterDto reg)
        {
            try
            {

                var equipRegions = await (from s in _context.ModelEquipment.Where(a => a.isDeleted == 0 && a.fleetEquipId == reg.fleetEquipmentId)
                                          join sp in _context.ModelEquipmentType.Where(a => a.isDeleted == 0) on s.equipmentTypeId equals sp.typeId
                                          join se in _context.ModelEquipmentOEM.Where(a => a.isDeleted == 0) on s.oemId equals se.oemId
                                          select new
                                          {
                                              fleetEquipmentId = s.fleetEquipId,
                                              fleetEquipmentTtile = s.title,
                                              fleetEquipmentType = sp.typeTitle,
                                              fleetEquipmentOEM = se.oemTitle
                                              ,
                                          }).Distinct().FirstOrDefaultAsync();


                return Ok(equipRegions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
