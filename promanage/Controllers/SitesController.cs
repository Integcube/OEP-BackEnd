using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ActionTrakingSystem.Controllers
{
    public class SitesController : BaseAPIController
    {
        private readonly DAL _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SitesController(DAL context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [Authorize]
        [HttpPost("insuranceReport/{siteId}")]
        public async Task<IActionResult> DownloadInsuranceFile(int siteId)
        {
            try
            {
                var sites = await (from a in _context.SitesInsuranceReport.Where(a => a.siteId == Convert.ToInt64(siteId))
                                   select a).FirstOrDefaultAsync();

                if (sites!=null)
                {
                    var insuranceURL = sites.reportPath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + insuranceURL;
                    // Get the file path from the URL

                    // Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", sites.reportName);

                }
                else
                {
                    return Ok(-1);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("tilReport/{siteId}")]
        public async Task<IActionResult> DownloadTilFile(int siteId)
        {
            try
            {
                var sites = await (from a in _context.SitesTilReport.Where(a => a.siteId == siteId)
                                   select a).FirstOrDefaultAsync();

                if (sites != null )
                {
                    var tilUrl = sites.reportPath.TrimStart('~').Replace('/', '\\');
                    // Get the file path from the URL
                    string filePath = _hostingEnvironment.ContentRootPath + '\\' + tilUrl;

                    // Check if the file exists
                    if (!System.IO.File.Exists(filePath))
                    {
                        return Ok(-1);
                    }

                    // Return the file as a FileStreamResult
                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/octet-stream", sites.reportPath);

                }
                else
                {
                    return Ok(-1);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("uploadFile")]
        public async Task<IActionResult> uploadFiles([FromForm] uploadFilesDto reg)
        {
            try
            {
                var siteId = Convert.ToInt32(reg.siteId);

                if (reg.tilsReport != null)
                {
                    string tilFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.tilsReport.FileName);
                    string tilfilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", tilFileName);
                    using (var stream = new FileStream(tilfilePath, FileMode.Create))
                    {
                        await reg.tilsReport.CopyToAsync(stream);
                    }
                    string tilFileUrl = $"~/uploads/{tilFileName}";
                    //_context.Database.ExecuteSqlCommand("DELETE FROM SitesTilReport WHERE siteId = @siteId", new SqlParameter("siteId", siteId));
                    SitesTilReport tr = new SitesTilReport();
                    tr.siteId= siteId;
                    tr.reportName = reg.tilsReport.FileName;
                    tr.reportPath = tilFileUrl;
                    tr.createdOn= DateTime.Now;
                    tr.isDeleted = 0;
                    tr.createdBy =Convert.ToInt32(reg.userId);
                    _context.Add(tr);


                }
                if (reg.insuranceReport != null)
                {
                    string insuranceFileName = Guid.NewGuid().ToString() + Path.GetExtension(reg.insuranceReport.FileName);
                    string insurancefilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", insuranceFileName);
                    using (var stream = new FileStream(insurancefilePath, FileMode.Create))
                    {
                        await reg.insuranceReport.CopyToAsync(stream);
                    }
                    string insuranceFileUrl = $"~/uploads/{insuranceFileName}";
                    //_context.Database.ExecuteSqlCommand("DELETE FROM SitesInsuranceReport WHERE siteId = @siteId", new SqlParameter("siteId", siteId));
                    SitesInsuranceReport tr = new SitesInsuranceReport();
                    tr.siteId = siteId;
                    tr.reportName = reg.insuranceReport.FileName;
                    tr.reportPath = insuranceFileUrl;
                    tr.createdOn = DateTime.Now;
                    tr.isDeleted = 0;
                    tr.createdBy = Convert.ToInt32(reg.userId);
                    _context.Add(tr);
                }
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception E)
            {
                return BadRequest(E.Message);
            }
        }
        [Authorize]
        [HttpPost("deleteSites")]
        public async Task<IActionResult> DeleteSites(SitesI reg)
        {
            try
            {
                Sites sitez = await (from r in _context.Sites.Where(a => a.siteId == reg.siteId)
                                     select r).FirstOrDefaultAsync();
                sitez.isDeleted = 1;
                await _context.SaveChangesAsync();
                return Ok(sitez);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("editData")]
        public async Task<IActionResult> EditData(Sites reg)
        {
            try
            {
                var technologies = await (from a in _context.Technology
                                          join b in _context.SitesTechnology.Where(a => a.siteId == reg.siteId) on a.techId equals b.techId
                                          select new
                                          {
                                              
                                              technologyTitle = a.name,
                                              technologyId = a.techId,
                                          }).ToListAsync();

                return Ok(technologies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getInterfaces")]
        public async Task<IActionResult> GetInterface(UserIdDto reg)
        {
            try
            {
             
                var siteProjectStatus = await (from r in _context.SiteProjectStatus.Where(a => a.isDeleted == 0)
                                               select new
                                               {
                                                   r.projectStatusTitle,
                                                   r.projectStatusId,
                                               }).ToListAsync();
         
                return Ok(siteProjectStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("saveSites")]
        public async Task<IActionResult> SaveSites(sSiteDto reg)
        {
            try
            {
                if (reg.site.sites.siteId == -1)
                {
                    Sites site = new Sites();
                    site.siteName = reg.site.sites.siteName;
                    site.countryId = reg.site.sites.countryId;
                    site.regionId = reg.site.sites.regionId;
                    site.projectCompany = reg.site.sites.projectCompany;
                    site.siteDescription = reg.site.sites.siteDescription;
                    site.projectStatusId = reg.site.sites.projectStatusId;
                    site.projectCOD = reg.site.sites.projectCOD;
                    site.tilPocId = reg.site.sites.tilPOCId;
                    site.onmContractExpiry = (DateTime)reg.site.sites.onmContractExpiry;
                    site.insuranceLastReportDate = reg.site.sites.insuranceLastReportDate;
                    site.insuranceNextAuditDate = reg.site.sites.insuranceNextAuditDate;
                    site.sitePGMId = reg.site.sites.sitePGMId;
                    site.insurancePOCId = reg.site.sites.insurancePOCId;
                    site.insuranceSummary = reg.site.sites.insuranceSummary;
                    site.tilsSummary = reg.site.sites.tilsSummary;
                    site.sitePGMId = reg.site.sites.sitePGMId;
                    site.clusterId = reg.site.sites.clusterId;
                    site.siteEMOId = reg.site.sites.siteEMOId;
                    site.isDeleted = 0;
                    site.createdDate = DateTime.Now;
                    site.createdBy = reg.userId;
                    site.onmName = reg.site.sites.onmName;
                    site.projectName = reg.site.sites.projectName;
                    site.s4hanaCode = reg.site.sites.s4hanaCode;
                    _context.Add(site);
       
                    _context.SaveChanges();


                    for (var i = 0; i < reg.site.techlogies.Length; i++)
                    {
                        SitesTechnology role = new SitesTechnology();
                        role.isDeleted = 0;
                        role.siteId = site.siteId;
                        role.techId = reg.site.techlogies[i].technologyId;
                        _context.Add(role);
                        _context.SaveChanges();
                    }
                    AUSite ss = new AUSite();
                    ss.isDeleted = 0;
                    ss.userId = reg.userId;
                    ss.siteId = site.siteId;
                    _context.Add(ss);
                    _context.SaveChanges();
                    if(reg.site.sites.tilPOCId != null)
                    {
                        TILAccessControl tac = new TILAccessControl();
                        tac.isPoc = 1;
                        tac.userId = (int)reg.site.sites.tilPOCId;
                        tac.siteId = site.siteId;
                        _context.Add(tac);
                        _context.SaveChanges();
                    }
                    if (reg.site.sites.insurancePOCId != null)
                    {
                        InsuranceAccessControl iac = new InsuranceAccessControl();
                        iac.isPoc = 1;
                        iac.userId = (int)reg.site.sites.insurancePOCId;
                        iac.siteId = site.siteId;
                        _context.Add(iac);
                        _context.SaveChanges();
                    }
                    reg.site.sites.siteId = site.siteId;
                    return Ok(reg.site.sites);
                }
                else
                {
                    Sites site = await (from s in _context.Sites.Where(s => s.siteId == reg.site.sites.siteId)
                                        select s).FirstOrDefaultAsync();
                    if (site != null)
                    {
                        site.siteName = reg.site.sites.siteName;
                        site.countryId = reg.site.sites.countryId;
                        site.regionId = reg.site.sites.regionId;
                        site.projectCompany = reg.site.sites.projectCompany;
                        site.siteDescription = reg.site.sites.siteDescription;
                        site.projectStatusId = reg.site.sites.projectStatusId;
                        site.projectCOD = reg.site.sites.projectCOD;
                        site.onmContractExpiry = (DateTime)reg.site.sites.onmContractExpiry;
                        site.insuranceLastReportDate = reg.site.sites.insuranceLastReportDate;
                        site.insuranceNextAuditDate = reg.site.sites.insuranceNextAuditDate;
                        site.sitePGMId = reg.site.sites.sitePGMId;
                        site.insurancePOCId = reg.site.sites.insurancePOCId;
                        site.insuranceSummary = reg.site.sites.insuranceSummary;
                        site.tilsSummary = reg.site.sites.tilsSummary;
                        site.siteEMOId = reg.site.sites.siteEMOId;
                        site.tilPocId = reg.site.sites.tilPOCId;
                        site.clusterId = reg.site.sites.clusterId;

                        site.sitePGMId = reg.site.sites.sitePGMId;
                        site.onmName = reg.site.sites.onmName;
                        site.projectName = reg.site.sites.projectName;
                        site.s4hanaCode = reg.site.sites.s4hanaCode;
                        site.modifiedDate = DateTime.Now;
                        site.modifiedBy = reg.userId;
                        if (reg.site.techlogies.Length > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM SitesTechnology WHERE siteId = @siteId", new SqlParameter("siteId", site.siteId));
                            for (var i = 0; i < reg.site.techlogies.Length; i++)
                            {
                                SitesTechnology role = new SitesTechnology();
                                role.isDeleted = 0;
                                role.siteId = site.siteId;
                                role.techId = reg.site.techlogies[i].technologyId;
                                _context.Add(role);
                                _context.SaveChanges();
                            }
                        }
                        var iact = (from a in _context.InsuranceAccessControl.Where(a => a.isPoc == 1 && a.siteId == site.siteId && a.userId == site.insurancePOCId)
                                    select a).FirstOrDefault();
                        if(iact != null)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM InsuranceAccessControl WHERE siteId = @siteId", new SqlParameter("siteId", site.siteId));
                           
                                InsuranceAccessControl iac = new InsuranceAccessControl();
                                iac.isPoc = 1;
                                iac.userId = (int)site.insurancePOCId;
                                iac.siteId = site.siteId;
                                _context.Add(iac);
                                _context.SaveChanges();
                            
                        }
                        var tact = (from a in _context.TILAccessControl.Where(a => a.isPoc == 1 && a.siteId == site.siteId && a.userId == site.tilPocId)
                                    select a).FirstOrDefault();
                        if (tact != null)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TILAccessControl WHERE siteId = @siteId", new SqlParameter("siteId", site.siteId));
                            TILAccessControl tac = new TILAccessControl();
                            tac.isPoc = 1;
                            tac.userId = (int)site.tilPocId;
                            tac.siteId = site.siteId;
                            _context.Add(tac);
                            _context.SaveChanges();

                        }
                        if (site.onmContractExpiry > new DateTime(2024, 1, 1))
                        {
                            var equipments = await (from a in _context.WH_SiteEquipment.Where(a => a.siteId == reg.site.sites.siteId) select a).ToListAsync();
                            foreach (var e in equipments)
                            {
                                var equipmentIdParameter = new SqlParameter("@equipmentId", e.equipmentId);
                                var endOfContractDateParameter = new SqlParameter("@endOfContractDate", site.onmContractExpiry);
                                var userIdParameter = new SqlParameter("@userId", reg.userId);
                                _context.Database.ExecuteSqlRaw("EXEC InsertOrUpdateRunningHours @equipmentId, @endOfContractDate, @userId", equipmentIdParameter, endOfContractDateParameter, userIdParameter);
                            }
                        }

                        

                    }
                    return Ok(reg.site.sites);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("getSites")]
        public async Task<IActionResult> GetSites(filterSiteDto reg)
        {
            try
            {
                List<int> countryIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.countryList))
                    countryIds = (reg.countryList.Split(',').Select(Int32.Parse).ToList());

                List<int> RegionIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.regionList))
                    RegionIds = (reg.regionList.Split(',').Select(Int32.Parse).ToList());
                List<int> technologyIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.technologyList))
                    technologyIds = (reg.technologyList.Split(',').Select(Int32.Parse).ToList());
                List<int> clusterIds = new List<int>();
                if (!string.IsNullOrEmpty(reg.clusterList))
                    clusterIds = (reg.clusterList.Split(',').Select(Int32.Parse).ToList());

                var sites = await (from s in _context.Sites.Where(a => a.isDeleted == 0)
                                   join ps in _context.SiteProjectStatus on s.projectStatusId equals ps.projectStatusId into all
                                   from aa in all.DefaultIfEmpty()
                                   join cl in _context.Cluster.Where(a => a.isDeleted == 0 && ((clusterIds.Count == 0) || clusterIds.Contains((int)a.clusterId))) on s.clusterId equals cl.clusterId
                                   join r in _context.Regions.Where(a=>a.isDeleted == 0 && ((RegionIds.Count == 0) || RegionIds.Contains((int)a.regionId))) on s.regionId equals r.regionId 
                                   join c in _context.Country.Where(a => a.isDeleted == 0 && ((countryIds.Count == 0) || countryIds.Contains((int)a.countryId))) on s.countryId equals c.countryId 
                                   join ts in _context.SitesTechnology.Where(a => a.isDeleted == 0 && ((technologyIds.Count == 0) || technologyIds.Contains((int)a.techId))) on s.siteId equals ts.siteId 
                                   join aus in _context.AUSite.Where(a => a.userId == reg.userId) on s.siteId equals aus.siteId
                                   join aut in _context.AUTechnology.Where(a => a.userId == reg.userId) on ts.techId equals aut.technologyId
                                   join au in _context.AppUser on s.siteEMOId equals au.userId into all5
                                   from ee in all5.DefaultIfEmpty()
                                   join au in _context.AppUser on s.sitePGMId equals au.userId into all6
                                   from ff in all6.DefaultIfEmpty()
                                   join au in _context.AppUser on s.insurancePOCId equals au.userId into all7
                                   from gg in all7.DefaultIfEmpty()
                                   join au in _context.AppUser on s.tilPocId equals au.userId into all8
                                   from ggg in all8.DefaultIfEmpty()
                                   select new
                                   {
                                       siteId = s.siteId,
                                       siteName = s.siteName,
                                       countryId = s.countryId,
                                       country = c.title,
                                       regionId = s.regionId,
                                       region = r.title,
                                       clusterId = s.clusterId,
                                       clusterTitle = cl.clusterTitle,
                                       projectCompany = s.projectCompany,
                                       siteDescription = s.siteDescription,
                                       projectStatusId = aa.projectStatusId,
                                       projectStatus = aa.projectStatusTitle,
                                       projectCOD = s.projectCOD,
                                       onmContractExpiry = s.onmContractExpiry,
                                       insuranceLastReportDate = s.insuranceLastReportDate,
                                       insuranceNextAuditDate = s.insuranceNextAuditDate,
                                       sitePGMId = s.sitePGMId,
                                       sitePGMName = ff.userName,
                                       siteEMOId = s.siteEMOId,
                                       siteEMO = ee.userName,
                                       insurancePOCId = s.insurancePOCId,
                                       insurancePOC = gg.userName,
                                       tilPOCId = s.tilPocId,
                                       tilPOC = ggg.userName,
                                       insuranceSummary = s.insuranceSummary,
                                       tilsSummary = s.tilsSummary,
                                      s.s4hanaCode,
                                      s.onmName,
                                      s.projectName,

                                   }).Distinct().OrderByDescending(z => z.siteId).ToListAsync();

               
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
