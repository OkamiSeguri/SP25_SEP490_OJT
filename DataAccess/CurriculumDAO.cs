using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CurriculumDAO : SingletonBase<CurriculumDAO>
    {
        public async Task<IEnumerable<Curriculum>> GetCurriculumAll()
        {
            return await _context.Curriculums.ToListAsync();
        }
        public async Task<Curriculum> GetCurriculumById(int id)
        {
            var curriculum = await _context.Curriculums.FirstOrDefaultAsync(c => c.CurriculumId == id);
            if (curriculum == null) return null; return curriculum;
        }
        public async Task Create(Curriculum curriculum)
        {
            await _context.Curriculums.AddAsync(curriculum);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Curriculum curriculum)
        {
            var existingItem = await GetCurriculumById(curriculum.CurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(curriculum);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var curriculum = await GetCurriculumById(id);
            if (curriculum != null)
            {
                _context.Curriculums.Remove(curriculum);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ImportCurriculumsAsync(IEnumerable<Curriculum> cohorts)
        {
            foreach (var cohort in cohorts)
            {
                var existingCurriculum = await _context.Curriculums.FindAsync(cohort.CurriculumId);
                if (existingCurriculum != null)
                {
                    _context.Entry(existingCurriculum).State = EntityState.Detached; 
                }
            }

            await _context.Curriculums.AddRangeAsync(cohorts);
            await _context.SaveChangesAsync();
        }

    }
}
