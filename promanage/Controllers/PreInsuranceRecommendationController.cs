using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ActionTrakingSystem.Controllers.ProjectManpowerPlanController;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreInsuranceRecommendationController : ControllerBase
    {
        private readonly DAL _context;
        public PreInsuranceRecommendationController(DAL context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet("getPreInsuranceRecommendation")]
        public async Task<IActionResult> GetPreInsuranceRecommendation(int UserId)
        {
            try
            {

                var data = await (from aaa in _context.AUTechnology.Where(hh => hh.userId == UserId)
                                        join insu in _context.PreInsuranceRecommendations.Where(a=>a.IsDeleted==0) on aaa.technologyId equals insu.TechId
                                        join tech in _context.Technology.Where(a => a.isDeleted == 0) on aaa.technologyId equals tech.techId
                                        join app in _context.AppUser.Where(a => a.isDeleted == 0) on insu.CreatedBy equals app.userId
                                        select new
                                        {
                                            insu.PreInsuranceId,
                                            insu.Title,
                                            insu.CreatedOn,
                                            tech.name,
                                            createdBy = app.firstName+" "+ app.lastName,

                                        }).OrderBy(a => a.PreInsuranceId).Take(1).ToListAsync();


                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("savePreInsuranceRecommendation")]
        public async Task<IActionResult> SavePreInsuranceRecommendation(int preInsuranceId, string title, int techId, int createdBy)
        {
            try
            {
                if (preInsuranceId == -1)
                {
                    var newRecommendation = new PreInsuranceRecommendation
                    {
                        Title = title,
                        TechId = techId,
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.Now,
                        IsDeleted = 0
                    };

                    _context.PreInsuranceRecommendations.Add(newRecommendation);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    var existingRecommendation = await _context.PreInsuranceRecommendations
                                                              .FirstOrDefaultAsync(r => r.PreInsuranceId == preInsuranceId && r.IsDeleted == 0);
                    existingRecommendation.Title = title;
                    existingRecommendation.TechId = techId;
                    existingRecommendation.ModifiedBy = createdBy;
                    existingRecommendation.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpDelete("deletePreInsuranceRecommendation")]
        public async Task<IActionResult> DeletePreInsuranceRecommendation(int preInsuranceId)
        {
            try
            {
                var recommendation = await _context.PreInsuranceRecommendations
                                                   .FirstOrDefaultAsync(r => r.PreInsuranceId == preInsuranceId && r.IsDeleted == 0);

                if (recommendation == null)
                {
                    return NotFound("PreInsuranceRecommendation not found or already deleted.");
                }

                recommendation.IsDeleted = 1;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
