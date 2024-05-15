using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_IndicatorWeightage
    {
        [Key]
        public int weightageId { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int siteInfoId { get; set; }
        public int siteId { get; set; }
        public decimal value { get; set; }
        public string annual { get; set; }
        public string comment { get; set; }
    }
}
