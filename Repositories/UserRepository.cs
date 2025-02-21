using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using DataAccess;
namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<IEnumerable<User>> GetUserAll()
        {
            return await UserDAO.Instance.GetUserAll();
        }

        public async Task<User> GetUserById(int id)
        {
            return await UserDAO.Instance.GetUserById(id);
        }
        public async Task<User> GetUserByEmail(string Email)
        {
            return await UserDAO.Instance.GetUserByEmail(Email);
        }
        public async Task<IEnumerable<User>> GetUserByRole(int type)
        {
            return await UserDAO.Instance.GetUserByRole(type);
        }
        public async Task<User> ValidateUser(string Email, string Password)
        {
            return await UserDAO.Instance.ValidateUser(Email, Password);
        }

        public async Task Create(User user)
        {
            await UserDAO.Instance.Create(user);
        }

        public async Task Update(User user)
        {
            await UserDAO.Instance.Update(user);
        }

        public async Task Delete(int id)
        {
            await UserDAO.Instance.Delete(id);
        }


        public async Task<User> GetCustomerByEmailAndPassword(string email, string password)
        {
            return await UserDAO.Instance.GetUserByEmailAndPassword(email, password);
        }
    }
}
