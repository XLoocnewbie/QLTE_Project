using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface ISOSRequestResponseMapper
    {
        SOSRequestResponseDTO ToDto(SOSRequest entity);
        List<SOSRequestResponseDTO> ToDtoList(List<SOSRequest> entities);

        UpdateSOSRequestResponseDTO ToUpdateDto(SOSRequest entity);
        List<UpdateSOSRequestResponseDTO> ToUpdateDtoList(List<SOSRequest> entities);
    }
}
