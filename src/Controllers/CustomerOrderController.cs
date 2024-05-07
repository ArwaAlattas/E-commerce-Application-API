using api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("/api/customer-order")]
    public class CustomerOrderController : ControllerBase
    {

        private readonly CustomerOrderService _customerOrderService;


        public CustomerOrderController(AppDBContext appDbContext)
        {
            _customerOrderService = new CustomerOrderService(appDbContext);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _customerOrderService.GetAllOrdersService();
            return ApiResponse.Success(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetCustomerOrderById(string orderId)
        {
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                return ApiResponse.BadRequest("Invalid user ID Format");
            }

            var order = await _customerOrderService.GetOrderById(orderIdGuid);

            if (order == null)
            {
                return ApiResponse.NotFound();
            }

            return ApiResponse.Success(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CustomerOrderModel newOrder)
        {
            try
            {
                await _customerOrderService.CreateOrderService(newOrder);
                return ApiResponse.Created("Order has added successfully!");
            }
            catch (Exception e)
            {
                return ApiResponse.ServerError(e.Message);
            }
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> AddProductToOrder(Guid orderId, Guid productId)
        {
            try
            {
                await _customerOrderService.AddProductToOrder(orderId, productId);
                return ApiResponse.Created("Products Added to the order successfully");
            }
            catch (Exception e)
            {
                return ApiResponse.ServerError(e.Message);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(string orderId, CustomerOrderModel updateOrder)
        {
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                return ApiResponse.BadRequest("Invalid user ID Format");
            }
            var result = await _customerOrderService.UpdateOrderService(orderIdGuid, updateOrder);
            if (result)
            {
                return ApiResponse.Updated("Order has updated successfully");
            }
            return ApiResponse.NotFound();
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            if (!Guid.TryParse(orderId, out Guid orderIdGuid))
            {
                return ApiResponse.BadRequest("Invalid user ID Format");
            }
            var result = await _customerOrderService.DeleteOrderService(orderIdGuid);
            if (result)
            {
                return ApiResponse.Deleted("Order has deleted Successfully");
            }
            return ApiResponse.NotFound("Order not Found");
        }
    }

}

