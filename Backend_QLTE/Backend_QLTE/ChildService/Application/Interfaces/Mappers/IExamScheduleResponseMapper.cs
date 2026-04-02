using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface IExamScheduleResponseMapper
    {
        ExamScheduleResponseDTO ToDto(ExamSchedule examSchedule);
        List<ExamScheduleResponseDTO> ToDtoList(List<ExamSchedule> examSchedules);

        UpdateExamScheduleResponseDTO ToUpdateDto(ExamSchedule examSchedule);
        List<UpdateExamScheduleResponseDTO> ToUpdateDtoList(List<ExamSchedule> examSchedules);
    }
}
