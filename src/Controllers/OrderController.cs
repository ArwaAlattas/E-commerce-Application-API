using System.Security.Claims;
using api.Controllers;
using api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("/api/")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _orderService.GetAllOrdersService();
            return ApiResponse.Success(orders);
        }

        // Only unbanned Users can get their orders 
        [Authorize(Roles = "notBanned")]
        [HttpGet("orders/my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new BadRequestException("Invalid user ID Format");
            }

            var order = await _orderService.GetMyOrders(userId);
            if (order == null)
            {
                throw new NotFoundException("Order Not Found");
            }

            return ApiResponse.Success(order);
        }

        // Only Admin can return orders by chosen Id
        [Authorize(Roles = "Admin")]
        [HttpGet("history/orders/{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                throw new NotFoundException("Order Not Found");
            }

            return ApiResponse.Success(order);
        }

        [Authorize(Roles = "notBanned")]
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromQuery] List<Guid> productIds, PaymentMethod paymentMethod)
        {
            // Create Order
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new BadRequestException("Invalid User Id");
            }
            var orderId = await _orderService.CreateOrderService(userId, paymentMethod);

            // Add product the order
            await _orderService.AddProductsToOrder(orderId,productIds);
            return ApiResponse.Created("Order has added successfully!");
        }

        // [Authorize(Roles = "notBanned")]
        // [HttpPost("{orderId}")]
        // public async Task<IActionResult> AddProductToOrder(Guid orderId, Guid productId)
        // {
        //     var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     if (string.IsNullOrEmpty(userIdString))
        //     {
        //         throw new UnauthorizedAccessException("User Id is missing from token");
        //     }
        //     await _orderService.AddProductToOrder(orderId, productId);
        //     return ApiResponse.Created("Products Added to the order successfully");
        // }

        [Authorize(Roles = "Admin")]
        [HttpPut("history/orders/{orderId}/update")]
        public async Task<IActionResult> UpdateOrder(string orderId, OrderModel updateOrder)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            var result = await _orderService.UpdateOrderService(orderIdGuid, updateOrder);
            if (result)
            {
                return ApiResponse.Updated("Order has updated successfully");
            }
            throw new NotFoundException("Order Not Found");
        }

        [Authorize(Roles = "notBanned")]
        [HttpPut("history/my-orders/{orderId}/update")]
        public async Task<IActionResult> UpdateMyOrder(string orderId, OrderModel updateOrder)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            var result = await _orderService.UpdateOrderService(userId, orderIdGuid, updateOrder);
            if (result)
            {
                return ApiResponse.Updated("Order has updated successfully");
            }
            throw new NotFoundException("Order Not Found");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("history/orders/{orderId}/delete")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            var result = await _orderService.DeleteOrderService(orderIdGuid);
            if (result)
            {
                return ApiResponse.Deleted("Order has deleted Successfully");
            }
            throw new NotFoundException("Order Not Found");
        }

        [Authorize(Roles = "notBanned")]
        [HttpDelete("history/my-orders/{orderId}/delete")]
        public async Task<IActionResult> DeleteMyOrder(string orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User Id is missing from token");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                throw new BadRequestException("Invalid user ID Format");
            }
            var result = await _orderService.DeleteOrderService(userId, orderIdGuid);
            if (result)
            {
                return ApiResponse.Deleted("Order has deleted Successfully");
            }
            throw new NotFoundException("Order Not Found");
        }
    }
}