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
        public async Task<CohortCurriculum> GetCohortCurriculum(int Id)
        {
            var cohorts = await _context.CohortCurriculums.FirstOrDefaultAsync(c => c.CohortCurriculumId == Id);
            if (cohorts == null) return null; return cohorts;
        }
        public async Task Create(CohortCurriculum cohort)
        {
            await _context.CohortCurriculums.AddAsync(cohort);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CohortCurriculum cohort)
        {
            var existingItem = await GetCohortCurriculum(cohort.CohortCurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(cohort);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var cohorts = await GetCohortCurriculum(id);
            if (cohorts != null)
            {
                _context.CohortCurriculums.Remove(cohorts);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ImportCohortCurriculumAsync(IEnumerable<CohortCurriculum> cohorts)
        {
            foreach (var cohort in cohorts)
            {
                var existingCohortCurriculum = await _context.CohortCurriculums.FindAsync(cohort.CohortCurriculumId);

                if (existingCohortCurriculum != null)
                {
                    _context.Entry(existingCohortCurriculum).CurrentValues.SetValues(cohort);
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