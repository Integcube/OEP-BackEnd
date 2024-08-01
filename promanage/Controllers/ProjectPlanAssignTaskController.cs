using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPlanAssignTaskController : ControllerBase
    {
        private readonly DAL _context;
        public ProjectPlanAssignTaskController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("getAssignTaskList")]
        public async Task<IActionResult> GetAssignTaskList(int UserId)
        {
            try
            {
                var Phase = await (from a in _context.ProjectStartPlan.Where( a=>a.isDeleted == 0 && a.pmId== UserId)
                                   join pl in _context.ProjectPlan.Where(a => a.isDeleted == 0) on a.planId equals pl.planId
                                   join plnt in _context.Sites on a.siteId equals plnt.siteId
                                   join user in _context.AppUser on a.createdBy equals user.userId
                                   join pgm in _context.AppUser on a.pmId equals pgm.userId
                                   select new
                                   {
                                       a.id,
                                       a.planId,
                                       a.siteId,
                                       a.startDate,
                                       a.endDate,
                                       a.createdOn,
                                       a.createdBy,
                                      site= plnt.siteName,
                                      plan=pl.title,
                                      user=user.userName,
                                      assignTo= pgm.userName,
                                      
                                   }).ToListAsync();


                return Ok(Phase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // getting Assign task for pgm on Final Window 
        [Authorize]
        [HttpGet("getAssignTasks")]
        public async Task<IActionResult> GetAssignTasks(int planId,int startPlanId)
        {
            try
            {
                var planInfo = await (from pl in _context.ProjectPlan.Where(aa => aa.isDeleted == 0)
                                      join a in _context.ProjectStartPlan.Where(a=>a.id== startPlanId) on pl.planId equals a.planId
                                      join plnt in _context.Sites on a.siteId equals plnt.siteId
                                      join cl in _context.Cluster on plnt.clusterId equals cl.clusterId
                                      join reg in _context.Regions2 on plnt.region2 equals reg.regionId
                                      select new
                                      {
                                          a.id,
                                          a.planId,
                                          a.siteId,
                                          a.startDate,
                                          a.endDate,
                                          a.createdOn,
                                          a.createdBy,
                                          site = plnt.siteName,
                                          plan = pl.title,
                                          region = reg.title,
                                          cluster = cl.clusterTitle
                                      }).FirstOrDefaultAsync();

                List<AssignTaskDTO> results = new List<AssignTaskDTO>();
                results = await _context.AssignTaskDTO.FromSqlRaw("EXEC dbo.[GetProjectPlan_Assign_Tasks] @PlanId,@StartPlanId", new SqlParameter("@PlanId", planInfo.planId), new SqlParameter("@StartPlanId", startPlanId) )
                    .ToListAsync();
                var responseData = new
                {
                    planInfo,
                    results
                };
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("saveAssignTask")]
        public async Task<IActionResult> SaveAssignTask(ProjectPlanDto projectPlanDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {



                // Insert tasks
                foreach (var task in projectPlanDto.Tasks)
                {
                    if (task.IsChild == 0)
                    {
                        await InsertTaskAsync(task, projectPlanDto.Project.PlanId);
                    }
                }

                await transaction.CommitAsync();
                return Ok(0);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }
  
        private async Task InsertTaskAsync(TaskDTO taskDto, int planId)
        {
            try
            {
                var existingTask = await _context.ProjectTaskAssignment
                                             .Where(t => t.TaskAssignmentId == taskDto.TaskAssignmentId)
                                             .FirstOrDefaultAsync();

                if (existingTask != null)
                {
                    existingTask.AssignTo = taskDto.AssignTo ?? -1;
                    existingTask.ModifiedOn = DateTime.UtcNow;
                    existingTask.ModifiedBy = taskDto.AssignTo ?? 0;
                }

            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }


        // getting Excuted task on Final Window 
        [Authorize]
        [HttpPost("getExceTasks")]
        public async Task<IActionResult> GetExceTasks(MonthYearDTO dto)
        {
            try
            {
                var planInfo = await (from pl in _context.ProjectPlan.Where(aa => aa.isDeleted == 0)
                                      join a in _context.ProjectStartPlan.Where(a => a.id == dto.startPlanId) on pl.planId equals a.planId
                                      join plnt in _context.Sites on a.siteId equals plnt.siteId
                                      join cl in _context.Cluster on plnt.clusterId equals cl.clusterId
                                      join reg in _context.Regions2 on plnt.region2 equals reg.regionId
                                      select new
                                      {
                                          a.id,
                                          a.planId,
                                          a.siteId,
                                          a.startDate,
                                          a.endDate,
                                          a.createdOn,
                                          a.createdBy,
                                          site = plnt.siteName,
                                          plan = pl.title,
                                          region = reg.title,
                                          cluster = cl.clusterTitle
                                      }).FirstOrDefaultAsync();

                var monthsParam = string.Join(",", dto.Months.Select(m => $"{FormatDate(m)}"));
               
                List<ProjectExeTask> results = new List<ProjectExeTask>();
                results = await _context.ProjectExeTask.FromSqlRaw("EXEC dbo.[GetProjectPlan_Assign_Tasks_alsoExcTask] @PlanId,@StartPlanId,@Months", new SqlParameter("@PlanId", planInfo.planId), new SqlParameter("@StartPlanId", dto.startPlanId), new SqlParameter("@Months", monthsParam))
                    .ToListAsync();
                var responseData = new
                {
                    results,
                    planInfo
                };
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static string FormatDate(string dateStr)
        {
            DateTime date = DateTime.ParseExact(dateStr, "MMM-yyyy", CultureInfo.InvariantCulture);
            return date.ToString("dd-MMM-yyyy");
        }


        [Authorize]
        [HttpPost("saveProgress")]
        public async Task<IActionResult> SaveProgress(SaveProgress progress)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int startplanId = 0;
              

                foreach (var task in progress.Tasks)
                {
                    if (task.IsChild == 0)
                    {
                        await InsertTaskAsync(task, progress.createdBy);
                    }
                }

                await transaction.CommitAsync();
                return Ok(0);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        private async Task InsertTaskAsync(ProjectExeTask taskDto, int createdBy)
        {
            try
            {
                var existingTask = await _context.ProjectTaskAssignment
                                             .Where(t => t.TaskAssignmentId == taskDto.TaskAssignmentId)
                                             .FirstOrDefaultAsync();

                if (existingTask != null)
                {
                    existingTask.Progress = taskDto.Progress;
                    
                    existingTask.ModifiedOn = DateTime.UtcNow;
                    existingTask.ModifiedBy = createdBy;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
