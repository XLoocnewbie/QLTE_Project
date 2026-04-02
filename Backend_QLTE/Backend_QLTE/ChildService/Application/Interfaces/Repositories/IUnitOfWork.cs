namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IStudyPeriodRepository StudyPeriods { get; } // StudyPeriod Repository
        ISOSRequestRepository SOSRequests { get; } // SOSRequest Repository
        IDeviceInfoRepository DeviceInfos { get; } // DeviceInfo Repository
        IChildRepository Childs { get; } // Child Repository
        IScheduleRepository Schedules { get; } // Schedule Repository
        IExamScheduleRepository ExamSchedules { get; } // ExamSchedule Repository

        Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default); // Transaction
    }
}
