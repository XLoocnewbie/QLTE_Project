using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Factories
{
    public interface IExamScheduleFactory
    {
        ExamSchedule Create(ExamScheduleCreateDTO dto);
        ExamSchedule Update(ExamScheduleUpdateDTO dto);
        ExamSchedule Delete(ExamScheduleDeleteDTO dto);
    }
}
