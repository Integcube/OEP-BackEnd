using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using ActionTrakingSystem.DTOs;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppMenusController : BaseAPIController
    {
        private readonly DAL _context;

        public AppMenusController(DAL context)
        {
            _context = context;
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<AppMenu>>> GetappMenuss(UserIdDto reg)
        //{
        //    var menu = await (from a in _context.AppMenu.Where(a => a.isDeleted == 0)
        //                      select a).OrderBy(a => a.DisplayOrder).ToArrayAsync();
        //    return Ok(menu);
        //}
        [Authorize]
        [HttpPost("getMenu")]
        public async Task<ActionResult<IEnumerable<AppMenu>>> GetappMenus(UserIdDto reg)
        {
            var menu = await (from a in _context.AppUser.Where(a => a.userId == reg.userId)
                              join ar in _context.AURole on a.userId equals ar.userId
                              join appr in _context.AppRole on ar.roleId equals appr.roleId
                              join rd in _context.AppRoleDetail on appr.roleId equals rd.roleId
                              join am in _context.AppMenu.Where(a => a.isDeleted == 0) on rd.menuId equals am.MenuId
                              select am).Distinct().OrderBy(a => a.DisplayOrder).ToArrayAsync();
            return Ok(menu);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppMenu>> GetAppMenu(int id)
        {
            var appMenu = await _context.AppMenu.FindAsync(id);

            if (appMenu == null)
            {
                return NotFound();
            }

            return appMenu;
        }

        // PUT: api/AppMenus/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppMenu(int id, AppMenu appMenu)
        {
            if (id != appMenu.MenuId)
            {
                return BadRequest();
            }

            _context.Entry(appMenu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppMenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AppMenus
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AppMenu>> PostAppMenu(AppMenu appMenu)
        {
            _context.AppMenu.Add(appMenu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppMenu", new { id = appMenu.MenuId }, appMenu);
        }

        // DELETE: api/AppMenus/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AppMenu>> DeleteAppMenu(int id)
        {
            var appMenu = await _context.AppMenu.FindAsync(id);
            if (appMenu == null)
            {
                return NotFound();
            }

            _context.AppMenu.Remove(appMenu);
            await _context.SaveChangesAsync();

            return appMenu;
        }

        private bool AppMenuExists(int id)
        {
            return _context.AppMenu.Any(e => e.MenuId == id);
        }
    }
}
