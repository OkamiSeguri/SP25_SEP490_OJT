using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;

namespace FOMSService
{
    public interface IOJTProgramService
    {
        Task<IEnumerable<OJTProgramDTO>> GetOJTProgramAll();
        Task<OJTProgramDTO> GetOJTProgramById(int id);
        Task Create(OJTProgramDTO oJTProgramDTO);
        Task Update(OJTProgramDTO oJTProgramDTO);
        Task Delete(int id);
    }
}
