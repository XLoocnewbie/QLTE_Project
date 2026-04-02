using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateExamScheduleException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateExamScheduleException(string monThi, DateTime thoiGianThi)
            : base($"Đã tồn tại lịch thi cho môn '{monThi}' vào thời gian {thoiGianThi:HH:mm dd/MM/yyyy}.") { }
    }
}
