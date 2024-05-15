using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class QF_FormController : BaseAPIController
    {
        public readonly DAL _context;
        public QF_FormController(DAL context)
        {
            _context = context;
        }

        [HttpGet("getForm")]
        public async Task<IActionResult> GetForm()
        {
            try
            {
                var formData = await (from q in _context.QF_Questions.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          q.questionId,
                                          q.questionTitle,
                                         

                                      }).ToListAsync();
                var checkBoxes = await (
                                      from c in _context.QF_CheckBoxes.Where(a => a.isDeleted == 0)
                                      select new
                                      {
                                          
                                              c.checkBoxId,
                                              c.checkBoxTitle,
                                              c.color
                                         

                                      }).ToListAsync();
                var obj = formData.Select(a => new
                {
                    a.questionId, a.questionTitle,answer = "",
                    checkBoxes = checkBoxes.Select(b => new
                    {
                        b.checkBoxId
                        , b.color,
                        b.checkBoxTitle,
                        selected=false
                    }).ToList()
                }).ToList();

                return Ok(obj);
            }
            catch(Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("saveForm")]
        public async Task<IActionResult> SaveForm(QF_SaveDataDto reg)
        {
            try
            {
                QF_User u = new QF_User();
                u.firstName = reg.user.firstName;
                u.lastName = reg.user.lastName;
                u.email= reg.user.email;
                u.phone = reg.user.phone;
                u.createdOn = DateTime.Now;

                _context.Add(u);
                _context.SaveChanges();
               
                for(var a = 0; a<reg.form.Length; a++)
                {
                    QF_Answers ans = new QF_Answers();
                    ans.questionId = reg.form[a].questionId;
                    ans.answerTitle = reg.form[a].answer;
                    ans.userId = u.userId;
                    for(var b =0; b < reg.form[a].checkBoxes.Length; b++)
                    {
                        if (reg.form[a].checkBoxes[b].selected == true)
                        {
                            ans.checkBoxId = reg.form[a].checkBoxes[b].checkBoxId;
                        }
                    }
                    _context.Add(ans);
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
