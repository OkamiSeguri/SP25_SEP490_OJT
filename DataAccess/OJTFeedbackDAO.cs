using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OJTFeedbackDAO : SingletonBase<OJTFeedbackDAO>
    {
        public async Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll()
        {
            return await _context.OJTFeedbacks.ToListAsync();
        }
        public async Task<OJTFeedback> GetOJTFeedbackById(int id)
        {
            var ojtFeedback = await _context.OJTFeedbacks.FirstOrDefaultAsync(c => c.FeedbackId == id);
            if (ojtFeedback == null) return null; return ojtFeedback;
        }
        public async Task Create(OJTFeedback ojtFeedback)
        {
            await _context.OJTFeedbacks.AddAsync(ojtFeedback);
            await _context.SaveChangesAsync();
        }
        public async Task Update(OJTFeedback ojtFeedback)
        {
            var existingItem = await GetOJTFeedbackById(ojtFeedback.FeedbackId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(ojtFeedback);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var ojtFeedback = await GetOJTFeedbackById(id);
            if (ojtFeedback != null)
            {
                _context.OJTFeedbacks.Remove(ojtFeedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
