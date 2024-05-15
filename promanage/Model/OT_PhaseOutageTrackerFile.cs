using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseOutageTrackerFile
    {
        [Key]
        public int fileId { get; set; }
        public int equipmentId { get; set; }
        public int phaseId { get; set; }
        public int phaseReadId { get; set; }
        public DateTime outageDate { get; set; }
        public string fileName { get; set; }
        public string remarks { get; set; }
        public string path { get; set; }
        public int createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int isDeleted { get; set; }
    }
}
