namespace ActionTrakingSystem.DTOs
{
    public class ClusterSaveDto
    {
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public string clusterCode { get; set; }
        public int? executiveDirectorId { get; set; }
        public string executiveDirectorTitle { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
        public ClusterVpDto[] executiveVps { get; set; }
    }
    public class ClusterDto
    {
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public string clusterCode { get; set; }
        public int? executiveDirectorId { get; set; }
        public string executiveDirectorTitle { get; set; }
        public string executiveVpName { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
    }
    public class ClusterVpDto
    {
        public int userId{ get; set;}
        public string userName { get; set; }
    }
    public class ClusterSaveUserDto
    {
        public int userId { get; set; }
        public ClusterSaveDto cluster { get; set; }
    }
    public class ClusterUserDto
    {
        public int userId { get; set; }
        public ClusterDto cluster { get; set; }
    }
}
