using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ClusterExecutiveVp
    {
        [Key]
        public int executiveVpId { get; set; }
        public int clusterId { get; set; }
        public int userId { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }

    }
}
