using StarterKit.Models;

namespace StarterKit.Repositories
{
	public interface IAttendanceRepository
	{
		Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
		Task<Attendance> GetAttendanceByIdAsync(int id);
		Task AddAttendanceAsync(Attendance Attendance);
		Task UpdateAttendanceAsync(Attendance Attendance);
		Task<Attendance> GetAttendanceByDateAsync(DateTime date);
		
		Task<Attendance> GetAttendanceByDateAndUserIdAsync(DateTime date, int userId);

		Task DeleteAttendanceAsync(int id);
	}
}