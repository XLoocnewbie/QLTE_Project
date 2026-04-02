using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Domain.Services.Interfaces
{
    public interface IStudyPeriodDomainService
    {
        void EnsureCanCreate(StudyPeriod studyPeriod);
        void EnsureCanUpdate(StudyPeriod studyPeriod);
        void EnsureCanDelete(StudyPeriod studyPeriod);
    }
}
