namespace ActionTrakingSystem.DTOs
{
    public class KPIConfiguratorDto
    {
        public int groupId { get; set; }
        public string groupTitle { get; set; }
        public string groupCode { get; set; }
        public decimal groupWeight { get; set; }
        public int indicatorId { get; set; }
        public string indicatorCode { get; set; }
        public string indicatorTitle { get; set; }
        public int siteId { get; set; }
        public int infoId { get; set; }
        public decimal infoWeight { get; set; }
        public string measurementTitle { get; set; }
        public string annualTargetTitle { get; set; }
        public decimal factor { get; set; }
        public string unit { get; set; }
        public string classificationTitle { get; set; }
        public int formulaType { get; set; }
    }
    public class GetKPIConfigListDto
    {
        public int siteId { get; set; }
        public int userId { get; set; }
    }
    public class GetKPIIndicatorDto
    {
        public int groupId { get; set; }
        public int userId { get; set; }
    }
    public class SaveKPIIndicatorDto
    {

            public int groupId { get; set; }
            public int indicatorId { get; set; }
            public string indicatorTitle { get; set; }
            public string indicatorCode { get; set; }
            public bool? isDisplay { get; set; }
            public bool? isParent { get; set; }
    }
}
