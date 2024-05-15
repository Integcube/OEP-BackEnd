using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class WH_ContractOutages
    {
        [Key]
        public int contractOutageId { get; set; }
        public int equipmentId { get; set; }
        public int outageId { get; set; }
        public DateTime nextOutageDate { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }

    public class WH_ContractOutageFilterData
    {
        public int userId { get; set; }
        public WH_ContractOutageFilter filter { get; set; }
    }
    public class WH_ContractOutageFilter
    {
        public int regionId { get; set; }
        public int siteId { get; set; }
        public int equipmentId { get; set; }
        public int outageId { get; set; }
    }
}
