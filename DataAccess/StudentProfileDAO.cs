using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class StudentProfileDAO : SingletonBase<StudentProfileDAO>
    {
        public async Task<IEnumerable<StudentProfile>> GetStudentProfileAll()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<StudentProfile> GetStudentProfile(int UserId, int StudentId)
        {
            var studentProfile = await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted)
                .FirstOrDefaultAsync(c => c.UserId == UserId && c.StudentId == StudentId);

            return studentProfile;
        }
        public async Task<StudentProfile> GetStudentProfileById(int id)
        {
            var studentProfile = await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.StudentId == id);

            return studentProfile;
        }


        public async Task<List<StudentProfile>> GetStudentProfilesByUserIds(List<int> userIds)
        {
            return await _context.StudentProfiles
                .Where(sp => userIds.Contains(sp.UserId) && !sp.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<StudentProfile> GetStudentProfileByUserId(int id)
        {
            var studentProfile = await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == id);

            return studentProfile;
        }

        public async Task Create(StudentProfile studentProfile)
        {
            var cohortCurriculum = await _context.CohortCurriculums
                .Where(cc => cc.Cohort == studentProfile.Cohort)
                .OrderBy(cc => cc.CurriculumId)
                .FirstOrDefaultAsync();

            if (cohortCurriculum == null)
            {
                throw new Exception($"Không tìm thấy CurriculumId cho Cohort {studentProfile.Cohort}");
            }

            studentProfile.CurriculumId = cohortCurriculum.CurriculumId;
            await _context.StudentProfiles.AddAsync(studentProfile);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StudentProfile studentProfile)
        {
            var existingItem = await GetStudentProfileById(studentProfile.StudentId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(studentProfile);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var studentProfile = await GetStudentProfileById(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDelete(int userId)
        {
            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == userId && !sp.IsDeleted);

            if (studentProfile != null)
            {
                studentProfile.IsDeleted = true;
                _context.StudentProfiles.Update(studentProfile);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteByUserId(int userId)
        {
            var studentProfile = _context.StudentProfiles.Where(g => g.UserId == userId);
            _context.StudentProfiles.RemoveRange(studentProfile);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Curriculum>> GetMandatorySubjectsAsync(int userId)
        {
            return await _context.Curriculums
                .Where(c => c.IsMandatory)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curriculum>> GetFailedMandatorySubjectsAsync(int userId)
        {
            return await _context.StudentGrades
                .Where(sg => sg.UserId == userId && sg.IsPassed == 0 && sg.Curriculum.IsMandatory)
                .Select(sg => sg.Curriculum)
                .ToListAsync();
        }
        public async Task<(List<int> MissingStudentIds, List<string> MissingCohorts)> ImportStudentProfilesAsync(IEnumerable<StudentProfile> profiles)
        {
            var existingStudentIds = await _context.Users.Select(u => u.UserId).ToHashSetAsync();
            var existingCohorts = await _context.CohortCurriculums
                .Select(c => new { c.Cohort, c.CurriculumId })
                .ToListAsync();

            var cohortDict = existingCohorts
                .GroupBy(c => c.Cohort)
                .ToDictionary(g => g.Key, g => g.First().CurriculumId);

            List<int> missingStudentIds = new List<int>();
            List<string> missingCohorts = new List<string>();

            var existingProfiles = await _context.StudentProfiles
                .Where(p => profiles.Select(sp => sp.StudentId).Contains(p.StudentId))
                .ToDictionaryAsync(p => p.StudentId);

            var profilesToAdd = new List<StudentProfile>();

            foreach (var profile in profiles)
            {
                if (!existingStudentIds.Contains(profile.UserId))
                {
                    missingStudentIds.Add(profile.UserId);
                    continue;
                }

                if (!cohortDict.TryGetValue(profile.Cohort, out var curriculumId))
                {
                    missingCohorts.Add(profile.Cohort);
                    continue;
                }

                profile.CurriculumId = curriculumId;

                if (existingProfiles.TryGetValue(profile.StudentId, out var existingProfile))
                {
                    _context.Entry(existingProfile).State = EntityState.Detached;

                    existingProfile.Cohort = profile.Cohort;
                    existingProfile.CurriculumId = profile.CurriculumId;
                    _context.StudentProfiles.Attach(existingProfile);
                    _context.Entry(existingProfile).State = EntityState.Modified;
                }
                else
                {
                    profilesToAdd.Add(profile);
                }
            }

            if (profilesToAdd.Any())
            {
                await _context.StudentProfiles.AddRangeAsync(profilesToAdd);
            }

            await _context.SaveChangesAsync();

            return (missingStudentIds, missingCohorts);
        }



    }
}
