using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Factories
{
    public interface IScheduleFactory
    {
        Schedule Create(ScheduleCreateDTO dto);
        Schedule Update(ScheduleUpdateDTO dto);
        Schedule Delete(ScheduleDeleteDTO dto);
    }
}
