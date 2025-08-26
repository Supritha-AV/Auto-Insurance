using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Auto_Insurance_System.Services
{
    public class UsersService : IUsersService
    {
        private readonly AutoInsuranceDbContext context;

        public UsersService(AutoInsuranceDbContext context)
        {
            this.context = context;
        }

        public bool RegisterUser(Users user)
        {
            try
            {
                DbSet<Users> users = context.Users;
                users.Add(user);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Users Login(string username, string password)
        {
            return context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public Users GetUserProfile(int userId)
        {
            return context.Users.Find(userId);
        }

        public List<Users> GetAllUsers()
        {
            return context.Users.ToList();
        }
        public bool Logout(int userId)
        {
            Users user =GetAllUsers().FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }
            context.Users.Remove(user);
            context.SaveChanges();
            return true;
        }



        public bool ChangePassword(Users userLogin)
        {
            Users user = GetAllUsers().FirstOrDefault(u => u.UserId == userLogin.UserId);
            if (user == null)
            {
                return false;
            }
            user.Password = userLogin.Password;
            context.SaveChanges();
            return true;
        }


        public bool VerifyUserLogin(Users userLogin)
        {
            Users user = context.Users.FirstOrDefault(u =>
                u.Username == userLogin.Username &&
                u.Password == userLogin.Password &&
                u.Role == userLogin.Role);

            return user != null;
        }

    }
}
