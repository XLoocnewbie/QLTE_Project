using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface IStudyPeriodResponseMapper
    {
        StudyPeriodResponseDTO ToDto(StudyPeriod studyPeriod);
        List<StudyPeriodResponseDTO> ToDtoList(List<StudyPeriod> studyPeriods);

        UpdateStudyPeriodResponseDTO ToUpdateDto(StudyPeriod studyPeriod);
        List<UpdateStudyPeriodResponseDTO> ToUpdateDtoList(List<StudyPeriod> studyPeriods);
    }
}
