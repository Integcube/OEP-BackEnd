namespace ActionTrakingSystem.DTOs
{
    public class OT_ActionRoleDto
    {
       
        public int actionOwnerId { get; set; }
        public string actionOwnerTitle { get; set; }
    }
    public class OT_ActionRoleUserDto
    {

        public int userId { get; set; }
        public OT_ActionRoleDto actionRole { get; set; }
    }

    public class OT_GetSelectedUsers
    {

        public int userId { get; set; }
        public int actionRoleId { get; set; }
    }
    public class OT_SaveActionUsers
    {

        public int userId { get; set; }
        public string userName { get; set; }
    }
    public class OT_SaveActionUsersRoles
    {

        public OT_SaveActionUsers[] users { get; set; }
        public OT_ActionRoleDto roles { get; set; }
    }
    public class OT_SaveActionRoles
    {

        public int userId { get; set; }
        public OT_SaveActionUsersRoles actionRole { get; set; }
    }
}
