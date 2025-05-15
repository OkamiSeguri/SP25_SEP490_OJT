using BusinessObject;
using DataAccess;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<List<User>> GetUserAll()
        {
            return await UserDAO.Instance.GetUserAll();
        }

        public async Task<User> GetUserById(int id)
        {
            return await UserDAO.Instance.GetUserById(id);
        }

        public async Task<User> GetUserByMSSV(string mssv)
        {
            return await UserDAO.Instance.GetUserByMSSV(mssv);
        }

        public async Task<List<User>> GetUserByMSSVList(List<string> mssvList)
        {
            return await UserDAO.Instance.GetUserByMSSVList(mssvList);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await UserDAO.Instance.GetUserByEmail(email);
        }

        public async Task<IEnumerable<User>> GetUserByRole(int type)
        {
            return await UserDAO.Instance.GetUserByRole(type);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            return await UserDAO.Instance.ValidateUser(email, password);
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
            //soft delete
            await UserDAO.Instance.SoftDelete(id);
        }

        public async Task<User> GetCustomerByEmailAndPassword(string email, string password)
        {
            return await UserDAO.Instance.GetUserByEmailAndPassword(email, password);
        }

        public async Task<(List<string> DuplicateMSSVs, List<string> DuplicateEmails)> ImportUsersAsync(IEnumerable<User> users)
        {
            return await UserDAO.Instance.ImportUsersAsync(users);
        }
    }
}
