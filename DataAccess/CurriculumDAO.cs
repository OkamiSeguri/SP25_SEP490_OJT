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
                    // ✅ CurriculumId đã tồn tại → Ghi đè
                    var existingCurriculum = await _context.Curriculums.FindAsync(curriculum.CurriculumId);
                    _context.Entry(existingCurriculum).CurrentValues.SetValues(curriculum);
                }
                else
                {
                    // ✅ CurriculumId không tồn tại → Kiểm tra SubjectCode
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
