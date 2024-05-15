using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class UserRoleController : BaseAPIController
    {
        private readonly DAL _context;
        public UserRoleController(DAL context) 
        { 
            _context = context;
        }
        [Authorize]
        [HttpPost("getUserRoles")]
        public async Task<IActionResult> GetUserRole(UserIdDto reg)
        {
            var userRole = await (from ur in _context.AppRole.Where(a => a.isDeleted == 0)
                                  select new
                                  {
                                      roleName = ur.title,
                                      roleDescription = ur.description,
                                      roleId = ur.roleId
                                  }).OrderByDescending(a=>a.roleId).ToListAsync();
            var menus = await (from ur in _context.AppMenu.Where(a => a.isDeleted == 0 && a.Role == "level3")
                               select new { 
                               menuId = ur.MenuId,
                               title = ur.Title,
                               parentId = ur.ParentId,
                              selected = false
                               }).OrderBy(a=>a.menuId).ToListAsync();

            var data = new
            {
                userRole,
                menus
            };
            return Ok(data);
        }
        [Authorize]
        [HttpPost("getTrash")]
        public async Task<IActionResult> GetTrash(UserIdDto reg)
        {
            var userRole = await (from ur in _context.AppRole.Where(a => a.isDeleted == 1)
                                  select new
                                  {
                                      roleName = ur.title,
                                      roleDescription = ur.description,
                                      roleId = ur.roleId
                                  }).ToListAsync();
            return Ok(userRole);
        }
        [Authorize]
        [HttpPost("restoreTrash")]
        public async Task<IActionResult> Restore(UserRoleDto reg)
        {
            AppRole userRole = await (from ur in _context.AppRole.Where(a => a.roleId == reg.role.roleId)
                                  select ur).FirstOrDefaultAsync();
            userRole.isDeleted = 0;
            _context.SaveChanges();
            return Ok(reg);
        }
        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTrash(UserRoleDto reg)
        {
            AppRole userRole = await (from ur in _context.AppRole.Where(a => a.roleId == reg.role.roleId)
                                      select ur).FirstOrDefaultAsync();
            userRole.isDeleted = 1;
            _context.SaveChanges();
            return Ok(reg);
        }
        [Authorize]
        [HttpPost("editData")]
        public async Task<IActionResult> GetUpdate(UserRoleDto reg)
        {
            var menu = await (from m in _context.AppMenu.Where(a=> a.Role == "level3")
                              join rd in _context.AppRoleDetail.Where(a => a.roleId == reg.role.roleId) on m.MenuId equals rd.menuId into all
                              from aa in all.DefaultIfEmpty()
                              select new
                              {
                                  menuId = m.MenuId,
                                  title = m.Title,
                                  parentId = m.ParentId,
                                  selected = aa == null ? false : true
                              }).ToListAsync();
            return Ok(menu);
        }
        [Authorize]
        [HttpPost("saveUserRole")]
        public async Task<IActionResult> SaveRole(UserRoleSaveDto reg)
        {
            try
            {
                if (reg.data.userRole.roleId == -1)
                {
                    AppRole role = new AppRole();
                    role.isDeleted = 0;
                    role.description = reg.data.userRole.roleDescription;
                    role.title = reg.data.userRole.roleName;
                    _context.Add(role);
                    _context.SaveChanges();
                    if (reg.data.menus.Length > 0)
                    {

                        for (var i = 0; i < reg.data.menus.Length; i++)
                        {
                            if (reg.data.menus[i].selected == true)
                            {
                                AppRoleDetail menu = new AppRoleDetail();
                                menu.isDeleted = 0;
                                menu.roleId = role.roleId;
                                menu.menuId = reg.data.menus[i].menuId;
                                _context.Add(menu);
                                _context.SaveChanges();
                            }

                        }
                    }
                    reg.data.userRole.roleId = role.roleId;
                    return Ok(reg.data);
                }
                else
                {
                    AppRole role = await (from r in _context.AppRole.Where(a => a.roleId == reg.data.userRole.roleId)
                                          select r).FirstOrDefaultAsync();
                    role.title = reg.data.userRole.roleName;
                    role.description = reg.data.userRole.roleDescription;
                    _context.SaveChanges();

                    if (reg.data.menus.Length > 0)
                    {
                        _context.Database.ExecuteSqlCommand("DELETE FROM AppRoleDetail WHERE roleId = @tapID", new SqlParameter("tapID", reg.data.userRole.roleId));
                        for (var i = 0; i < reg.data.menus.Length; i++)
                        {
                            if (reg.data.menus[i].selected == true)
                            {
                                AppRoleDetail detail = new AppRoleDetail();
                                detail.isDeleted = 0;
                                detail.roleId = reg.data.userRole.roleId;
                                detail.menuId = reg.data.menus[i].menuId;
                                _context.Add(detail);
                                _context.SaveChanges();
                            }

                        }
                    }
                    return Ok(reg.data);
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
