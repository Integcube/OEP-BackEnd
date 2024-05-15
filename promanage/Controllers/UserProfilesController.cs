using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class UserProfilesController : BaseAPIController
    {
        private DAL _context;
        public UserProfilesController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getUserProfile")]
        public async Task<IActionResult> GetUserProfile(UserIdDto reg)
        {
            try
            {
                var user = await (from a in _context.AppUser.Where(b => b.userId == reg.userId) 
                                  join emp in _context.Employee on a.userId equals emp.userId into uservar
                                  from alu in uservar.DefaultIfEmpty()
                                  select new
                                  {
                                      education = alu == null?"": alu.education,
                                      address = alu == null?"":alu.address,
                                      email =  a.email,
                                      middleName = a.middleName,
                                      cell = alu == null ? "" : alu.cell,
                                      mobile = alu == null ? "" : alu.mobile,
                                      gender = alu == null ? "" : alu.gender,
                                      about = alu == null ? "" : alu.about,
                                      empId = alu.empId,
                                      experience = alu.experience,
                                      workshops = alu.workshops == null ? "" : alu.workshops,
                                      DOB = alu.DOB,
                                      designation = alu == null ? "" : alu.designation,
                                      country = alu== null? "":alu.country,
                                      city = alu == null? "":alu.city
                                  }).FirstOrDefaultAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPost("securityForm")]
        public async Task<IActionResult> UpdateSecurityForm(SecurityForm reg)
        {
            var user = await(from au in _context.AppUser.Where(a=>a.userName == reg.userName && a.password == reg.password)
                             select au).FirstOrDefaultAsync();
            if (user == null)
            {
                return Ok(0);
            }
            else
            {
                user.password= reg.newPassword;
                await _context.SaveChangesAsync();
                return Ok(user);
            }
        }
        [Authorize]
        [HttpPost("experienceForm")]
        public async Task<IActionResult> UpdateExperienceForm(ExperienceForm reg)
        {
            try
            {
                Employee emp = await (from e in _context.Employee.Where(a => a.userId == reg.userId)
                                      select e).FirstOrDefaultAsync();
                emp.designation = reg.designation;
                emp.education = reg.education;
                emp.experience = reg.experience;
                emp.workshops = reg.workshops;
                await _context.SaveChangesAsync();
                return Ok(reg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("accountForm")]
        public async Task<IActionResult> UpdateAccountForm(AccountForm reg)
        {
            try
            {
                AppUser user = await (from au in _context.AppUser.Where(a => a.userId == reg.userId)
                                      select au).FirstOrDefaultAsync();
                user.firstName = reg.firstName;
                user.lastName = reg.lastName;
                user.middleName = reg.middleName;
                user.email = reg.email;
                Employee emp = await (from e in _context.Employee.Where(a => a.userId == reg.userId)
                                      select e).FirstOrDefaultAsync();
                emp.city = reg.city;
                emp.country = reg.country;
                emp.address = reg.address;
                emp.firstName = reg.firstName;
                emp.middleName = reg.middleName;
                emp.lastName = reg.lastName;
                emp.mobile = reg.mobile;
                emp.DOB = Convert.ToDateTime(reg.dob);
                emp.about = reg.about;
                //if (reg.uploadFile != null)
                //{
                //    var picpath = ImageServer.saveImage(reg.uploadFile);
                //    user.picPath = picpath;
                //}
                await _context.SaveChangesAsync();
                return Ok(reg);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
