using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;

namespace ActionTrakingSystem.Controllers
{
    public class MitigationActionPlanProgramsController : BaseAPIController
    {
        private readonly DAL _context;
        public MitigationActionPlanProgramsController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getTechnologies")]
        public async Task<IActionResult> GetTechnology(programUserDto reg)
        {
            try
            {
                var technologies = await (from a in _context.OMA_ProgramTechnologies.Where(a => a.isDeleted == 0 && (reg.programId == -1 || a.programId == reg.programId))
                                          join t in _context.Technology on a.technologyId equals t.techId
                                          select new
                                          {
                                              technologyId = a.technologyId,
                                              technologyTitle = t.name,
                                          }

                                         ).ToListAsync();

                return Ok(technologies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("allTechnologies")]
        public async Task<IActionResult> AllTechnology(programUserDto reg)
        {
            try
            {
                var technologies = await (from t in _context.Technology.Where(a=>a.isDeleted == 0)
                                          select new
                                          {
                                              technologyId = t.techId,
                                              technologyTitle = t.name,
                                          }

                                         ).ToListAsync();

                return Ok(technologies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteProgram")]
        public async Task<IActionResult> deleteProgram(programDto reg)
        {
            try
            {
               
                    OMA_IProgram program = await (from a in _context.OMA_IProgram.Where(a => a.programId == reg.programId)
                                                  select a).FirstOrDefaultAsync();
                    program.isDeleted = 1;
                   
                    _context.SaveChanges();
                SaveProgramsRow(_context, reg.programId, 3);

                return Ok(reg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveTechnology")]
        public async Task<IActionResult> saveTechnology(saveProgramDto reg)
        {
            try
            {
                if(reg.program.program.programId == -1)
                {
                    OMA_IProgram program = new OMA_IProgram();
                    program.programTitle = reg.program.program.programTitle;
                    program.createdBy = reg.userId;
                    program.createdOn = DateTime.Now;
                    _context.Add(program);
                    _context.SaveChanges();
                    reg.program.program.programId = program.programId;
                    for (var i = 0; i < reg.program.technologiesSubmit.Length; i++)
                    {
                        OMA_ProgramTechnologies eq = new OMA_ProgramTechnologies();
                        eq.technologyId = reg.program.technologiesSubmit[i].technologyId;
                        eq.programId = program.programId;
                        eq.isDeleted = 0;
                        _context.Add(eq);
                    }
                    _context.SaveChanges();
                    SaveProgramsRow(_context, program.programId, 1);
                }
                else
                {
                    OMA_IProgram program = await(from a in _context.OMA_IProgram.Where(a=>a.programId == reg.program.program.programId)
                                                 select a).FirstOrDefaultAsync();
                    program.programTitle = reg.program.program.programTitle;
                    program.modifiedBy = reg.userId;
                    program.modifiedOn = DateTime.Now;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM OMA_ProgramTechnologies WHERE programId = @tapID", new SqlParameter("tapID", program.programId));
                    for (var i = 0; i < reg.program.technologiesSubmit.Length; i++)
                    {
                        OMA_ProgramTechnologies eq = new OMA_ProgramTechnologies();
                        eq.technologyId = reg.program.technologiesSubmit[i].technologyId;
                        eq.programId = program.programId;
                        eq.isDeleted = 0;
                        _context.Add(eq);
                    }
                    _context.SaveChanges();
                    SaveProgramsRow(_context, program.programId, 2);

                }

                return Ok(reg.program.program);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static void SaveProgramsRow(DAL dal, int programId, int typeId)
        {

            dal.Database.ExecuteSqlRaw("EXEC udpOMA_SiteControl @ProgramId, @TypeId",
        new SqlParameter("@ProgramId", programId),
        new SqlParameter("@TypeId", typeId));
          
        }
        [Authorize]
        [HttpPost("getProgram")]
        public async Task<IActionResult> GetProgram(UserIdDto reg)
        {
            try
            {
                var program = await (from a in _context.OMA_IProgram.Where(a => a.isDeleted == 0)
                                     //join at in _context.OMA_ProgramTechnologies on a.programId equals at.programId into all1
                                     //from att in all1.DefaultIfEmpty()
                                     //join tech in _context.Technology on att.technologyId equals tech.techId into all2
                                     //from tect in all2.DefaultIfEmpty()
                                     select new
                                     {
                                         a.programId,
                                         a.programTitle,
                                         //att.technologyId,
                                         //technologyTitle = tect.name,

                                     }).ToListAsync();

                return Ok(program);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
