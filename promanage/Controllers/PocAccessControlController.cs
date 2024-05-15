using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class PocAccessControlController : BaseAPIController
    {
        private readonly DAL _context;
        public PocAccessControlController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getPocInsuranceSites")]
        public async Task<IActionResult> GetPocInsuranceSites(UserIdDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && a.insurancePOCId == reg.userId)
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
                                      ).Distinct().ToListAsync();

                return Ok(sites);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getPocTILSites")]
        public async Task<IActionResult> GetPocTILSites(UserIdDto reg)
        {
            try
            {
                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0 && a.tilPocId == reg.userId)
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
                                     ).Distinct().ToListAsync();

                return Ok(sites);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getInsuranceUsers")]
        public async Task<IActionResult> GetInsuranceUsers(PocAccessControlDto reg)
        {
            try
            {
                var allUser = await (from s in _context.AppUser.Where(a => a.isDeleted == 0 && a.userId != reg.userId)
                                   join aus in _context.AUSite on s.userId equals aus.userId
                                   join a in _context.Sites.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId)) on aus.siteId equals a.siteId

                                   select new
                                   {
                                       s.userId,
                                       s.userName,
                                       name = s.firstName + " " + s.lastName,
                                       s.email,
                                   }
                                  ).Distinct().ToListAsync();
                var selectedUser = await (from a in _context.InsuranceAccessControl.Where(a=>a.siteId == reg.siteId && a.isPoc == 0)
                                          join s in _context.AppUser.Where(a=>a.isDeleted ==0) on a.userId equals s.userId
                                     select new
                                     {
                                         s.userId,
                                         s.userName,
                                         name = s.firstName + " " + s.lastName,
                                         s.email,
                                     }
                                  ).Distinct().ToListAsync();
                var obj = new
                {
                    allUser,
                    selectedUser,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getTILUsers")]
        public async Task<IActionResult> GetTILUsers(PocAccessControlDto reg)
        {
            try
            {
                var allUser = await (from s in _context.AppUser.Where(a => a.isDeleted == 0 && a.userId != reg.userId)
                                     join aus in _context.AUSite on s.userId equals aus.userId
                                     join a in _context.Sites.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId)) on aus.siteId equals a.siteId

                                     select new
                                     {
                                         s.userId,
                                         s.userName,
                                         name = s.firstName + " " + s.lastName,
                                         s.email,
                                     }
                                  ).Distinct().ToListAsync();
                var selectedUser = await (from a in _context.TILAccessControl.Where(a => a.siteId == reg.siteId && a.isPoc == 0)
                                          join s in _context.AppUser.Where(a => a.isDeleted == 0) on a.userId equals s.userId
                                          select new
                                          {
                                              s.userId,
                                              s.userName,
                                              name = s.firstName + " " + s.lastName,
                                              s.email,
                                          }
                                  ).Distinct().ToListAsync();
                var obj = new
                {
                    allUser,
                    selectedUser,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveInsuranceUsers")]
        public async Task<IActionResult> SaveInsuranceUsers(PocAccessControlSaveUserDto reg)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM InsuranceAccessControl WHERE siteId = @siteId AND isPoc = 0", new SqlParameter("siteId", reg.accessObj.siteId));
                if (reg.accessObj.userList.Length > 0)
                {
                    for (var i = 0; i < reg.accessObj.userList.Length; i++)
                    {
                        InsuranceAccessControl role = new InsuranceAccessControl();
                        role.isPoc = 0;
                        role.userId = reg.accessObj.userList[i].userId;
                        role.siteId = reg.accessObj.siteId;
                        _context.Add(role);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.accessObj.userList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveTILUsers")]
        public async Task<IActionResult> SaveTILUsers(PocAccessControlSaveUserDto reg)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TILAccessControl WHERE siteId = @siteId AND isPoc = 0", new SqlParameter("siteId", reg.accessObj.siteId));
                if (reg.accessObj.userList.Length > 0)
                {
                    for (var i = 0; i < reg.accessObj.userList.Length; i++)
                    {
                        TILAccessControl role = new TILAccessControl();
                        role.isPoc = 0;
                        role.userId = reg.accessObj.userList[i].userId;
                        role.siteId = reg.accessObj.siteId;
                        _context.Add(role);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.accessObj.userList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
