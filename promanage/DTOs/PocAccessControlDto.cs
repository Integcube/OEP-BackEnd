
namespace ActionTrakingSystem.DTOs
{
    public class PocAccessControlDto
    {
        public int userId { get; set; }
        public int siteId { get; set; }
    }
    public class PocAccessControlSaveUserDto
    {
        public int userId { get; set; }
        public PocAccessControlSaveDto accessObj { get; set; }
    }
    public class PocAccessControlSaveDto
    {
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public CUserDto[] userList { get; set; }
    }
    public class CUserDto
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}
