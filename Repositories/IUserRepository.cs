using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUserAll();
        Task<IEnumerable<User>> GetUserByRole(int type);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string Email);
        Task<User> ValidateUser(string Email, string Password);
        Task Create(User user);
        Task Update(User user);
        Task Delete(int id);
        Task<User> GetCustomerByEmailAndPassword(string email, string password);
    

    }
}
