namespace ActionTrakingSystem.DTOs
{
    public class tatFileListDto
    {
        public int equipId { get; set; }
        public int tapId { get; set; }
        public int userId { get; set; }



    }
    public class ActionPackageDto
    {
        public int packageId { get; set; }
        public int? tilId { get; set; }
        public string tilNumber { get; set; }
        public string actionTitle { get; set; }
        public int? actionClosureGuidelinesId { get; set; }
        public string actionCategory { get; set; }
        public int? outageId { get; set; }
        public string unitStatus { get; set; }
        public string actionDescription { get; set; }
        public string expectedBudget { get; set; }
        public int? budgetSourceId { get; set; }
        public string budegetSource { get; set; }
        public string patComments { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public int? reviewStatusId { get; set; }
        public string reviewStatus { get; set; }
        public string recurrence { get; set; }
        //public int siteId { get; set; }
        //public string siteTitle { get; set; }
        //public int regionId { get; set; }
        //public string regionTitle { get; set; }
        
    }
    public class userActionPackageDto
    {
        public string email { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
    }
    public class assignTilUser
    {
        public int userId { get; set; }
        public AssignTilDto data { get; set; }
    }
    public class copyActionDto
    {
        public int userId { get; set; }
        public ActionPackageDto data { get; set; }
    }
    public class AssignTilDto
    {
        public ActionPackageDto action { get; set; }
  
        public ITAPEquipment[] equipment { get; set; }

    }
    public class ITAPEquipment
    {
        public int equipmentId { get; set; }

        public string unit { get; set; }
    }
    public class TAPFilterUserDto
    {
        public int userId { get; set; }
        public string tilList { get; set; }
        public string outageList { get; set; }
        public string budgetList { get; set; }
        public string reviewList { get; set; }
        public string priority { get; set; }
        //public AssignTilFilterDto filter { get; set; }
    }
    public class AssignTilFilterDto
    {
        public int budgetSource { get; set; }
        public int tilNumber { get; set; }
        public int unitStatus { get; set; }
        public int reviewStatus { get; set; }
        public int priority { get; set; }
    }
}
