using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;

namespace FOMSService
{
    public interface IOJTRegistrationService 
    {
        Task<IEnumerable<OJTRegistrationDTO>> GetOJTRegistrationAll();
        Task<OJTRegistrationDTO> GetOJTRegistrationById(int id);
        Task Create(OJTRegistrationDTO oJTRegistrationDTO);
        Task Update(OJTRegistrationDTO oJTRegistrationDTO);
        Task Delete(int id);
    }
}
