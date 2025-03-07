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
        public async Task<CohortCurriculum> GetCohortCurriculum(string cohort, int curriculumId)
        {
            var cohorts = await _context.CohortCurriculums.FirstOrDefaultAsync(c => c.Cohort == cohort && c.CurriculumId == curriculumId);
            if (cohorts == null) return null; return cohorts;
        }
        public async Task Create(CohortCurriculum cohort)
        {
            await _context.CohortCurriculums.AddAsync(cohort);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CohortCurriculum cohort)
        {
            var existingItem = await GetCohortCurriculum(cohort.Cohort,cohort.CurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(cohort);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(string cohort, int curriculumId)
        {
            var cohorts = await GetCohortCurriculum(cohort,curriculumId);
            if (cohorts != null)
            {
                _context.CohortCurriculums.Remove(cohorts);
                await _context.SaveChangesAsync();
            }
        }
        public int GetExpectedSemester(string cohort,int curriculumId)
        {
            var cohortCurriculum = _context.CohortCurriculums.FirstOrDefault(c => c.Cohort == cohort);
            return cohortCurriculum?.Semester ?? 6;
        }

    }
}