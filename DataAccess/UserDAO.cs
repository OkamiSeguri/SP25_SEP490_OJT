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
        public async Task<IEnumerable<User>> GetUserAll()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserId == id);
            if (user == null) return null; return user;
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

    }
}
