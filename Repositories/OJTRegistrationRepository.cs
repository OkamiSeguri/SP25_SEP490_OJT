using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
