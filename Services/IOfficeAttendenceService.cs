using StarterKit.Models;

namespace StarterKit.Services
{
    public interface IOfficeAttendanceService
    {
        OfficeAttendanceResult BookOfficeAttendance(OfficeAttendanceModel model);
        OfficeAttendanceResult UpdateOfficeAttendance(OfficeAttendanceModel model);
        OfficeAttendanceResult CancelOfficeAttendance(OfficeAttendanceModel model);

    }
}