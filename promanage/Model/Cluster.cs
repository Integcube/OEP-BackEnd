using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    public class Cluster
    {
        [Key]
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public int regionId { get; set; }
        public string clusterCode { get; set; }
        public int? executiveDirector { get; set; }
        public int createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int isDeleted { get; set; }
    }
}
