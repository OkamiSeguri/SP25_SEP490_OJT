using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace Repositories
{
    public interface IOJTRegistrationRepository
    {
        Task<IEnumerable<OJTRegistration>> GetOJTRegistrationAll();
        Task<OJTRegistration> GetOJTRegistrationById(int id);
        Task Create(OJTRegistration ojtRegistration);
        Task Update(OJTRegistration ojtRegistration);
        Task Delete(int id);
    }
}
