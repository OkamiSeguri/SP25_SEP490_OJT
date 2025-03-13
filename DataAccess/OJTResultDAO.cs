using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OJTResultDAO : SingletonBase<OJTResultDAO>
    {
        public async Task<IEnumerable<OJTResult>> GetOJTResultAll()
        {
            return await _context.OJTResults.ToListAsync();
        }
        public async Task<OJTResult> GetOJTResultById(int id)
        {
            var ojtResult = await _context.OJTResults.FirstOrDefaultAsync(c => c.OJTId == id);
            if (ojtResult == null) return null; return ojtResult;
        }
        public async Task Create(OJTResult ojtResult)
        {
            await _context.OJTResults.AddAsync(ojtResult);
            await _context.SaveChangesAsync();
        }
        public async Task Update(OJTResult ojtResult)
        {
            var existingItem = await GetOJTResultById(ojtResult.OJTId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(ojtResult);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var ojtResult = await GetOJTResultById(id);
            if (ojtResult != null)
            {
                _context.OJTResults.Remove(ojtResult);
                await _context.SaveChangesAsync();
            }
        }
    }
}
