using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecNomacStatus
    {
        [Key]
        public int nomacStatusId { get; set; }
        public string nomacStatusTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
