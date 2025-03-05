using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;

namespace FOMSService
{
    public interface IOJTFeedbackService
    {
        Task<IEnumerable<OJTFeedbackDTO>> GetOJTFeedbackAll();
        Task<OJTFeedbackDTO> GetOJTFeedbackById(int id);
        Task Create(OJTFeedbackDTO oJTFeedbackDTO);
        Task Update(OJTFeedbackDTO oJTFeedbackDTO);
        Task Delete(int id);
    }
}
