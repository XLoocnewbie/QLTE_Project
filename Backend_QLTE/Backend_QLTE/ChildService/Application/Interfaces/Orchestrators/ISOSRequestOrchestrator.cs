using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Orchestrators
{
    public interface ISOSRequestOrchestrator
    {
        Task<SOSRequest> CreateAsync(SOSRequestCreateDTO dto, CancellationToken cancellationToken = default);
        Task<SOSRequest> UpdateAsync(SOSRequestUpdateDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(SOSRequestDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
