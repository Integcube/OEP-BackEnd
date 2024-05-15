using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class ClusterController : BaseAPIController
    {
        private readonly DAL _context;
        public ClusterController(DAL context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("getCluster")]
        public async Task<IActionResult> GetCluster(UserIdDto reg)
        {
            try
            {
                var cluster = await (from a in _context.Cluster.Where(a => a.isDeleted == 0)
                                     join r in _context.Regions2.Where(a => a.isDeleted == 0) on a.regionId equals r.regionId
                                     join e in _context.ClusterExecutiveVp on a.clusterId equals e.clusterId into all
                                     from ee in all.DefaultIfEmpty()
                                     join u in _context.AppUser on a.executiveDirector equals u.userId into all2
                                     from uu in all2.DefaultIfEmpty()
                                     join ue in _context.AppUser on ee.userId equals ue.userId into all3
                                     from uee in all3.DefaultIfEmpty()
                                     select new
                                     {
                                         a.clusterId,
                                         a.regionId,
                                         regionTitle = r.title,
                                         a.clusterTitle,
                                         a.clusterCode,
                                         executiveDirectorId = a.executiveDirector,
                                         executiveDirectorTitle = uu.userName,
                                         ee.executiveVpId,
                                         executiveVpTitle = uee.userName,
                                     }
                                     ).ToListAsync();
                var groupedDate = cluster.GroupBy(a => a.clusterId).Select(a => new
                {
                    a.FirstOrDefault().clusterId,
                    a.FirstOrDefault().clusterTitle,
                    a.FirstOrDefault().clusterCode,
                    a.FirstOrDefault().executiveDirectorId,
                    a.FirstOrDefault().executiveDirectorTitle,
                    a.FirstOrDefault().regionTitle,
                    a.FirstOrDefault().regionId,
                    executiveVpName = string.Join(", ", a.Select(u => u.executiveVpTitle))

                }).ToList();
                return Ok(groupedDate);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("saveCluster")]
        public async Task<IActionResult> SaveCluster(ClusterSaveUserDto reg)
        {
            try
            {
                if (reg.cluster.clusterId == -1)
                {
                    Cluster c = new Cluster();
                    c.clusterTitle = reg.cluster.clusterTitle;
                    c.clusterCode = reg.cluster.clusterCode;
                    c.regionId = reg.cluster.regionId;
                    c.executiveDirector = reg.cluster.executiveDirectorId;
                    c.createdBy = reg.userId;
                    c.createdOn = DateTime.Now;
                    c.isDeleted = 0;
                    _context.Add(c);
                    _context.SaveChanges();
                    reg.cluster.clusterId = c.clusterId;
                    foreach(var vp in reg.cluster.executiveVps)
                    {
                        ClusterExecutiveVp v = new ClusterExecutiveVp();
                        v.userId = vp.userId;
                        v.clusterId = c.clusterId;
                        v.createdOn = DateTime.Now;
                        v.createdBy = reg.userId;
                        _context.Add(v);
                        _context.SaveChanges();
                    }

                }
                else
                {
                    Cluster c = await (from a in _context.Cluster.Where(a => a.clusterId == reg.cluster.clusterId)
                                             select a).FirstOrDefaultAsync();
                    c.clusterTitle = reg.cluster.clusterTitle;
                    c.clusterCode = reg.cluster.clusterCode;
                    c.regionId = reg.cluster.regionId;
                    c.executiveDirector = reg.cluster.executiveDirectorId;
                    c.modifiedBy = reg.userId;
                    c.modifiedOn = DateTime.Now;
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlCommand("DELETE FROM ClusterExecutiveVp WHERE clusterId = @clusterId", new SqlParameter("clusterId", reg.cluster.clusterId));
                    foreach (var vp in reg.cluster.executiveVps)
                    {
                        ClusterExecutiveVp v = new ClusterExecutiveVp();
                        v.userId = vp.userId;
                        v.clusterId = c.clusterId;
                        v.createdOn = DateTime.Now;
                        v.createdBy = reg.userId;
                        _context.Add(v);
                        _context.SaveChanges();
                    }

                }
                return Ok(reg.cluster);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteCluster")]
        public async Task<IActionResult> DeleteCluster(ClusterUserDto reg)
        {
            try
            {
                Cluster cluster = await (from a in _context.Cluster.Where(a => a.clusterId == reg.cluster.clusterId)
                                     select a).FirstOrDefaultAsync();
                cluster.isDeleted = 1;
                _context.SaveChanges();
                return Ok(reg.cluster);
              
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("getClusterInfo")]
        public async Task<IActionResult> GetClusterInfo(ClusterUserDto reg)
        {
            try
            {
                var clusterInfo = await (from a in _context.Cluster.Where(a => a.clusterId == reg.cluster.clusterId)
                                         join b in _context.ClusterExecutiveVp on a.clusterId equals b.clusterId
                                         join u in _context.AppUser on b.userId equals u.userId
                                         select new
                                         {
                                             b.userId,
                                             u.userName,
                                         }

                                        ).ToListAsync();
                return Ok(clusterInfo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
