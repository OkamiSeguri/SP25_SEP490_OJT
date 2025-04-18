﻿using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OJTProgramRepository : IOJTProgramRepository
    {
        public async Task<IEnumerable<OJTProgram>> GetOJTProgramAll()
        {
            return await OJTProgramDAO.Instance.GetOJTProgramAll();
        }
        public async Task<OJTProgram> GetOJTProgramById(int id)
        {
            return await OJTProgramDAO.Instance.GetOJTProgramById(id);
        }
        public async Task<IEnumerable<OJTProgram>> ApproveRequest(int id)
        {
            return (IEnumerable<OJTProgram>)await OJTProgramDAO.Instance.ApproveRequest(id);
        }
        public async Task<IEnumerable<OJTProgram>> RejectRequest(int id)
        {
            return (IEnumerable<OJTProgram>)await OJTProgramDAO.Instance.RejectRequest(id);
        }
        public async Task<IEnumerable<OJTProgram>> ListApproved()
        {
            return await OJTProgramDAO.Instance.ListApproved();
        }
        public async Task<IEnumerable<OJTProgram>> ListRejected()
        {
            return await OJTProgramDAO.Instance.ListRejected();
        }
        public async Task<IEnumerable<OJTProgram>> ListPending()
        {
            return await OJTProgramDAO.Instance.ListPending();
        }
        public async Task Create(OJTProgram ojtProgram)
        {
            await OJTProgramDAO.Instance.Create(ojtProgram);
        }
        public async Task Update(OJTProgram ojtProgram)
        {
            await OJTProgramDAO.Instance.Update(ojtProgram);
        }
        public async Task Delete(int id)
        {
            await OJTProgramDAO.Instance.Delete(id);
        }
    }
}
