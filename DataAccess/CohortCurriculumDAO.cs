using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CohortCurriculumDAO : SingletonBase<CohortCurriculumDAO>
    {
        public async Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll()
        {
            return await _context.CohortCurriculums.ToListAsync();
        }
        public async Task<CohortCurriculum> GetCohortCurriculumByCohort(string cohort)
        {
            var cohorts = await _context.CohortCurriculums.FirstOrDefaultAsync(c => c.Cohort == cohort);
            if (cohorts == null) return null; return cohorts;
        }
        public async Task Create(CohortCurriculum cohort)
        {
            await _context.CohortCurriculums.AddAsync(cohort);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CohortCurriculum cohort)
        {
            var existingItem = await GetCohortCurriculumByCohort(cohort.Cohort);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(cohort);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(string cohort)
        {
            var cohorts = await GetCohortCurriculumByCohort(cohort);
            if (cohorts != null)
            {
                _context.CohortCurriculums.Remove(cohorts);
                await _context.SaveChangesAsync();
            }
        }
    }
}