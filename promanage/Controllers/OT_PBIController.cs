using ActionTrakingSystem.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using ActionTrakingSystem.Model;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ActionTrakingSystem.Controllers
{

    public class OT_PBIController : BaseAPIController
    {
        private readonly DAL _context;
        public OT_PBIController(DAL context)
        {
            _context = context;
        }
        [HttpGet("getOutageTracker")]
        public async Task<IActionResult> GetOutageTracker()
        {
            try
            {
                DateTime filterDate = new DateTime(2023, 12, 31);
                var outagesInfo = await (from d in _context.OT_Phase.Where(a => a.isDeleted == 0)
                                         join e in _context.OT_PhaseDuration.Where(a => a.isDeleted == 0) on d.phaseId equals e.phaseId
                                         select new
                                         {
                                             d.phaseId,
                                             e.outageId,
                                             e.durationMonths
                                         }
                ).ToListAsync();

                var action = await (from b in _context.OT_PhaseReadinessDescriptionAO.Where(a => a.isDeleted == 0)
                                    join c in _context.OT_PhaseReadinessDescription.Where(a => a.isDeleted == 0) on b.phaseReadId equals c.phaseReadId
                                    join d in _context.OT_Phase.Where(a => a.isDeleted == 0) on c.phaseId equals d.phaseId
                                    join e in _context.OT_PhaseDuration.Where(a => a.isDeleted == 0) on d.phaseId equals e.phaseId
                                    join f in _context.OT_SiteNextOutages.Where(a => a.isDeleted == 0 && a.nextOutageDate > filterDate) on e.outageId equals f.outageId
                                    join ff in _context.OT_ISiteOutages.Where(a => a.isDeleted == 0) on f.outageId equals ff.outageId
                                    join eq in _context.OT_SiteEquipment.Where(a => a.isDeleted == 0) on f.equipmentId equals eq.equipmentId
                                    join g in _context.Sites.Where(a => a.isDeleted == 0 && (a.otValid == 1)) on eq.siteId equals g.siteId
                                    join i in _context.OT_PhaseOutageTracker.Where(a => a.isDeleted == 0)
                                                on new { PhaseId = d.phaseId, PhaseReadId = c.phaseReadId, SNOId = f.snoId }
                                            equals new { PhaseId = i.phaseId, PhaseReadId = i.phaseReadId, SNOId = i.snoId??-1 } into all
                                    from ii in all.DefaultIfEmpty()
                                    join jik in _context.OT_PhaseOutageTrackerProgress on ii.potId equals jik.potId into allzx
                                    from iizx in allzx.DefaultIfEmpty()
                                    join j in _context.OT_IStatus on ii.statusId equals j.statusId into all2
                                    from jj in all2.DefaultIfEmpty()
                                    select new OutageTrackerModel
                                    {
                                        siteId = g.siteId,
                                        siteTitle = g.siteName,
                                        phaseId = d.phaseId,
                                        phaseNumber = d.phaseNumber,
                                        phaseTitle = d.phaseTitle,
                                        phaseReadId = c.phaseReadId,
                                        phaseReadDesc = c.phaseReadDesc,
                                        notApplicable = ii.notApplicable == null || ii.notApplicable == 0 ? false : true,
                                        statusId = jj.statusId == null ? 1 : jj.statusId,
                                        statusTitle = jj.statusTitle == null ? "Pending" : jj.statusTitle,
                                        potId = ii.potId == null ? -1 : ii.potId,
                                        snoId = f.snoId,
                                        outageId = f.outageId,
                                        nextOutageDate = f.nextOutageDate,
                                        outageTitle = ff.outageTitle,
                                        phaseDurId = e.phaseDurId,
                                        durationMonths = e.durationMonths,
                                        equipmentId = eq.equipmentId,
                                        unit = eq.unit,
                                        startDate = DateTime.Now,
                                        endDate = DateTime.Now,
                                        progress = iizx.progress,
                                    }
                                    ).Distinct().OrderBy(a => a.nextOutageDate).ToListAsync();

                var actions = action.GroupBy(a => new
                {
                    a.siteId,
                    a.siteTitle,
                    a.phaseId,
                    a.phaseNumber,
                    a.phaseTitle,
                    a.phaseReadId,
                    a.phaseReadDesc,
                    a.notApplicable,
                    a.statusId,
                    a.statusTitle,
                    a.potId,
                    a.outageId,
                    a.nextOutageDate,
                    a.outageTitle,
                    a.phaseDurId,
                    a.equipmentId,
                    a.unit,
                    a.durationMonths,
                })
.Select(group => new OutageTrackerModel
{
    siteId = group.FirstOrDefault().siteId,
    siteTitle = group.FirstOrDefault().siteTitle,
    phaseId = group.FirstOrDefault().phaseId,
    phaseNumber = group.FirstOrDefault().phaseNumber,
    phaseTitle = group.FirstOrDefault().phaseTitle,
    phaseReadId = group.FirstOrDefault().phaseReadId,
    phaseReadDesc = group.FirstOrDefault().phaseReadDesc,
    notApplicable = group.FirstOrDefault().notApplicable,
    statusId = group.FirstOrDefault().statusId,
    statusTitle = group.FirstOrDefault().statusTitle,
    potId = group.FirstOrDefault().potId,
    outageId = group.FirstOrDefault().outageId,
    nextOutageDate = group.FirstOrDefault().nextOutageDate,
    outageTitle = group.FirstOrDefault().outageTitle,
    durationMonths = group.FirstOrDefault().durationMonths,
    phaseDurId = group.FirstOrDefault().phaseDurId,
    equipmentId = group.FirstOrDefault().equipmentId,
    unit = group.FirstOrDefault().unit,
    startDate = DateTime.Now, // You can change this value as needed
    endDate = DateTime.Now,   // You can change this value as needed
    progress = group.Max(a => a.progress),
})
.ToList();

                var todayDate = DateTime.Now;
                for (var a = 0; a < actions.Count; a++)
                {
                    var uniqueEquipments = outagesInfo.Where(b => b.outageId == actions[a].outageId && b.durationMonths < actions[a].durationMonths).OrderByDescending(a => a.durationMonths).ToList();
                    var uniqueNegativeEquipments = outagesInfo.Where(b => b.outageId == actions[a].outageId && b.durationMonths > actions[a].durationMonths && b.durationMonths < 0).OrderByDescending(a => a.durationMonths).ToList();

                    if (actions[a].durationMonths > 0)
                    {
                        actions[a].startDate = actions[a].nextOutageDate.AddMonths(-(int)((actions[a].durationMonths == null) ? 0 : actions[a].durationMonths));
                    }
                    if (actions[a].durationMonths < 0)
                    {
                        actions[a].startDate = actions[a].nextOutageDate.AddMonths(-(int)((uniqueNegativeEquipments.Count == 0) ? 0 : uniqueNegativeEquipments[0].durationMonths));
                    }
                    DateTime obd = actions[a].startDate;
                    var dds = (uniqueEquipments.Count > 0 ? (actions[a].durationMonths - ((uniqueEquipments.FirstOrDefault().durationMonths == 0) ? 0 : uniqueEquipments.FirstOrDefault().durationMonths)) : 0);
                    if (actions[a].durationMonths > 0)
                    {
                        actions[a].endDate = obd.AddMonths((int)dds);
                    }
                    if (actions[a].durationMonths < 0)
                    {
                        actions[a].endDate = actions[a].startDate.AddMonths(-(int)((actions[a].durationMonths == null) ? 0 : ((uniqueNegativeEquipments.Count == 0) ? actions[a].durationMonths : (actions[a].durationMonths - uniqueNegativeEquipments[0].durationMonths))));
                    }
                   
                }
                return Ok(actions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
