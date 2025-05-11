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
            return await _context.OJTRegistrations.FirstOrDefaultAsync(c => c.OJTId == id);
        }

        public async Task<IEnumerable<OJTRegistration>> GetOJTRegistrationByStatus(string status)
        {
            return await _context.OJTRegistrations
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTRegistration>> ListApproved()
        {
            return await _context.OJTRegistrations
                .Where(c => c.Status == "Approved")
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTRegistration>> ListRejected()
        {
            return await _context.OJTRegistrations
                .Where(c => c.Status == "Rejected")
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTRegistration>> ListPending()
        {
            return await _context.OJTRegistrations
                .Where(c => c.Status == "Pending")
                .ToListAsync();
        }
        public async Task<IEnumerable<OJTRegistration>> GetByStudentId(int studentId)
        {
            return await _context.OJTRegistrations
                .Where(c => c.StudentId == studentId)
                .ToListAsync();
        }
        public async Task<IEnumerable<OJTRegistration>> GetByProgramId(int programId)
        {
            return await _context.OJTRegistrations
                .Where(c => c.ProgramId == programId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTRegistration>> GetByEnterpriseId(int EnterpriseId)
        {
            return await _context.OJTRegistrations
                .Where(c => c.EnterpriseId == EnterpriseId)
                .ToListAsync();
        }

        public async Task<int> CountByEnterpriseId(int EnterpriseId)
        {
            return await _context.OJTRegistrations
                .CountAsync(c => c.EnterpriseId == EnterpriseId);
        }
        public async Task<int> CountByProgramId(int programId)
        {
            return await _context.OJTRegistrations
                .CountAsync(c => c.ProgramId == programId);
        }

        public async Task<bool> ChangeStatus(int ojtId, string newStatus)
        {
            var registration = await GetOJTRegistrationById(ojtId);
            if (registration == null) return false;

            registration.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetCurrentStatusByStudentId(int studentId)
        {
            var latest = await _context.OJTRegistrations
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.OJTId)
                .FirstOrDefaultAsync();

            return latest?.Status;
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
                await _context.SaveChangesAsync();
            }
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
