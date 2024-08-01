using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ActionTrakingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectManpowerPlanController : ControllerBase
    {
        private readonly DAL _context;
        public ProjectManpowerPlanController(DAL context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("getManpowerPlan")]
        public async Task<IActionResult> GetManpowerPlan(int siteId, int chnageType)
        {
            try
            {

                var FirstPhase = await (from aaa in _context.ProjectStartPlan.Where(hh => hh.siteId == siteId & hh.isDeleted==0)
                                        join plnt in _context.ProjectStartPhase on aaa.id equals plnt.startplanId
                                        join ph in _context.ProjectPhase.Where (aa=>aa.isDeleted == 0) on plnt.PhaseId equals ph.phaseId
                                        select new
                                        {
                                            ph.phaseId,
                                            ph.title,
                                            ph.duration,
                                            plnt.StartDate,
                                            plnt.EndDate,
                                            ph.displayOrder,
                                        }).OrderBy(a => a.displayOrder).Take(1).ToListAsync();



                var Phase = (from a in FirstPhase
                             join plnt in _context.ProjectPlanTask.Where(aa => aa.isDeleted == 0) on a.phaseId equals plnt.phaseId
                             join ph in _context.ProjectTaskAssignment on plnt.taskId equals ph.TaskId
                             select new
                             {
                                 a.phaseId,
                                 plnt.title,
                                 plnt.duration,
                                 ph.StartDate,
                                 ph.EndDate,
                                 a.displayOrder,
                                 plnt.code,
                                 plnt.taskId
                             }).ToList();


                if (Phase.Count > 0)
                {

                    var maxDate =  Phase.Max(p => (DateTime)p.EndDate);
                    var minDate = Phase.Min(p => (DateTime)p.StartDate);


                    DateTime  endDate = new DateTime(maxDate.Year, maxDate.Month, 1);
                    DateTime startDate = new DateTime(minDate.Year, minDate.Month, 1).AddYears(-2);


                    List<DateTime> allDates = new List<DateTime>();
                    DateTime tempDate = startDate;

                    while (tempDate <= endDate)
                    {
                        allDates.Add(new DateTime(tempDate.Year, tempDate.Month, 1));
                        tempDate = tempDate.AddMonths(1);
                    }

                    var isData = await _context.ProjectManpowerPlanData.Where(a => a.siteId == siteId).ToListAsync();
                    List<MyData> data = new List<MyData>();

                    if (isData.Count > 0)
                    {
                        var distinctPositionTitles = isData.Select(a => new { a.position, a.title }).Distinct().ToList();

                        var items2 = from e in distinctPositionTitles
                                     from t in allDates
                                     select new
                                     {
                                         e.title,
                                         positionId = e.position,
                                         Date = t
                                     };

                        data = (from a in items2
                                join pmd in _context.ProjectManpowerPlanData.Where(a => a.siteId == siteId) on new { position = a.positionId, Date = a.Date } equals new { position = pmd.position, Date = pmd.month } into ALLData
                                from d in ALLData.DefaultIfEmpty()
                                select new MyData
                                {
                                    positionId = a.positionId,
                                    Date = a.Date,
                                    title = a.title,
                                    approvedManpower = d == null ? null : (d.approvedManpower == 0 ? null : d.approvedManpower),
                                    positionType = d == null ? 1 : d.positionType,
                                    numberOfPosition = d == null ? null : (d.numberOfPosition == 0 ? null : d.numberOfPosition),
                                    numberOfPositionActual = d == null ? null : (d.numberOfPositionActual == 0 ? null : d.numberOfPositionActual),
                                    
                                    siteId = siteId,
                                    peopleName = d == null ? null : d.peopleName,
                                }).ToList();

                    }
                    else
                    {
                        var positions = await _context.ProjectManpowerPosition.Where(a => a.isDeleted == 0).ToListAsync();

                        var items = from e in positions
                                    from t in allDates
                                    select new
                                    {
                                        e.title,
                                        positionId = e.id,
                                        Date = t
                                    };



                        data = (from a in items
                                join pmd in _context.ProjectManpowerPlanData.Where(a => a.siteId == siteId) on new { positionId = a.positionId, Date = a.Date } equals new { positionId = pmd.position, Date = pmd.month } into ALLData
                                from d in ALLData.DefaultIfEmpty()
                                select new MyData
                                {
                                    positionId = a.positionId,
                                    Date = a.Date,
                                    title = a.title,
                                    approvedManpower = d == null ? null : (d.approvedManpower == 0 ? null : d.approvedManpower),
                                    positionType = d == null ? 1 : d.positionType,
                                    numberOfPosition = d == null ? null : (d.numberOfPosition == 0 ? null : d.numberOfPosition),
                                    numberOfPositionActual = d == null ? null : (d.numberOfPositionActual == 0 ? null : d.numberOfPositionActual),
                                    siteId = siteId,
                                    peopleName = d == null ? null : d.peopleName,

                                }).ToList();
                    }

                    var retData = data.Select(s => new MyData
                    {
                        positionId = s.positionId,
                        Date = s.Date,
                        title = s.title,
                        approvedManpower = data.Where(w => w.positionId == s.positionId).Max(a => a.approvedManpower),
                        numberOfPosition = s.numberOfPosition,
                        numberOfPositionActual = s.numberOfPositionActual,
                        positionType = data.Where(ww => ww.positionId == s.positionId).Max(aa => aa.positionType),
                        siteId = siteId,
                        peopleName = s.peopleName,
                        Ishow=1
                    }).ToList();



                  
                    var yearTotal = retData
                        .GroupBy(a => a.Date)
                        .Select(g => new MyData
                        {
                            positionId = 999999999,
                            Date =g.Key,
                            title = "Position Total",
                            approvedManpower = g.Sum(a => a.approvedManpower),
                            numberOfPosition = g.Sum(a => a.numberOfPosition),
                            numberOfPositionActual = g.Sum(a => a.numberOfPositionActual),
                            positionType = g.Max(a => a.positionType),
                            siteId = siteId,
                            peopleName = "Total",
                            Ishow = 1
                        }).ToList();

                    // Combine the results
                    retData.AddRange(yearTotal);




               
                    
                    var dataobj = new
                    {
                        startDate = startDate,
                        endDate = endDate,
                        DataList = retData,
                        phase= Phase,
                    };
                    return Ok(dataobj);
                }
              

                return Ok(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("saveManpowerPlan")]
        public async Task<IActionResult> SaveManpowerPlan(ManpowerPlanDto reg)
        {
            try
            {
                List<JObject> jObjects = reg.ManpowerPlans.Select(mp => JObject.FromObject(mp)).ToList();
                DataTable dataTable1 = ConvertJObjectListToDataTable(jObjects);

                if (jObjects.Count > 0)
                {
                    SqlParameter _DetailModel1 = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "ManPowerPositionDto",
                        ParameterName = "@ManPowerPositionDto",
                        Value = dataTable1
                    };

                    SqlParameter _CreatedBy = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@CreatedBy",
                        Value = reg.userId
                    };
                    _context.Database.ExecuteSqlCommand("InsertProjectManpowerPlanData @ManPowerPositionDto,@CreatedBy", _DetailModel1, _CreatedBy);
                }



                await _context.SaveChangesAsync();
                return Ok(reg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public DataTable ConvertJObjectListToDataTable(List<JObject> jObjects)
        {
            DataTable dataTable = new DataTable();
            if (jObjects.Count == 0)
            {
                return dataTable;
            }

            var firstObject = jObjects[0];
            foreach (var property in firstObject.Properties())
            {
                dataTable.Columns.Add(property.Name, typeof(string));
            }


            foreach (var jObject in jObjects)
            {
                var row = dataTable.NewRow();
                foreach (var property in jObject.Properties())
                {
                    row[property.Name] = property.Value.ToString();
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;

        }


        public class MyData
        {
            public int positionId { get; set; }
            public DateTime Date { get; set; }
            public string title { get; set; }
            public int? approvedManpower { get; set; }
            public int positionType { get; set; }
            public int? numberOfPosition { get; set; }
            public int siteId { get; set; }
            public string peopleName { get; set; }
            public int? numberOfPositionActual { get; set; }
            public int Ishow { get; set; }
            

        }
    }
}
