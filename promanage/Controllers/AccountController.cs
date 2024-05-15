using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Interfaces;
using ActionTrakingSystem.Model;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class AccountController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly ITokenService _tokenService;
        public AccountController(DAL context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                var user = await (from a in _context.AppUser.Where(a => (login.username == a.userName || login.username == a.email) && a.isDeleted == 0 && a.password == login.password)
                                  select a).SingleOrDefaultAsync();
                if (user != null)
                {
                    var imgBytes = "";
                    //if (user.picPath != null)
                    //{
                    //    imgBytes = ImageServer.getImage(user.picPath);

                    //}
                    UserDto userData = new UserDto();
                    userData.id = user.userId;
                    userData.firstName = user.firstName;
                    userData.lastName = user.lastName;
                    userData.email = user.email;
                    userData.username = user.userName;
                    userData.middleName = user.middleName;
                    userData.img = imgBytes;
                    userData.role = user.role;
                    userData.token = _tokenService.CreateToken(user);
                    return Ok(userData);
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationDto newUser)
        {
            try
            {
                var checkUser = await (from a in _context.AppUser.Where(x => x.userName.ToLower() == newUser.userName.ToLower() && x.email.ToLower() == newUser.email.ToLower() && x.isDeleted == 0)
                                       select a).FirstOrDefaultAsync();
                if (checkUser != null)
                {
                    return BadRequest("User already exists signup using different userName or email");
                }
                var user = new AppUser();
                user.userName = newUser.userName;
                user.email = newUser.email;
                user.password = newUser.password;
                user.firstName = newUser.firstName;
                user.lastName = newUser.lastName;
                user.middleName = newUser.middleName;
                user.role = newUser.role;
                user.isDeleted = 0;
                user.lastLoginId = 0;
                _context.AppUser.Add(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
