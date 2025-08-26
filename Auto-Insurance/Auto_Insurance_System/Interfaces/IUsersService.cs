using Auto_Insurance_System.Models;

namespace Auto_Insurance_System.Interfaces
{
    public interface IUsersService
    {
        bool RegisterUser(Users user);
        Users Login(string username, string password);
        Users GetUserProfile(int userId);
        bool Logout(int userId);
        List<Users> GetAllUsers();
        bool ChangePassword(Users userLogin);
        bool VerifyUserLogin(Users userLogin);

    }
}
