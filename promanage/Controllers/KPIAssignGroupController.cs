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

    public class KPIAssignGroupController : BaseAPIController
    {
        private readonly DAL _context;
        public KPIAssignGroupController(DAL context) 
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getAssignedGroup")]
        public async Task<IActionResult> GetAssignedGroup(KPIAssignGroupDto reg)
        {
            try
            {
                var selectedGroup = await (from a in _context.KPI_IndicatorGroup.Where(a => a.isDeleted == 0)
                                           join ug in _context.KPI_UserGroup.Where(a => a.userId == reg.assignedUserId) on a.groupId equals ug.groupId into all
                                           from ugg in all.DefaultIfEmpty()
                                           select new
                                           {
                                               a.groupId,
                                               a.groupTitle,
                                               a.groupCode,
                                               selected = ugg.groupId != null ? true : false
                                           }).ToListAsync();
                return Ok(selectedGroup);
            }
            catch(Exception e)
            {
               return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveAssignedGroup")]
        public async Task<IActionResult> saveAssignedGroup(SaveAssignGroupDto reg)
        {
            try
            {
                if (reg.data.Length > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM KPI_UserGroup WHERE userId = @tapID", new SqlParameter("tapID", reg.assignedUserId));
                    for (var i = 0; i < reg.data.Length; i++)
                    {
                        if (reg.data[i].selected == true)
                        {
                            KPI_UserGroup role = new KPI_UserGroup();
                            role.isDeleted = 0;
                            role.userId = reg.assignedUserId;
                            role.groupId = reg.data[i].groupId;
                            _context.Add(role);
                            _context.SaveChanges();
                        }
                    }
                }
                return Ok(reg.data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
