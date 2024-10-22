using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

namespace StarterKit.Services
{
    public class OfficeAttendanceService : IOfficeAttendanceService
    {
        private readonly DatabaseContext _context;

        public OfficeAttendanceService(DatabaseContext context)
        {
            _context = context;
        }

        public OfficeAttendanceResult BookOfficeAttendance(OfficeAttendanceModel model)
        {
            var user = _context.User.Find(model.UserId);
            if (user == null)
            {
                return new OfficeAttendanceResult { Success = false, Message = "User not found" };
            }

            var officeAttendance = new OfficeAttendanceModel
            {
                UserId = model.UserId,
                Date = model.Date
            };

            _context.OfficeAttendances.Add(officeAttendance);
            _context.SaveChanges();

            return new OfficeAttendanceResult { Success = true };
        }

        public OfficeAttendanceResult UpdateOfficeAttendance(OfficeAttendanceModel model)
        {
            var officeAttendance = _context.OfficeAttendances.FirstOrDefault(a => a.UserId == model.UserId && a.Date == model.Date);
                

            if (officeAttendance == null)
            {
                return new OfficeAttendanceResult { Success = false, Message = "Office attendance not found" };
            }

            officeAttendance.Date = model.Date;

            _context.SaveChanges();

            return new OfficeAttendanceResult { Success = true };
        }

        public OfficeAttendanceResult CancelOfficeAttendance(OfficeAttendanceModel model)
        {
            var officeAttendance = _context.OfficeAttendances
                .FirstOrDefault(a => a.UserId == model.UserId && a.Date == model.Date);

            if (officeAttendance == null)
            {
                return new OfficeAttendanceResult { Success = false, Message = "Office attendance not found" };
            }

            _context.OfficeAttendances.Remove(officeAttendance);
            _context.SaveChanges();

            return new OfficeAttendanceResult { Success = true };
        }
    }
}