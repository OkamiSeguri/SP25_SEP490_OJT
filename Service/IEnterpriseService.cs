using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public interface IEnterpriseService
    {
        Task<IEnumerable<EnterpriseDTO>> GetEnterpriseAll();
        Task<EnterpriseDTO> GetEnterpriseById(int id);
        Task Create(EnterpriseDTO enterpriseDTO);
        Task Update(EnterpriseDTO enterpriseDTO);
        Task Delete(int id);
    }
}
