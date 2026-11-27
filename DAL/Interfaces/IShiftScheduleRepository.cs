using Entity.Entities;

namespace DAL.Interfaces;

public interface IShiftScheduleRepository
{
    List<ShiftSchedule> GetAll(DateTime? fromDate = null, DateTime? toDate = null, int? employeeId = null);
    List<WorkShift> GetWorkShifts();
    int Add(ShiftSchedule schedule);
    bool Update(ShiftSchedule schedule);
    bool Delete(int scheduleId);
}
