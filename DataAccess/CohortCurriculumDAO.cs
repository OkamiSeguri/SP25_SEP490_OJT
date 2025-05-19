using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class CohortCurriculumDAO : SingletonBase<CohortCurriculumDAO>
    {
        public async Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll()
        {
            return await _context.CohortCurriculums.AsNoTracking().ToListAsync();
        }

        public async Task<CohortCurriculum> GetCohortCurriculum(string cohort, int curriculumId)
        {
            return await _context.CohortCurriculums
                .AsNoTracking().FirstOrDefaultAsync(c => c.Cohort == cohort && c.CurriculumId == curriculumId);
        }
        public async Task<List<CohortCurriculum>> GetCohortCurriculumByCohort(List<string> cohort)
        {
            return await _context.CohortCurriculums
                         .Where(c => cohort.Contains(c.Cohort))
                         .AsNoTracking()
                         .ToListAsync();
        }


        public async Task Create(CohortCurriculum cohort)
        {
            await _context.CohortCurriculums.AddAsync(cohort);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CohortCurriculum cohort)
        {
            var existingItem = await GetCohortCurriculum(cohort.Cohort, cohort.CurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(cohort);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(string cohort, int curriculumId)
        {
            var existingItem = await GetCohortCurriculum(cohort, curriculumId);
            if (existingItem != null)
            {
                _context.CohortCurriculums.Remove(existingItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ImportCohortCurriculumAsync(IEnumerable<CohortCurriculum> cohorts)
        {
            foreach (var cohort in cohorts)
            {
                var existingItem = await GetCohortCurriculum(cohort.Cohort, cohort.CurriculumId);
                if (existingItem != null)
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(cohort);
                }
                else
                {
                    await _context.CohortCurriculums.AddAsync(cohort);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
