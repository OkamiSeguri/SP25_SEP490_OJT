using BusinessObject;
using DataAccess;

namespace Repositories
{
    public class OJTRegistrationRepository : IOJTRegistrationRepository
    {
        public async Task<IEnumerable<OJTRegistration>> GetOJTRegistrationAll()
        {
            return await OJTRegistrationDAO.Instance.GetOJTRegistrationAll();
        }

        public async Task<OJTRegistration> GetOJTRegistrationById(int id)
        {
            return await OJTRegistrationDAO.Instance.GetOJTRegistrationById(id);
        }

        public async Task<IEnumerable<OJTRegistration>> GetOJTRegistrationByStatus(string status)
        {
            return await OJTRegistrationDAO.Instance.GetOJTRegistrationByStatus(status);
        }

        public async Task<IEnumerable<OJTRegistration>> ListApproved()
        {
            return await OJTRegistrationDAO.Instance.ListApproved();
        }

        public async Task<IEnumerable<OJTRegistration>> ListRejected()
        {
            return await OJTRegistrationDAO.Instance.ListRejected();
        }

        public async Task<IEnumerable<OJTRegistration>> ListPending()
        {
            return await OJTRegistrationDAO.Instance.ListPending();
        }

        public async Task<IEnumerable<OJTRegistration>> GetByEnterpriseId(int enterpriseId)
        {
            return await OJTRegistrationDAO.Instance.GetByEnterpriseId(enterpriseId);
        }

        public async Task<IEnumerable<OJTRegistration>> GetByStudentId(int studentId)
        {
            return await OJTRegistrationDAO.Instance.GetByStudentId(studentId);
        }

        public async Task<IEnumerable<OJTRegistration>> GetByProgramId(int programId)
        {
            return await OJTRegistrationDAO.Instance.GetByProgramId(programId);
        }

        public async Task<int> CountByEnterpriseId(int enterpriseId)
        {
            return await OJTRegistrationDAO.Instance.CountByEnterpriseId(enterpriseId);
        }

        public async Task<int> CountByProgramId(int programId)
        {
            return await OJTRegistrationDAO.Instance.CountByProgramId(programId);
        }

        public async Task<bool> ChangeStatus(int ojtId, string newStatus)
        {
            return await OJTRegistrationDAO.Instance.ChangeStatus(ojtId, newStatus);
        }

        public async Task<string> GetCurrentStatusByStudentId(int studentId)
        {
            return await OJTRegistrationDAO.Instance.GetCurrentStatusByStudentId(studentId);
        }

        public async Task Create(OJTRegistration ojtRegistration)
        {
            await OJTRegistrationDAO.Instance.Create(ojtRegistration);
        }

        public async Task Update(OJTRegistration ojtRegistration)
        {
            await OJTRegistrationDAO.Instance.Update(ojtRegistration);
        }

        public async Task Delete(int id)
        {
            await OJTRegistrationDAO.Instance.Delete(id);
        }
    }
}
