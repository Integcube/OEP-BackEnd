using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    public class SitesInsuranceReport
    {
        [Key]
        public int sirId { get; set; }
        public int siteId { get; set; }
        public string reportPath { get; set; }
        public string reportName { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
    }
}
