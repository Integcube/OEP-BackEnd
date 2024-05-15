using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActionTrakingSystem.Model;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppMenus1Controller : ControllerBase
    {
        private readonly DAL _context;

        public AppMenus1Controller(DAL context)
        {
            _context = context;
        }

        // GET: api/AppMenus1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppMenu>>> GetAppMenu()
        {
            return await _context.AppMenu.ToListAsync();
        }

        // GET: api/AppMenus1/5
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

        // PUT: api/AppMenus1/5
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

        // POST: api/AppMenus1
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AppMenu>> PostAppMenu(AppMenu appMenu)
        {
            _context.AppMenu.Add(appMenu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppMenu", new { id = appMenu.MenuId }, appMenu);
        }

        // DELETE: api/AppMenus1/5
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
