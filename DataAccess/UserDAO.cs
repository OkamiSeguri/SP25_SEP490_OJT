using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class UserDAO : SingletonBase<UserDAO>
    {
        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password && !c.IsDeleted);
        }

        public async Task<List<User>> GetUserAll()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.UserId == id && !c.IsDeleted);
        }

        public async Task<User> GetUserByMSSV(string mssv)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.MSSV == mssv && !c.IsDeleted);
        }

        public async Task<List<User>> GetUserByMSSVList(List<string> mssvList)
        {
            return await _context.Users
                .Where(u => mssvList.Contains(u.MSSV) && !u.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUserByRole(int type)
        {
            return await _context.Users
                .Where(a => a.Role == type && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password && !c.IsDeleted);
        }

        public async Task Create(User user)
        {
            user.IsDeleted = false;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            var existingItem = await GetUserById(user.UserId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDelete(int id)
        {
            var user = await _context.Users
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);

            if (user != null)
            {
                user.IsDeleted = true;

                if (user.StudentProfile != null)
                {
                    user.StudentProfile.IsDeleted = true;
                    _context.StudentProfiles.Update(user.StudentProfile);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(List<string> DuplicateMSSVs, List<string> DuplicateEmails)> ImportUsersAsync(IEnumerable<User> users)
        {
            var existingUsers = await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            var existingMSSVs = existingUsers.Select(u => u.MSSV).ToHashSet();
            var existingEmails = existingUsers.Select(u => u.Email).ToHashSet();

            List<string> duplicateMSSVs = new List<string>();
            List<string> duplicateEmails = new List<string>();
            List<User> usersToAdd = new List<User>();

            foreach (var user in users)
            {
                if (existingMSSVs.Contains(user.MSSV))
                {
                    duplicateMSSVs.Add(user.MSSV);
                }
                else if (existingEmails.Contains(user.Email))
                {
                    duplicateEmails.Add(user.Email);
                }
                else
                {
                    user.Role = 0;
                    user.IsDeleted = false;
                    usersToAdd.Add(user);
                }
            }

            if (usersToAdd.Count > 0)
            {
                await _context.Users.AddRangeAsync(usersToAdd);
                await _context.SaveChangesAsync();
            }

            return (duplicateMSSVs, duplicateEmails);
        }
    }
}
