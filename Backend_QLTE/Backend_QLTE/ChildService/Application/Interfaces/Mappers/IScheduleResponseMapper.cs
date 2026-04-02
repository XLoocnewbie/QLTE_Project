using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface IScheduleResponseMapper
    {
        ScheduleResponseDTO ToDto(Schedule schedule);
        List<ScheduleResponseDTO> ToDtoList(List<Schedule> schedules);

        UpdateScheduleResponseDTO ToUpdateDto(Schedule schedule);
        List<UpdateScheduleResponseDTO> ToUpdateDtoList(List<Schedule> schedules);
    }
}
