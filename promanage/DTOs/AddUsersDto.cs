namespace ActionTrakingSystem.DTOs
{
    public class AddUsersDto
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }

    }
    public class UpdateUsersDto
    {
        public AddUsersDto info { get; set; }
        public int userId { get; set; }
    }
}
