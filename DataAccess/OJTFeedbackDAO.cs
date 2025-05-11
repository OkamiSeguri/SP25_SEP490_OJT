using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class OJTFeedbackDAO : SingletonBase<OJTFeedbackDAO>
    {
        public async Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll()
        {
            return await _context.OJTFeedbacks.ToListAsync();
        }
        // Lấy feedback theo FeedbackId
        public async Task<OJTFeedback> GetOJTFeedbackById(int id)
        {
            return await _context.OJTFeedbacks.FirstOrDefaultAsync(f => f.FeedbackId == id);
        }
        //Lấy feedback theo OJTId
        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByOJTId(int ojtId)
        {
            return await _context.OJTFeedbacks
                .Where(f => f.OJTId == ojtId)
                .ToListAsync();
        }
        // Xem tất cả feedback của một chương trình OJT
        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByProgramId(int programId)
        {
            return await _context.OJTFeedbacks
                .Where(f => f.ProgramId == programId)
                .ToListAsync();
        }

        //Lấy feedback theo EnterpriseId
        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByEnterpriseId(int enterpriseId)
        {
            return await _context.OJTFeedbacks
                .Where(f => f.EnterpriseId == enterpriseId)
                .ToListAsync();
        }

        //Kiểm tra feedback đã tồn tại chưa (1 sinh viên 1 chương trình)
        public async Task<bool> FeedbackExists(int ojtId, int programId)
        {
            return await _context.OJTFeedbacks
                .AnyAsync(f => f.OJTId == ojtId && f.ProgramId == programId);
        }

        //Lấy tất cả feedback của một chương trình OJT
        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByOJTProgramId(int programId)
        {
            return await _context.OJTFeedbacks
                .Where(f => f.ProgramId == programId)
                .ToListAsync();
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
                await _context.SaveChangesAsync();
            }
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
