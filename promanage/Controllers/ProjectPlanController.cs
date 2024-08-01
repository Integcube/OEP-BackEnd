using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPlanController : ControllerBase
    {
        private readonly DAL _context;
        public ProjectPlanController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("getPlans")]
        public async Task<IActionResult> GetPlans(int UserId)
        {
            try
            {

                var plan = await (from a in _context.ProjectStartPlan.Where(a => a.isDeleted == 0)
                                   join pl in _context.ProjectPlan.Where(aas=> aas.isDeleted==0) on a.planId equals pl.planId
                                   join plnt in _context.Sites on a.siteId equals plnt.siteId
                                   join ausit in _context.AUSite.Where(a => a.userId == UserId) on plnt.siteId equals ausit.siteId
                                   join user in _context.AppUser on a.createdBy equals user.userId
                                   join pgm in _context.AppUser on a.pmId equals pgm.userId
                                   select new
                                   {

                                       startplanId=a.id,
                                       a.planId,
                                       a.siteId,
                                       a.startDate,
                                       a.createdOn,
                                       a.createdBy,
                                       site = plnt.siteName,
                                       plan = pl.title,
                                       user = user.userName,
                                       assignTo = pgm.userName,
                                       a.pmId,
                                       pl.description,
                                       pl.title
                                   }).ToListAsync();


                return Ok(plan);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Authorize]
        [HttpPost("savePlan")]
        public async Task<IActionResult> SavePlan(ProjectPlandDto reg)
        {
            try
            {
                if (reg.planId == -1)
                {
                    ProjectPlan region = new ProjectPlan();
                    region.isDeleted = 0;
                    region.title = reg.title;
                    region.description = reg.description;
                    region.createdBy = reg.createdBy;
                    region.createdOn = reg.createdOn;
                    region.createdOn = DateTime.Now;
                    _context.Add(region);
                    _context.SaveChanges();

                    ProjectStartPlan plan = new ProjectStartPlan();
                    plan.isDeleted = 0;
                    plan.startDate = reg.startDate;
                    plan.planId = region.planId;
                    plan.siteId = reg.siteId;
                    plan.createdBy = reg.createdBy;
                    plan.pmId = reg.pmId;
                    plan.createdOn = DateTime.Now;
                    _context.Add(plan);
                    _context.SaveChanges();


                    
               }
                else
                {
                    ProjectPlan region = await (from r in _context.ProjectPlan.Where(a => a.planId == reg.planId)
                                                select r).FirstOrDefaultAsync();
                    region.title = reg.title;
                    region.description = reg.description;

                ProjectStartPlan planInfo = await (from r in _context.ProjectStartPlan.Where(a => a.id == reg.startplanId)
                                                   select r).FirstOrDefaultAsync();
                planInfo.startDate = reg.startDate;
                planInfo.planId = reg.planId;
                planInfo.siteId = reg.siteId;
                planInfo.pmId = reg.pmId;
                _context.SaveChanges();

            }
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("deletePlan")]
        public async Task<IActionResult> DeletePlan(int planId)
        {
            try
            {
                ProjectPlan plan = await (from r in _context.ProjectPlan.Where(a => a.planId == planId)
                                          select r).FirstOrDefaultAsync();
                plan.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GETING TASK for configuarion window
        [Authorize]
        [HttpGet("getPlanTaskBy")]
        public async Task<IActionResult> GetPlanTaskBy(int planId)
        {
            try
            {
                var planInfo = await (from pl in _context.ProjectPlan.Where(aa => aa.isDeleted == 0 && aa.planId == planId)
                                      join a in _context.ProjectStartPlan on pl.planId equals a.planId
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
                results = await _context.AssignTaskDTO.FromSqlRaw("EXEC dbo.[GetProjectPlan_Assign_Tasks] @PlanId,@StartPlanId", new SqlParameter("@PlanId", planId), new SqlParameter("@StartPlanId", planInfo.id))
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
        [HttpPost("savePhaseTask")]
        public async Task<IActionResult> SavePhaseTask(ProjectPlanPhaseDto reg)
        {
            try
            {
                if (reg.phaseId == -1)
                {
                    ProjectPhase task = new ProjectPhase();
                    task.isDeleted = 0;
                    task.title = reg.title;
                    task.planId = reg.planId;
                    task.weightage = Convert.ToDecimal(reg.weightage);
                    task.duration = Convert.ToInt32(reg.duration); ;
                    task.createdBy = reg.createdBy;
                    task.createdOn = DateTime.Now;
                    task.displayOrder = reg.displayOrder;
                    _context.Add(task);
                    _context.SaveChanges();
                    await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));
                }
                else
                {
                    ProjectPhase task = await (from r in _context.ProjectPhase.Where(a => a.phaseId == reg.phaseId)
                                                  select r).FirstOrDefaultAsync();

                    task.title = reg.title;
                    task.weightage = Convert.ToDecimal(reg.weightage);
                    task.duration = Convert.ToInt32(reg.duration);
                    task.modifiedBy = reg.createdBy;
                    task.modifiedOn = DateTime.Now;
                    task.displayOrder = reg.displayOrder;
                    _context.SaveChanges();
                    await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));

                }

                
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpPost("savemaintask")]
        public async Task<IActionResult> SaveMainTask(ProjectPlanTaskDto reg)
        {
            try
            {
                if (reg.taskId == -1)
                {
                    ProjectPlanTask task = new ProjectPlanTask();
                    task.isDeleted = 0;
                    task.title = reg.title;
                    task.phaseId = reg.phaseId;
                    task.taskParentId = -1;
                    task.planId= reg.planId;
                   
                    task.duration= reg.duration;
                    task.createdBy = reg.createdBy;
                    task.createdOn = DateTime.Now;
                    task.predecessorId = reg.predecessorId;
                    task.lagDays = reg.lagDays;
                    task.predecessorType = reg.predecessorType;
                    task.durationUnit = reg.durationUnit;
                    task.lagUnit = reg.lagUnit;

                    _context.Add(task);
                    _context.SaveChanges();

                   await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId",new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));
                }
                else
                {
                    ProjectPlanTask task = await (from r in _context.ProjectPlanTask.Where(a => a.taskId == reg.taskId)
                                                select r).FirstOrDefaultAsync();

                    task.title = reg.title;
                    task.phaseId = reg.phaseId;
                    //task.weightage = Convert.ToDecimal(reg.weightage);
                    task.duration = reg.duration;
                    task.modifiedBy = reg.createdBy;
                    task.modifiedOn = DateTime.Now;
                    task.predecessorId = reg.predecessorId;
                    task.lagDays = reg.lagDays;
                    task.predecessorType = reg.predecessorType;
                    task.durationUnit = reg.durationUnit;
                    task.lagUnit = reg.lagUnit;
                    _context.SaveChanges();

                }
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("savesubtask")]
        public async Task<IActionResult> SaveSubtask(ProjectPlanSubTaskDto reg)
        {
            try
            {
                if (reg.taskId == -1)
                {
                    ProjectPlanTask task = new ProjectPlanTask();
                    task.isDeleted = 0;
                    task.title = reg.title;
                    task.phaseId = reg.phaseId;
                    task.taskParentId = reg.mainTaskId;
                    task.planId = reg.planId;
                    task.duration = reg.duration;
                    task.predecessorId = reg.predecessorId;
                    task.lagDays = reg.lagDays;
                    task.weightage = 0;
                    task.createdBy = reg.createdBy;
                    task.createdOn = DateTime.Now;            
                    task.predecessorType = reg.predecessorType;
                    task.durationUnit = reg.durationUnit;
                    task.lagUnit = reg.lagUnit;
                    _context.Add(task);
                    _context.SaveChanges();

                    await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", task.planId), new SqlParameter("@PhaseId", task.phaseId));
                }
                else
                {
                    ProjectPlanTask task = await (from r in _context.ProjectPlanTask.Where(a => a.taskId == reg.taskId)
                                                  select r).FirstOrDefaultAsync();

                    task.title = reg.title;
                    task.phaseId = reg.phaseId;
                    task.duration =reg.duration;
                    task.modifiedBy = reg.createdBy;
                    task.modifiedOn = DateTime.Now;
                    task.predecessorId = reg.predecessorId;
                    task.lagDays = reg.lagDays;
                    task.predecessorType = reg.predecessorType;
                    task.durationUnit = reg.durationUnit;
                    task.lagUnit = reg.lagUnit;
                    _context.SaveChanges(); 

                }
                return Ok(reg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("deletetask")]
        public async Task<IActionResult> Deletetask(int taskId,int userId)
        {
            try
            {
                ProjectPlanTask plan = await (from r in _context.ProjectPlanTask.Where(a => a.taskId == taskId)
                                          select r).FirstOrDefaultAsync();
                plan.isDeleted = 1;
                plan.modifiedBy = userId;
                plan.modifiedOn= DateTime.Now;


                var task = await _context.ProjectTaskAssignment.Where(t => t.TaskId == taskId).FirstOrDefaultAsync();

                if (task != null)
                {
                    _context.ProjectTaskAssignment.Remove(task);
                }
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePhaseWeightageAndGenerateTaskCodes @PlanId, @PhaseId", new SqlParameter("@PlanId", plan.planId), new SqlParameter("@PhaseId", plan.phaseId));

               
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpDelete("deletephase")]
        public async Task<IActionResult> Deletephase(int phaseId, int userId)
        {
            try
            {
                ProjectPhase plan = await (from r in _context.ProjectPhase.Where(a => a.phaseId == phaseId)
                                              select r).FirstOrDefaultAsync();
                plan.isDeleted = 1;
                plan.modifiedBy = userId;
                plan.modifiedOn = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpGet("getstartplans")]
        public async Task<IActionResult> GetStartPlans()
        {
            try
            {
                var Phase = await (from a in _context.ProjectStartPlan.Where( a=>a.isDeleted == 0)
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

        [Authorize]
        [HttpGet("getsites")]
        public async Task<IActionResult> Getsites(int UserId)
        {
            try
            {
                var Phase = await (from a in _context.AUSite.Where(a => a.userId == UserId)
                                   join aus in _context.Sites.Where(a => a.isDeleted == 0) on a.siteId equals aus.siteId
                                   select new
                                   {

                                       a.siteId,
                                       siteTitle = aus.siteName

                                   }).OrderBy(a=>a.siteTitle).ToListAsync();


                return Ok(Phase);
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
                int startplanId = projectPlanDto.Project.Id;

                var uniquePhases = projectPlanDto.Tasks.GroupBy(t => t.PhaseId).Select(g => g.First()).ToList();

                foreach (var phase in uniquePhases)
                {
                    await InsertPhaseAsync(phase.PhaseId, phase.PhaseStartDate, phase.PhaseEndDate, phase.PhasePgmId, projectPlanDto.Project.PlanId, startplanId, phase.startPhaseId);
                }


                // Insert tasks
                foreach (var task in projectPlanDto.Tasks)
                {
                    if (task.IsChild == 0)
                    {
                        await InsertTaskAsync(task, projectPlanDto.Project.PlanId, startplanId);
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

        private async Task InsertPhaseAsync(int phaseId, DateTime? phaseStartDate, DateTime? phaseEndDate, int phasePgmId, int planId,int startplanId,int startPhaseId)
        {
            try
            {
                var existingPhase = await _context.ProjectStartPhase.FirstOrDefaultAsync(p => p.StartPhaseId == startPhaseId);

                if (existingPhase != null)
                {

                    existingPhase.StartDate = phaseStartDate;
                    existingPhase.EndDate = phaseEndDate;
                    existingPhase.PgmId = phasePgmId;
                }
                else
                {
                    var phase = new ProjectStartPhase
                    {
                        StartDate = phaseStartDate,
                        EndDate = phaseEndDate,
                        PhaseId = phaseId,
                        PlanId = planId,
                        PgmId = phasePgmId,
                        startplanId = startplanId
                    };
                
                    _context.ProjectStartPhase.Add(phase);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
              
               
            }
        }
        private async Task InsertTaskAsync(TaskDTO taskDto, int planId,int StartplanId)
        {
            try
            {
                var existingTask = await _context.ProjectTaskAssignment
                                             .Where(t => t.TaskAssignmentId == taskDto.TaskAssignmentId)
                                             .FirstOrDefaultAsync();

                if (existingTask != null)
                {
                    
                    existingTask.Weightage = taskDto.Weightage;
                    existingTask.Duration = taskDto.Duration;
                    existingTask.StartDate = taskDto.TaskAssignmentStartDate;
                    existingTask.EndDate = taskDto.TaskAssignmentEndDate;
                    existingTask.ModifiedOn = DateTime.UtcNow;
                    existingTask.ModifiedBy = taskDto.AssignTo ?? 0;
                }
                else
                {
                    var taskAssignment = new ProjectTaskAssignment
                    {
                        TaskId = taskDto.TaskId,
                        StartplanId = StartplanId,
                        AssignTo = taskDto.AssignTo ?? 0,
                        Weightage = taskDto.Weightage,
                        Duration = taskDto.Duration,
                        StartDate = taskDto.TaskAssignmentStartDate,
                        EndDate = taskDto.TaskAssignmentEndDate,
                        Progress = 0,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = taskDto.AssignTo ?? 0,
                        IsDeleted = 0
                    };

                    _context.ProjectTaskAssignment.Add(taskAssignment);
                }


            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }



        [Authorize]
        [HttpGet("getExecutedPlan")]
        public async Task<IActionResult> GetExecutedPlan(int userId)

        {
            try
            {
                var Phase = await  (from pl in _context.ProjectStartPlan
                             join cpln in _context.ProjectPlan.Where(a => a.isDeleted == 0) on pl.planId equals cpln.planId
                             join plnt in _context.Sites on pl.siteId equals plnt.siteId
                             join ausit in _context.AUSite.Where(a => a.userId == userId) on plnt.siteId equals ausit.siteId
                             join user in _context.AppUser on pl.createdBy equals user.userId
                             select new
                             {
                                 pl.id,
                                 pl.planId,
                                 site = plnt.siteName,
                                 plan = cpln.title,
                                 user = user.userName,
                                 pl.createdOn,
                                 pl.createdBy,
                                 pl.startDate,
                                 IsStarted = pl.startDate <= DateTime.UtcNow ? 1 : 0
                             }).ToListAsync();

                return Ok(Phase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpGet("getAssignTasktoUser")]
        public async Task<IActionResult> GetAssignTasktoUser(int UserId, int startPlanId)
        {
            try
            {

               

                List<AssignTaskDTO> results = new List<AssignTaskDTO>();
                results = await _context.AssignTaskDTO.FromSqlRaw("EXEC dbo.[GetProjectPlan_Assign_Tasks] @PlanId,@StartPlanId", new SqlParameter("@PlanId", -1), new SqlParameter("@StartPlanId", startPlanId))
                    .ToListAsync();
                var responseData = new
                {
                   
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
        [HttpGet("getpredecessorTask")]
        public async Task<IActionResult> GetpredecessorTask(int planId)

        {
            try
            {
                var Phase = await (from a in _context.ProjectPlanTask.Where(a => a.isDeleted == 0 && a.planId == planId)
                                   join phase in _context.ProjectPhase.Where(aa => aa.isDeleted == 0) on a.phaseId equals phase.phaseId
                                   select new
                                   {

                                       a.taskId,
                                       title=a.code+ " " +a.title,
                                       phase.displayOrder

                                   }).OrderBy(aa => aa.displayOrder).ToListAsync();



                return Ok(Phase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getpredecessor")]
        public async Task<IActionResult> Getpredecessor()
        {
            try
            {
                var pre = await (from a in _context.Project_PredecessorType.Where(a => a.isDeleted == 0)

                                   select new
                                   {

                                       a.id,
                                       a.title

                                   }).ToListAsync();


                return Ok(pre);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("makecopyplan")]
        public async Task<IActionResult> MakecopyPlan(int planId)
        {
            try
            {
                    await _context.Database.ExecuteSqlRawAsync("EXEC dbo.CopyProjectPlan @PlanId", new SqlParameter("@PlanId", planId));

                return Ok(1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
