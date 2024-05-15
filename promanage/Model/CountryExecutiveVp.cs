using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CountryExecutiveVp
    {
        [Key]
        public int executiveVpId { get; set; }
        public int countryId { get; set; }
        public int userId { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
    }
}
