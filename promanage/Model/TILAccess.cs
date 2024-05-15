using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILAccess
    {
        [Key]
        public int tilAccessId { get; set; }
        public int packageId { get; set; }
        public int equipId { get; set; }
        public int userId { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
    }
}
