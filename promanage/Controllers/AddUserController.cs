using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class AddUserController : BaseAPIController
    {
        private readonly DAL _context;
        public AddUserController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getUser")]
        public async Task<IActionResult> GetUser(UserIdDto reg)
        
        {
            var users = await (from a in _context.AppUser.Where(a => a.isDeleted == 0)
                               select new
                               {
                                   userId = a.userId,
                                   userName = a.userName,
                                   email = a.email,
                                   password = a.password,
                                   phone = a.cell,
                                   firstName = a.firstName,
                                   lastName = a.lastName,
                               }).OrderBy(z => z.userId).ToListAsync();
            var roles = await (from a in _context.AppRole.Where(a => a.isDeleted == 0)
                               select new
                               {
                                   roleId = a.roleId,
                                   roleTitle = a.title,

                               }).ToListAsync();
            var sites = await (from a in _context.Sites.Where(a => a.isDeleted == 0)
                               select new
                               {
                                   siteId = a.siteId,
                                   siteName = a.siteName,
                                   regionId = a.regionId,
                                   selected = false

                               }).ToListAsync();
            var regions = await (from a in _context.Regions.Where(a => a.isDeleted == 0)
                               select new
                               {
                                   regionId = a.regionId,
                                   regionTitle = a.title,
                                   isSelected = false,

                               }).ToListAsync();
            var technologys = await (from a in _context.Technology.Where(a => a.isDeleted == 0)
                                     select new
                                     {
                                         techId = a.techId,
                                         techTitle = a.name,
                                         selected = false
                                     }).ToListAsync();

            var obj = new
            {
                users,
                roles,
                sites,
                technologys,
                regions,
            };
            return Ok(obj);
        }
        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTrash(AddUsersDto reg)
        {
            AppUser users = await (from ur in _context.AppUser.Where(a => a.userId == reg.userId)
                                   select ur).FirstOrDefaultAsync();
            users.isDeleted = 1;
            _context.SaveChanges();
            return Ok(reg);
        }
        [Authorize]
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreTrash(AddUsersDto reg)
        {
            AppUser users = await (from ur in _context.AppUser.Where(a => a.userId == reg.userId)
                                   select ur).FirstOrDefaultAsync();
            users.isDeleted = 0;
            _context.SaveChanges();
            return Ok(reg);
        }
        [Authorize]
        [HttpPost("getTrash")]
        public async Task<IActionResult> GetTrash(UserIdDto reg)
        {
            var user = await (from a in _context.AppUser.Where(a => a.isDeleted == 1)
                              select new
                              {
                                  userId = a.userId,
                                  userName = a.userName,
                                  email = a.email,
                                  password = a.password,
                                  phone = a.cell,
                                  firstName = a.firstName,
                                  lastName = a.lastName,
                              }).ToListAsync();
            return Ok(user);
        }
        [Authorize]
        [HttpPost("saveUser")]
        public async Task<IActionResult> SaveUser(saveUserDto reg)
        {
            try
            {
                if (reg.data.users.userId == -1)
                {
                    AppUser user = new AppUser();
                    user.userName = reg.data.users.userName;
                    user.password = reg.data.users.password;
                    user.cell = reg.data.users.phone;
                    user.email = reg.data.users.email;
                    user.isDeleted = 0;
                    user.role = "level3";
                    user.firstName = reg.data.users.firstName;
                    user.lastName = reg.data.users.lastName;
                    user.createdOn = DateTime.Now;
                    user.createdBy = reg.userId;
                    _context.Add(user);
                    _context.SaveChanges();

                    if (reg.data.roles.Length > 0)
                    {

                        for (var i = 0; i < reg.data.roles.Length; i++)
                        {
                            AURole role = new AURole();
                            role.isDeleted = 0;
                            role.userId = user.userId;
                            role.roleId = reg.data.roles[i].roleId;
                            _context.Add(role);
                            _context.SaveChanges();
                        }
                    }

                    if (reg.data.sites.Length > 0)
                    {

                        for (var i = 0; i < reg.data.sites.Length; i++)
                        {

                            if (reg.data.sites[i].selected == true)
                            {
                                AUSite site = new AUSite();
                                site.isDeleted = 0;
                                site.userId = user.userId;
                                site.siteId = reg.data.sites[i].siteId;
                                _context.Add(site);
                                _context.SaveChanges();
                            }
                        }
                    }

                    if (reg.data.technologys.Length > 0)
                    {

                        for (var i = 0; i < reg.data.technologys.Length; i++)
                        {

                            if (reg.data.technologys[i].selected == true)
                            {
                                AUTechnology tech = new AUTechnology();
                                tech.isDeleted = 0;
                                tech.userId = user.userId;
                                tech.technologyId = reg.data.technologys[i].techId;
                                _context.Add(tech);
                                _context.SaveChanges();
                            }

                        }
                    }

                    reg.data.users.userId = user.userId;
                    return Ok(reg.data.users);
                }
                else
                {
                    AppUser user = await (from u in _context.AppUser.Where(a => a.userId == reg.data.users.userId)
                                          select u).FirstOrDefaultAsync();
                    user.userName = reg.data.users.userName;
                    user.password = reg.data.users.password;
                    user.cell = reg.data.users.phone;
                    user.email = reg.data.users.email;
                    user.firstName = reg.data.users.firstName;
                    user.lastName = reg.data.users.lastName;
                    user.modifiedOn = DateTime.Now;
                    user.modifiedBy = reg.userId;
                    user.isDeleted = 0;

                    if (reg.data.roles.Length > 0)
                    {
                        _context.Database.ExecuteSqlCommand("DELETE FROM AURole WHERE userId = @tapID", new SqlParameter("tapID", reg.data.users.userId));
                        for (var i = 0; i < reg.data.roles.Length; i++)
                        {
                            AURole role = new AURole();
                            role.isDeleted = 0;
                            role.userId = user.userId;
                            role.roleId = reg.data.roles[i].roleId;
                            _context.Add(role);
                        }
                    }
                    if (reg.data.sites.Length > 0)
                    {
                        _context.Database.ExecuteSqlCommand("DELETE FROM AUSite WHERE userId = @tapID", new SqlParameter("tapID", reg.data.users.userId));
                        for (var i = 0; i < reg.data.sites.Length; i++)
                        {
                            if (reg.data.sites[i].selected == true)
                            {
                                AUSite site = new AUSite();

                                site.isDeleted = 0;
                                site.userId = user.userId;
                                site.siteId = reg.data.sites[i].siteId;
                                _context.Add(site);
                            }
                        }
                    }
                    if (reg.data.technologys.Length > 0)
                    {
                        _context.Database.ExecuteSqlCommand("DELETE FROM AUTechnology WHERE userId = @tapID", new SqlParameter("tapID", reg.data.users.userId));

                        for (var i = 0; i < reg.data.technologys.Length; i++)
                        {
                            if (reg.data.technologys[i].selected == true)
                            {
                                AUTechnology tech = new AUTechnology();
                                tech.isDeleted = 0;
                                tech.userId = user.userId;
                                tech.technologyId = reg.data.technologys[i].techId;
                                _context.Add(tech);
                            }

                        }
                    }
                    _context.SaveChanges();
                    return Ok(reg);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }
        [Authorize]
        [HttpPost("getUserInfo")]
        public async Task<IActionResult> GetInfo(UpdateUsersDto reg)
        {
            try
            {
                var roles = await (from a in _context.AURole.Where(a => a.userId == reg.info.userId)
                                   join b in _context.AppRole on a.roleId equals b.roleId
                                   select new
                                   {
                                       roleId = a.roleId,
                                       roleTitle = b.title,

                                   }).ToListAsync();
                var sites = await (from a in _context.Sites.Where(a=>a.isDeleted == 0)
                                   join b in _context.AUSite.Where(a => a.userId == reg.info.userId) on a.siteId equals b.siteId into all
                                   from c in all.DefaultIfEmpty()
                                   select new
                                   {

                                       siteId = a.siteId,
                                       siteName = a.siteName,
                                       regionId = a.regionId,
                                       selected = c == null ? false : true,

                                   }).ToListAsync();
                //var regions = await (from a in _context.Regions.Where(a => a.isDeleted == 0)
                //                     join s in _context.Sites.Where(a => a.isDeleted == 0) on a.regionId equals s.regionId
                //                   join b in _context.AUSite.Where(a => a.userId == reg.info.userId) on s.siteId equals b.siteId into all
                //                   from c in all.DefaultIfEmpty()
                //                   select new
                //                   {

                //                       regionId = a.regionId,
                //                       regionTitl = a.title,
                //                       isSelected = c == null ? false : true,

                //                   }).Distinct().ToListAsync();
                var technologys = await (from a in _context.Technology.Where(a => a.isDeleted == 0)
                                         join b in _context.AUTechnology.Where(a => a.userId == reg.info.userId) on a.techId equals b.technologyId into ab
                                         from x in ab.DefaultIfEmpty()
                                         select new
                                         {
                                             techId = a.techId,
                                             techTitle = a.name,
                                             selected = x == null ? false : true,
                                         }).ToListAsync();

                var obj = new
                {
                    roles,
                    sites,
                    technologys,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
