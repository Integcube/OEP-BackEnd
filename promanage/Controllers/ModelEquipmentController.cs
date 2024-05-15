using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelEquipmentController : ControllerBase
    {
        private readonly DAL _context;
        public ModelEquipmentController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterfaces(UserIdDto reg)
        {
            try
            {


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
        [HttpPost("getModelEquipment")]
        public async Task<IActionResult> GetSiteEquipment(UserDto reg)
        {
            try
            {
             
                var outages = await (from a in _context.ModelEquipment.Where(a => a.isDeleted == 0 )
                                     join e in _context.ModelEquipmentOEM on a.oemId equals e.oemId
                                     join s in _context.ModelEquipmentType on a.equipmentTypeId equals s.typeId
                                     select new
                                     {
                                        a.fleetEquipId,
                                        a.title,
                                        a.equipmentTypeId,
                                        a.oemId,
                                        e.oemTitle,
                                        s.typeTitle,
                                     }).OrderByDescending(a=>a.fleetEquipId).ToListAsync();

                return Ok(outages);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteModelEquipment")]
        public async Task<IActionResult> DeleteEquipment(SiteModelSaveDto reg)
        {
            try
            {
                ModelEquipment equip = await (from r in _context.ModelEquipment.Where(a => a.fleetEquipId == reg.fleetEquipId)
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
        [HttpPost("saveModelEquipment")]
        public async Task<IActionResult> SaveEquipment(SiteModelSaveDto reg)
        {
            try
            {
                if (reg.fleetEquipId == -1)
                {
                    ModelEquipment siteEq = new ModelEquipment();
                    siteEq.isDeleted = 0;
                    siteEq.createdDate = DateTime.Now;
                    siteEq.createdBy = reg.userId;
                    siteEq.oemId = reg.oemId;
                    siteEq.equipmentTypeId = reg.equipmentTypeId;
                    siteEq.title = reg.title;
                    _context.Add(siteEq);
                    _context.SaveChanges();
                    return Ok(0);
                }
                else
                {
                    ModelEquipment siteEq = await (from r in _context.ModelEquipment.Where(a => a.fleetEquipId == reg.fleetEquipId)
                                                  select r).FirstOrDefaultAsync();
                    siteEq.isDeleted = 0;
                    siteEq.modifiedDate = DateTime.Now;
                    siteEq.modifiedBy = reg.userId;
                    siteEq.oemId = reg.oemId;
                    siteEq.equipmentTypeId = reg.equipmentTypeId;
                    siteEq.title = reg.title;
                    _context.SaveChanges();
                    return Ok(0);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
