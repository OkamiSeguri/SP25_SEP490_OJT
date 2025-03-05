using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;

namespace FOMSService
{
    public interface IOJTResultService
    {
        Task<IEnumerable<OJTResultDTO>> GetOJTResultAll();
        Task<OJTResultDTO> GetOJTResultById(int id);
        Task Create(OJTResultDTO ojtResultDTO);
        Task Update(OJTResultDTO ojtResultDTO);
        Task Delete(int id);
    }
}
