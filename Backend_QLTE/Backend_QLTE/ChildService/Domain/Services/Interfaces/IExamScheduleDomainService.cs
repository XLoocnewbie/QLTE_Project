using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Domain.Services.Interfaces
{
    public interface IExamScheduleDomainService
    {
        void EnsureCanCreate(ExamSchedule examSchedule, IEnumerable<ExamSchedule> existingExamSchedules);
        void EnsureCanUpdate(ExamSchedule examSchedule);
        void EnsureCanDelete(ExamSchedule examSchedule);
    }
}
