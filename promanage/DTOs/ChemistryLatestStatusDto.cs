namespace ActionTrakingSystem.DTOs
{
    public class ChemistryLatestStatusDto
    {
        public int updateId { get; set; }
        public int? siteId { get; set; }
        public string siteTitle { get; set; }
        public string remarks { get; set; }
        public string target { get; set; }
        public string liveApplication { get; set; }
        public int? modelId { get; set; }
        public string modelStatus { get; set; }
        public int? statusId { get; set; }
        public int? lusId { get; set; }
        public string status { get; set; }
        public decimal? score { get; set; }
    }
    public class chemistryLatestUser{
        public int userId { get; set; }
        public ChemistryLatestStatusDto latestStatus { get; set; }
    }
}
