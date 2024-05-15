using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_IProgram
    {
        [Key]
        public int programId { get; set; }
        public string programTitle { get; set; }
        public int createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int isDeleted { get; set; }
    }
}
