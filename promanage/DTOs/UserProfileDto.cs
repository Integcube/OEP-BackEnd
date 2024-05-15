namespace ActionTrakingSystem.DTOs
{
    public class ExperienceForm
    {
        public int userId { get; set; }
        public string designation { get; set; }
        public string education { get; set; }
        public string experience { get; set; }
        public string workshops { get; set; }
    }
    public class SecurityForm
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string newPassword { get; set; }
    }
    public class AccountForm
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string uploadFile { get; set; }
        public string mobile { get; set; }
        public string dob { get; set; }
        public string about { get; set; }

    }
}
