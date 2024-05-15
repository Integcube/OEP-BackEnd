using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    public class TILActiontrackerFile
    {
        [Key]
        public int tapfId { get; set; }
        public int tapId { get; set; }
        public int equipId { get; set; }
        public string remarks { get; set; }
        public string reportPath { get; set; }
        public string reportName { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
    }
}
