using Entity.Entities;

namespace BLL.Interfaces;

public interface IShiftScheduleService
{
    List<ShiftSchedule> GetAll(DateTime fromDate, DateTime toDate);
    List<WorkShift> GetWorkShifts();
    bool Add(ShiftSchedule schedule, out string message);
    bool Update(ShiftSchedule schedule, out string message);
    bool Delete(int scheduleId, out string message);
}
