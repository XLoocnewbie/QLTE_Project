namespace Backend_QLTE.AuthService.Application.DTOs.Common
{
    public class PaginationDTO
    {
        public int Page { get; set; } // Số trang hiện tại FE yêu cầu 
        public int Last { get; set; } // Số trang cuối FE database
        public int Limit { get; set; } // Số lượng phần tử trên mỗi trang
        public int Total { get; set; } // Tổng số phần tử chưa phân trang (dữ liệu gốc)
    }
}
