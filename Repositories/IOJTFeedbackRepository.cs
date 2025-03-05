using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace Repositories
{
    public interface IOJTFeedbackRepository
    {
        Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll();
        Task<OJTFeedback> GetOJTFeedbackById(int id);
        Task Create(OJTFeedback ojtFeedback);
        Task Update(OJTFeedback ojtFeedback);
        Task Delete(int id);
    }
}
