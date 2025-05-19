using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class CurriculumDAO : SingletonBase<CurriculumDAO>
    {
        public async Task<IEnumerable<Curriculum>> GetCurriculumAll()
        {
            return await _context.Curriculums.AsNoTracking().ToListAsync();
        }
        public async Task<Curriculum> GetCurriculumById(int id)
        {
            var curriculum = await _context.Curriculums.AsNoTracking().FirstOrDefaultAsync(c => c.CurriculumId == id);
            if (curriculum == null) return null; return curriculum;
        }
        public async Task<Curriculum> GetCurriculumBySubjectCode(string sc)
        {
            var curriculum = await _context.Curriculums.AsNoTracking().FirstOrDefaultAsync(c => c.SubjectCode == sc);
            if (curriculum == null) return null; return curriculum;
        }
        public async Task<List<Curriculum>> GetCurriculumBySubjectCodeList(List<string> subjectCodes)
        {
            return await _context.Curriculums
                .Where(c => subjectCodes.Contains(c.SubjectCode))
                .AsNoTracking()
                .ToListAsync();
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
        public async Task<List<string>> ImportCurriculumsAsync(IEnumerable<Curriculum> curriculums)
        {
            var existingCurriculums = await _context.Curriculums.ToListAsync();
            var existingSubjectCodes = existingCurriculums.Select(c => c.SubjectCode).ToHashSet();
            var existingIds = existingCurriculums.Select(c => c.CurriculumId).ToHashSet();

            List<string> duplicateSubjectCodes = new List<string>();

            foreach (var curriculum in curriculums)
            {
                if (existingIds.Contains(curriculum.CurriculumId))
                {
                    var existingCurriculum = await _context.Curriculums.FindAsync(curriculum.CurriculumId);
                    _context.Entry(existingCurriculum).CurrentValues.SetValues(curriculum);
                }
                else
                {
                    if (existingSubjectCodes.Contains(curriculum.SubjectCode))
                    {
                        duplicateSubjectCodes.Add(curriculum.SubjectCode);
                    }
                    else
                    {
                        await _context.Curriculums.AddAsync(curriculum);
                    }
                }
            }

            if (duplicateSubjectCodes.Count > 0)
            {
                return duplicateSubjectCodes;
            }

            await _context.SaveChangesAsync();
            return new List<string>();
        }




        public async Task<List<int>> GetAllIds()
        {
            using var context = new ApplicationDbContext();
            return await context.Curriculums.Select(c => c.CurriculumId).ToListAsync();
        }


    }
}
