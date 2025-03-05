using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class OJTRegistrationDAO : SingletonBase<OJTRegistrationDAO>
    {
        public async Task<IEnumerable<OJTRegistration>> GetOJTRegistrationAll()
        {
            return await _context.OJTRegistrations.ToListAsync();
        }
        public async Task<OJTRegistration> GetOJTRegistrationById(int id)
        {
            var ojtRegistration = await _context.OJTRegistrations.FirstOrDefaultAsync(c => c.OJTId == id);
            if (ojtRegistration == null) return null; return ojtRegistration;
        }
        public async Task Create(OJTRegistration ojtRegistration)
        {
            await _context.OJTRegistrations.AddAsync(ojtRegistration);
            await _context.SaveChangesAsync();
        }
        public async Task Update(OJTRegistration ojtRegistration)
        {
            var existingItem = await GetOJTRegistrationById(ojtRegistration.OJTId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(ojtRegistration);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var ojtRegistration = await GetOJTRegistrationById(id);
            if (ojtRegistration != null)
            {
                _context.OJTRegistrations.Remove(ojtRegistration);
                await _context.SaveChangesAsync();
            }
        }
    }
}
