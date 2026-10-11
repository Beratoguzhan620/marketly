using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderDto>> CreateOrderAsync(int userId, OrderCreateRequest request);

        Task<List<OrderDto>> GetMyOrdersAsync(int userId);

        Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(int userId, string orderCode);
    }
}
