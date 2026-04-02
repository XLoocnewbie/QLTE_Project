using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IChildService 
    {
        Task<ResultListDTO<ParentWithChildrenResponseDTO>> GetAllParentsWithChildrenAsync();
        Task<ResultListDTO<ChildrenResponseDTO>> GetChildrenByUserIdAsync(string userId);
        Task<ResultDTO> CreateChildAsync(CreateChildRequestDTO request);
        Task<ResultDTO<ChildrenResponseDTO>> UpdateChildAsync(UpdateChildRequestDTO request);
        Task<ResultDTO> DeleteChildAsync(Guid childrenId);
        Task<ResultDTO<ChildrenResponseDTO>> GetChildByUserIdAsync(string userId);
    }
}
