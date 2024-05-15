using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{

    public class CountryController : BaseAPIController
    {
        private readonly DAL _context;
        public CountryController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getCountries")]
        public async Task<IActionResult> GetCountries(UserIdDto reg)
        {
            try
            {
                var countries = await (from a in _context.Country.Where(a => a.isDeleted == 0)
                                       join r in _context.Regions on a.regionId equals r.regionId
                                       //join c in _context.Cluster on a.clustedId equals c.clusterId into aalz
                                       //from cc in aalz.DefaultIfEmpty()
                                       join du in _context.AppUser on a.executiveDirector equals du.userId into all2
                                       from duu in all2.DefaultIfEmpty()
                                       join vp in _context.CountryExecutiveVp on a.countryId equals vp.countryId into all
                                       from vpp in all.DefaultIfEmpty()
                                       join vpu in _context.AppUser on vpp.userId equals vpu.userId into all3
                                       from vpuu in all3.DefaultIfEmpty()
                                       select new
                                       {
                                           a.regionId,
                                           regionTitle = r.title,
                                           a.countryId,
                                           countryTitle = a.title,
                                           countryCode = a.code,
                                           executiveDirectorId = a.executiveDirector,
                                           executiveDirectorTitle = duu.userName,
                                           executiveVpId = vpp.userId,
                                           executiveVpTitle = vpuu.userName,
                                           //a.clustedId,
                                           //cc.clusterTitle,
                                       }).OrderByDescending(a => a.countryId).ToListAsync();
                var groupedData = countries.GroupBy(a => a.countryId).Select(group => new
                {
                    countryId = group.Key,
                    regionId = group.FirstOrDefault().regionId,
                    regionTitle = group.FirstOrDefault().regionTitle,
                    countryTitle = group.FirstOrDefault().countryTitle,
                    //clustedId = group.FirstOrDefault().clustedId,
                    //clusterTitle = group.FirstOrDefault().clusterTitle,
                    countryCode = group.FirstOrDefault().countryCode,
                    executiveDirectorId = group.FirstOrDefault().executiveDirectorId,
                    executiveDirectorTitle = group.FirstOrDefault().executiveDirectorTitle,
                    executiveVpId = string.Join(", ", group.Select(u => u.executiveVpId)),
                    executiveVpTitle = string.Join(", ", group.Select(u => u.executiveVpTitle)),
                }).ToList();
                return Ok(groupedData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("deleteCountry")]
        public async Task<IActionResult> DeleteCountry(deleteCountryDto reg)
        {
            try
            {
                Country country = await (from r in _context.Country.Where(a => a.countryId == reg.country.countryId)
                                         select r).FirstOrDefaultAsync();
                country.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("getUsers")]
        public async Task<IActionResult> GetUsers(CommonFilterDto reg)
        {
            try
            {
                var users = await (from s in _context.AppUser.Where(a => a.isDeleted == 0)
                                   join aus in _context.AUSite on s.userId equals aus.userId
                                   join a in _context.Sites.Where(a => a.isDeleted == 0 && (reg.siteId == -1 || a.siteId == reg.siteId)) on aus.siteId equals a.siteId
                                   join c in _context.Country.Where(a => a.isDeleted == 0) on a.countryId equals c.countryId
                                   select new
                                   {
                                       s.userId,
                                       s.userName,
                                       name = s.firstName + " " + s.lastName,
                                       s.email,
                                   }
                                  ).Distinct().ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSelectedData")]
        public async Task<IActionResult> GetSelectedData(SelectedCountryDto reg)
        {
            try
            {
                var users = await (from s in _context.AppUser.Where(a => a.isDeleted == 0)
                                   join c in _context.CountryExecutiveVp.Where(a => a.countryId == reg.countryId) on s.userId equals c.userId
                                   select new
                                   {
                                       s.userId,
                                       s.userName,
                                       name = s.firstName + " " + s.lastName,
                                       s.email,
                                   }
                                  ).ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveCountry")]
        public async Task<IActionResult> SaveCountry(SaveCountryDto reg)
        {
            try
            {
                if (reg.country.country.countryId == -1)
                {
                    Country c = new Country();
                    c.isDeleted = 0;
                    c.title = reg.country.country.countryTitle;
                    c.code = reg.country.country.countryCode;
                    c.regionId = reg.country.country.regionId;
                    c.executiveDirector = reg.country.country.executiveDirectorId;
                    c.createdBy = reg.userId;
                    //c.clustedId = reg.country.country.clustedId;
                    c.createdOn = DateTime.Now;
                    _context.Add(c);
                    _context.SaveChanges();
                    for (var a = 0; a < reg.country.executiveVp.Length; a++)
                    {
                        CountryExecutiveVp vp = new CountryExecutiveVp();
                        vp.countryId = c.countryId;
                        vp.userId = reg.country.executiveVp[a].userId;
                        vp.createdOn = DateTime.Now;
                        vp.createdBy = reg.userId;
                        _context.Add(vp);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    Country c = await (from r in _context.Country.Where(a => a.countryId == reg.country.country.countryId)
                                       select r).FirstOrDefaultAsync();
                    c.title = reg.country.country.countryTitle;
                    c.code = reg.country.country.countryCode;
                    c.regionId = reg.country.country.regionId;
                    c.executiveDirector = reg.country.country.executiveDirectorId;
                    c.modifiedBy = reg.userId;
                    //c.clustedId = reg.country.country.clustedId;
                    c.modifiedOn = DateTime.Now;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM CountryExecutiveVp WHERE countryId = @countryId", new SqlParameter("countryId", reg.country.country.countryId));
                    for (var a = 0; a < reg.country.executiveVp.Length; a++)
                    {
                        CountryExecutiveVp vp = new CountryExecutiveVp();
                        vp.countryId = c.countryId;
                        vp.userId = reg.country.executiveVp[a].userId;
                        vp.createdOn = DateTime.Now;
                        vp.createdBy = reg.userId;
                        _context.Add(vp);
                        _context.SaveChanges();
                    }
                }
                return Ok(reg.country.country);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getClusters")]
        public async Task<IActionResult> getClusters(CommonFilterDto reg)
        {
            try
            {
                var users = await (from s in _context.Cluster.Where(a => a.isDeleted == 0)

                                   select new
                                   {
                                       s.clusterId,
                                       s.clusterTitle,
                                   }
                                  ).Distinct().ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);


            }
        }
    }
}
