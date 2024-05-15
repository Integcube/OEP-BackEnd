using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WH_CoeController : ControllerBase
    {
        private readonly DAL _context;
        public WH_CoeController(DAL context)
        {
            _context = context;
        }
        public class RunningHourData
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string Equipment { get; set; }
            public int YearId { get; set; }
            public int MonthId { get; set; }
            public double woh { get; set; }
            public double wce { get; set; }
        }
        public string RemoveSpaces(string input)
        {
            string trimmed = input.Trim();
            string cleaned = string.Join("", trimmed.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            return cleaned;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoeData()
        {
            string baseUrl = "https://dashboard.nomac.com/api/";
            string endpoint = "runninghour";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        List<RunningHourData> runningHours = JsonConvert.DeserializeObject<List<RunningHourData>>(responseData);

                        foreach (var data in runningHours)
                        {
                            var cleanedEquipment = RemoveSpaces(data.Equipment);

                            var equipment = await (from a in _context.WH_SiteEquipment.Where(a => a.unit == cleanedEquipment && a.siteId == data.SiteId)
                                                   select a)
                                                   .FirstOrDefaultAsync();
                            if (equipment == null)
                            {
                                WH_SiteEquipment siteEq = new WH_SiteEquipment();
                                siteEq.isDeleted = 0;
                                siteEq.createdDate = DateTime.Now;
                                siteEq.createdBy = 1;
                                siteEq.siteId = data.SiteId;
                                siteEq.fleetEquipmentId = 1;
                                siteEq.unitSN = cleanedEquipment;
                                siteEq.details = "";
                                siteEq.responsible = 1;
                                siteEq.unitCOD = DateTime.Now;
                                siteEq.unit = cleanedEquipment;
                                _context.Add(siteEq);
                                _context.SaveChanges();
                                var sites = await (from a in _context.Sites.Where(a => a.siteId == data.SiteId)
                                                   select a).FirstOrDefaultAsync();
                                if (sites != null)
                                {
                                    var equipmentIdParameter = new SqlParameter("@equipmentId", siteEq.equipmentId);
                                    var endOfContractDateParameter = new SqlParameter("@endOfContractDate", sites.onmContractExpiry);
                                    var userIdParameter = new SqlParameter("@userId", 1);
                                    //var coePara = new SqlParameter("@fromCoe", 0);
                                    _context.Database.ExecuteSqlRaw("EXEC InsertOrUpdateRunningHours @equipmentId, @endOfContractDate, @userId", equipmentIdParameter, endOfContractDateParameter, userIdParameter);
                                }
                                WH_StartingHours sh = new WH_StartingHours();
                                sh.startHours = 0;
                                sh.startDate = new DateTime(2023, 12, 31);
                                sh.unitId = siteEq.equipmentId;
                                sh.isDeleted = 0;
                                sh.createdOn = DateTime.Now;
                                sh.createdBy = 1;
                                _context.Add(sh);
                                _context.SaveChanges();

                                WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a => a.monthId == data.MonthId && a.yearId == data.YearId && a.unitId == siteEq.equipmentId)
                                                            select a).FirstOrDefaultAsync();
                                if (ys != null)
                                {
                                    ys.runningHour = (decimal)data.woh;
                                    ys.wce_runningHour = (decimal)data.wce;
                                    ys.createdBy = 1;
                                    ys.fromCoe = 1;
                                    ys.createdOn = DateTime.Now;
                                    _context.SaveChanges();
                                }

                            }
                            else
                            {
                                var cv = await (from a in _context.WH_StartingHours.Where(a => a.unitId == equipment.equipmentId)
                                                select a
                       ).FirstOrDefaultAsync();
                                if(cv == null)
                                {
                                    WH_StartingHours sh = new WH_StartingHours();
                                    sh.startHours = 0;
                                    sh.startDate = new DateTime(2023, 12, 31);
                                    sh.unitId = equipment.equipmentId;
                                    sh.isDeleted = 0;
                                    sh.createdOn = DateTime.Now;
                                    sh.createdBy = 1;
                                    _context.Add(sh);
                                    _context.SaveChanges();
                                }
                                WH_MonthlyHours ys = await (from a in _context.WH_MonthlyHours.Where(a => a.monthId == data.MonthId && a.yearId == data.YearId && a.unitId == equipment.equipmentId)
                                                            select a).FirstOrDefaultAsync();
                                if (ys != null)
                                {
                                    ys.runningHour = (decimal)data.woh;
                                    ys.wce_runningHour = (decimal)data.wce; 
                                    ys.fromCoe = 1;
                                    ys.createdOn = DateTime.Now;
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return Ok();
        }
    }
}

