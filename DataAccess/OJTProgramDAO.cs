using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class OJTProgramDAO : SingletonBase<OJTProgramDAO>
    {
        public async Task<IEnumerable<OJTProgram>> GetOJTProgramAll()
        {
            return await _context.OJTPrograms.ToListAsync();
        }
        public async Task<OJTProgram> GetOJTProgramById(int id)
        {
            var ojtProgram = await _context.OJTPrograms.FirstOrDefaultAsync(c => c.ProgramId == id);
            if (ojtProgram == null) return null; return ojtProgram;
        }
        public async Task<OJTProgram> ApproveRequest(int id)
        {
            var ojtProgram = await _context.OJTPrograms.FirstOrDefaultAsync(c => c.ProgramId == id);
            if (ojtProgram == null) return null;
            ojtProgram.Status = "Approved";
            await _context.SaveChangesAsync();
            return ojtProgram;
        }
        public async Task<OJTProgram> RejectRequest(int id)
        {
            var ojtProgram = await _context.OJTPrograms.FirstOrDefaultAsync(c => c.ProgramId == id);
            if (ojtProgram == null) return null;
            ojtProgram.Status = "Rejected";
            await _context.SaveChangesAsync();
            return ojtProgram;
        }
        public async Task<IEnumerable<OJTProgram>> ListApproved()
        {
            return await _context.OJTPrograms.Where(c => c.Status == "Approved").ToListAsync();
        }
        public async Task<IEnumerable<OJTProgram>> ListPending()
        {
            return await _context.OJTPrograms.Where(c => c.Status == "Pending").ToListAsync();
        }
        public async Task<IEnumerable<OJTProgram>> ListRejected()
        {
            return await _context.OJTPrograms.Where(c => c.Status == "Rejected").ToListAsync();
        }
        public async Task Create(OJTProgram ojtProgram)
        {
            await _context.OJTPrograms.AddAsync(ojtProgram);
            await _context.SaveChangesAsync();
        }
        public async Task Update(OJTProgram ojtProgram)
        {
            var existingItem = await GetOJTProgramById(ojtProgram.ProgramId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(ojtProgram);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var ojtProgram = await GetOJTProgramById(id);
            if (ojtProgram != null)
            {
                _context.OJTPrograms.Remove(ojtProgram);
                await _context.SaveChangesAsync();
            }
        }
    }
}
