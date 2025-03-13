using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OJTFeedbackRepository : IOJTFeedbackRepository
    {
        public async Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll()
        {
            return await OJTFeedbackDAO.Instance.GetOJTFeedbackAll();
        }
        public async Task<OJTFeedback> GetOJTFeedbackById(int id)
        {
            return await OJTFeedbackDAO.Instance.GetOJTFeedbackById(id);
        }
        public async Task Create(OJTFeedback ojtFeedback)
        {
            await OJTFeedbackDAO.Instance.Create(ojtFeedback);
        }
        public async Task Update(OJTFeedback ojtFeedback)
        {
            await OJTFeedbackDAO.Instance.Update(ojtFeedback);
        }
        public async Task Delete(int id)
        {
            await OJTFeedbackDAO.Instance.Delete(id);
        }
    }
}
