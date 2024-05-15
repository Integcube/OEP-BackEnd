using ActionTrakingSystem.Model;

namespace ActionTrakingSystem.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
