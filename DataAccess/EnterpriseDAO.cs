using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EnterpriseDAO : SingletonBase<EnterpriseDAO>
    {
        public async Task<IEnumerable<Enterprise>> GetEnterpriseAll()
        {
            return await _context.Enterprises.ToListAsync();
        }
        public async Task<Enterprise> GetEnterpriseById(int id)
        {
            var enterprise = await _context.Enterprises.FirstOrDefaultAsync(c => c.EnterpriseId == id);
            if (enterprise == null) return null; return enterprise;
        }
        public async Task Create(Enterprise enterprise)
        {
            await _context.Enterprises.AddAsync(enterprise);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Enterprise enterprise)
        {
            var existingItem = await GetEnterpriseById(enterprise.EnterpriseId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(enterprise);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var enterprise = await GetEnterpriseById(id);
            if (enterprise != null)
            {
                _context.Enterprises.Remove(enterprise);
                await _context.SaveChangesAsync();
            }
        }
    }
}
