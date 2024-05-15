using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WH_UploadGtHoursController : ControllerBase
    {
        private readonly DAL _context;
        public WH_UploadGtHoursController(DAL context)
        {
            _context = context;
        }

    }
}
