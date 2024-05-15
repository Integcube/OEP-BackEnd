namespace ActionTrakingSystem.DTOs
{
    public class CountryDto
    {
        public int countryId { get; set; }
        public int regionId { get; set; }
        public int clustedId { get; set; }
        public string regionTitle { get; set; }
        public string countryTitle { get; set; }
        public string countryCode { get; set; }
        public int? executiveDirectorId { get; set; }
        public string executiveDirectorTitle { get; set; }
    }
    public class deleteCountryDto
    {
        public int userId { get; set; }
        public CountryDto country { get; set; }
    }
    public class SelectedCountryDto
    {
        public int userId { get; set; }
        public int countryId { get; set; }
    }
    public class SaveCountryDto
    {
        public int userId { get; set; }
        public SaveCountryUserListDto country { get; set; }
    }
    public class SaveCountryUserListDto
    {
        public CountryDto country { get; set; }
        public SaveUserListDto[] executiveVp { get; set; }
    }

    public class SaveUserListDto
    {
        public int userId { get; set; }
        public string userName { get; set; }
    }
}
