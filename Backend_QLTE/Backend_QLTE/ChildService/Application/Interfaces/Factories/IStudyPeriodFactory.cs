using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Factories
{
    public interface IStudyPeriodFactory
    {
        StudyPeriod Create(StudyPeriodCreateDTO dto);
        StudyPeriod Update(StudyPeriodUpdateDTO dto);
        StudyPeriod Delete(StudyPeriodDeleteDTO dto);
    }
}
