using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Factories
{
    public interface ISOSRequestFactory
    {
        SOSRequest Create(SOSRequestCreateDTO dto);
        SOSRequest Update(SOSRequestUpdateDTO dto);
        SOSRequest Delete(SOSRequestDeleteDTO dto);
    }
}
