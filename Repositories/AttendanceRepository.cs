using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterKit.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DatabaseContext _context;

        public AttendanceRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _context.Set<Attendance>().ToListAsync();
        }

        public async Task<Attendance> GetAttendanceByIdAsync(int id)
        {
            return await _context.Set<Attendance>().FindAsync(id);
        }

        public async Task AddAttendanceAsync(Attendance attendance)
        {
            await _context.Set<Attendance>().AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            _context.Set<Attendance>().Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task<Attendance> GetAttendanceByDateAsync(DateTime date)
        {
            return await _context.Set<Attendance>()
                .FirstOrDefaultAsync(a => a.AttendanceDate.Date == date.Date);
        }

        public async Task<Attendance> GetAttendanceByDateAndUserIdAsync(DateTime date, int userId)
        {
            return await _context.Set<Attendance>()
                .FirstOrDefaultAsync(a => a.AttendanceDate.Date == date.Date && a.User.Id == userId);
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            var attendance = await GetAttendanceByIdAsync(id);
            if (attendance != null)
            {
                _context.Set<Attendance>().Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }
    }
}
