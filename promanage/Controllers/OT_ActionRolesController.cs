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

    public class OT_ActionRolesController : BaseAPIController
    {
        private readonly DAL _context;
        public OT_ActionRolesController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getActionRoles")]
        public async Task<IActionResult> GetActionRoles(UserIdDto reg)
        {
            try
            {


                var users = await (from a in _context.OT_IActionOwner.Where(a => a.isDeleted == 0)
                                   join u in _context.OT_IActionOwnerUser on a.actionOwnerId equals u.actionOwnerId into all1
                                   from uu in all1.DefaultIfEmpty()
                                   join au in _context.AppUser on uu.userId equals au.userId into all2
                                   from auu in all2.DefaultIfEmpty()
                                   select new
                                   {
                                       a.actionOwnerId,
                                       a.actionOwnerTitle,

                                       auu.userName
                                   }).OrderByDescending(a => a.actionOwnerId).ToListAsync();

                var concatenatedUserNamesByActionOwnerId = users
    .GroupBy(u => u.actionOwnerId)
    .Select(group => new
    {
        ActionOwnerId = group.Key,
        actionOwnerTitle= group.FirstOrDefault().actionOwnerTitle,
        userName = string.Join(", ", group.Select(u => u.userName))
    }).ToList();


           








                return Ok(concatenatedUserNamesByActionOwnerId);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize]
        [HttpPost("deleteActionRoles")]
        public async Task<IActionResult> DeleteActionRoles(OT_ActionRoleUserDto reg)
        {
            try
            {
                var actionRole = await (from a in _context.OT_IActionOwner.Where(a => a.actionOwnerId == reg.actionRole.actionOwnerId)
                                        select a).FirstOrDefaultAsync();
                actionRole.isDeleted = 1;
                _context.SaveChanges();
                return Ok(reg.actionRole);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize]
        [HttpPost("getSelectedUsers")]
        public async Task<IActionResult> GetSelectedUsers(OT_GetSelectedUsers reg)
        {
            try
            {
                var actionRole = await (from a in _context.OT_IActionOwnerUser.Where(a => a.actionOwnerId == reg.actionRoleId)
                                        join ap in _context.AppUser on a.userId equals ap.userId
                                        select new
                                        {
                                            a.userId,
                                            ap.userName,
                                            ap.email,
                                        }).ToListAsync();
                return Ok(actionRole);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize]
        [HttpPost("saveActionRoles")]
        public async Task<IActionResult> SaveActionRoles(OT_SaveActionRoles reg)
        {
            try
            {
                if (reg.actionRole.roles.actionOwnerId == -1)
                {
                    OT_IActionOwner ao = new OT_IActionOwner();
                    ao.actionOwnerTitle = reg.actionRole.roles.actionOwnerTitle;
                    ao.createdBy = reg.userId;
                    ao.createdOn = DateTime.Now;
                    _context.Add(ao);
                    _context.SaveChanges();
                    for (var a = 0; a < reg.actionRole.users.Length; a++)
                    {
                        OT_IActionOwnerUser au = new OT_IActionOwnerUser();
                        au.actionOwnerId = ao.actionOwnerId;
                        au.userId = reg.actionRole.users[a].userId;
                        _context.Add(au);
                        _context.SaveChanges();
                    }
                    reg.actionRole.roles.actionOwnerId = ao.actionOwnerId;
                    return Ok(reg.actionRole.roles);
                }
                else
                {
                    var ao = await (from a in _context.OT_IActionOwner.Where(a => a.actionOwnerId == reg.actionRole.roles.actionOwnerId)
                                    select a).FirstOrDefaultAsync();
                    ao.actionOwnerTitle = reg.actionRole.roles.actionOwnerTitle;
                    ao.modifiedOn = DateTime.Now;
                    ao.modifiedBy = reg.userId;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM OT_IActionOwnerUser WHERE actionOwnerId = @siteId", new SqlParameter("siteId", ao.actionOwnerId));

                    for (var a = 0; a < reg.actionRole.users.Length; a++)
                    {
                        OT_IActionOwnerUser au = new OT_IActionOwnerUser();
                        au.actionOwnerId = ao.actionOwnerId;
                        au.userId = reg.actionRole.users[a].userId;
                        _context.Add(au);
                        _context.SaveChanges();
                    }
                    return Ok(reg.actionRole.roles);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
