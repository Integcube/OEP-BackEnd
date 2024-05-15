using System;

namespace ActionTrakingSystem.DTOs
{
    public class UserRoleDto
    {
        public RoleDto role { get; set; }
        public int userId { get; set; }

    }

    public class UserRoleSaveDto
    {
        public URObj data { get; set; }
        public int userId { get; set; }

    }
    public class RoleDto
    {
        public string roleName { get; set; }
        public string roleDescription { get; set; }
        public int roleId { get; set; }

    }
    public class saveUserDto
    {
        public int userId { get; set; }
        public UFormSubmit data { get; set; }
    }
    public class UFormSubmit
    {
        public UUserDto users { get; set; }
        public URoleDto[] roles { get; set; }
        public USiteDto[] sites { get; set; }
        public UTechnologyDto[] technologys { get; set; }
    }
    public class URoleDto
    {
        public int roleId { get; set; }
        public string roleTitle { get; set; }
    }

    public class USiteDto
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        public bool selected { get; set; }
    }

    public class UTechnologyDto
    {
        public int techId { get; set; }
        public string techTitle { get; set; }
        public bool selected { get; set; }
    }

    public class UUserDto
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
    public class URObj
    {
        public UMenu[] menus { get; set; }
        public URole userRole { get; set; }
    }
    public class UMenu
    {
        public int menuId { get; set; }
        public string title { get; set; }
        public int parentId { get; set; }
        public bool selected { get; set; }

    }
    public class URole
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }
    }

}
