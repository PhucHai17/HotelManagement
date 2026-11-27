using BLL.Interfaces;
using BLL.Services;

namespace BLL;

public static partial class AppServices
{
    private static Lazy<IShiftScheduleService> _shiftSchedule = CreateLazy<IShiftScheduleService>(() => new ShiftScheduleService());
    public static IShiftScheduleService ShiftSchedule => _shiftSchedule.Value;
}
