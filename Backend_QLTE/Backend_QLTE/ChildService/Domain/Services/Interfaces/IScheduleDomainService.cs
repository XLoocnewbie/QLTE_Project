using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Domain.Services.Interfaces
{
    public interface IScheduleDomainService
    {
        void EnsureCanCreate(Schedule schedule, IEnumerable<Schedule> existingSchedules);
        void EnsureCanUpdate(Schedule schedule);
        void EnsureCanDelete(Schedule schedule);
    }
}
