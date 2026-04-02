using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Infrastructure.Data;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChildDbContext _context;

        public UnitOfWork
        (
            ChildDbContext context,
            IStudyPeriodRepository studyPeriod,
            ISOSRequestRepository sOSRequest,
            IDeviceInfoRepository deviceInfo,
            IChildRepository child,
            IScheduleRepository schedules,
            IExamScheduleRepository examSchedules)
        {
            _context = context;
            StudyPeriods = studyPeriod;
            SOSRequests = sOSRequest;
            DeviceInfos = deviceInfo;
            Childs = child;
            Schedules = schedules;
            ExamSchedules = examSchedules;
        }

        public IStudyPeriodRepository StudyPeriods { get; }
        public ISOSRequestRepository SOSRequests { get;  }
        public IDeviceInfoRepository DeviceInfos { get; }
        public IChildRepository Childs { get; }
        public IScheduleRepository Schedules { get; }
        public IExamScheduleRepository ExamSchedules { get; }

        public async Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }
    }

}
