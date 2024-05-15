using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_FormulaType
    {
        [Key]
        public int formulaTypeId { get; set; }
        public string formulaTitle { get; set; }
        public string formulaCode { get; set; }
        public int isDeleted { get; set; }
    }
}
