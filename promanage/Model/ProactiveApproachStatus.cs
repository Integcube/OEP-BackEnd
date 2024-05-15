using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveApproachStatus
    {
        [Key]
        public int approachStatusId { get; set; }
        public string approachStatusTitle { get; set; }

        public int isDeleted { get; set; }

    }
}
