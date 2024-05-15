using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_ProgramTechnologies
    {
        [Key]
        public int ptId { get; set; }
        public int programId { get; set; }
        public int technologyId { get; set; }
        public int isDeleted { get; set; }
    }
}
