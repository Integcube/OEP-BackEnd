using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class SitesTilReport
    {
        [Key]
        public int strId { get; set; }
        public int siteId { get; set; }
        public string reportPath { get; set; }
        public string reportName { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
    }
}
