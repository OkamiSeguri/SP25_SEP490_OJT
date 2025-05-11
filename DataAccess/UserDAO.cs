using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserDAO: SingletonBase<UserDAO>
    {
        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            var User = await _context.Users
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
            return User;
        }
        public async Task<List<User>> GetUserAll()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserId == id);
            if (user == null) return null; return user;
        }       
        public async Task<User> GetUserByMSSV(string mssv)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.MSSV == mssv);
            if (user == null) return null; return user;
        }
        public async Task<List<User>> GetUserByMSSVList(List<string> mssvList)
        {
            return await _context.Users
                .Where(u => mssvList.Contains(u.MSSV))
                .AsNoTracking() 
                .ToListAsync();
        }
        public async Task<User> GetUserByEmail(string Email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == Email);
            if (user == null) return null; return user;
        }
        public async Task<IEnumerable<User>> GetUserByRole(int type)
        {
            return await _context.Users.Where(a => a.Role == type).ToListAsync();
        }
        public async Task<User> ValidateUser(string Email, string Password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == Email && c.Password == Password);
            if (user == null) return null; return user;
        }
        public async Task Create(User user)
        {
            await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync();
        }
        public async Task Update(User user)
        {
            var existingItem = await GetUserById(user.UserId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(user);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var user = await GetUserById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<(List<string> DuplicateMSSVs, List<string> DuplicateEmails)> ImportUsersAsync(IEnumerable<User> users)
        {
            var existingUsers = await _context.Users.ToListAsync();

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
                    // Thiết lập Role mặc định là 0
                    user.Role = 0;
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
