using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Domain.Services.Interfaces
{
    public interface ISOSRequestDomainService
    {
        void EnsureCanCreate(SOSRequest sosRequest, IEnumerable<SOSRequest> existingRequests);
        void EnsureCanUpdate(SOSRequest sosRequest);
        void EnsureCanDelete(SOSRequest sosRequest);
    }
}
