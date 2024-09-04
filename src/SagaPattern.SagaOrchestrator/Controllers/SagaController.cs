using Microsoft.AspNetCore.Mvc;

namespace SagaPattern.SagaOrchestrator.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class SagaController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public SagaController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpPost]
		public async Task<IActionResult> Process([FromBody] Order order)
		{
			order.Status = "Pending";
			var orderResponse = await CreateOrder(order);
			if (!orderResponse.IsSuccessStatusCode)
			{
				return BadRequest("Order creation failed.");
			}
			var inventoryResponse = await ReserveInventory(order);
			if (!inventoryResponse.IsSuccessStatusCode)
			{
				await CancelOrder(order.Id);
				return BadRequest("Inventory reservation failed.");
			}
			var paymentResponse = await ProcessPayment(order);
			if (!paymentResponse.IsSuccessStatusCode)
			{
				await ReleaseInventory(order);
				await CancelOrder(order.Id);
				return BadRequest("Payment processing failed.");
			}
			order.Status = "Completed";
			return Ok(order);
		}

		private async Task<HttpResponseMessage> CreateOrder(Order order)
		{
			var client = _httpClientFactory.CreateClient();
			return await client.PostAsJsonAsync("http://localhost:5298/order", order);
		}

		private async Task<HttpResponseMessage> CancelOrder(int orderId)
		{
			var client = _httpClientFactory.CreateClient();
			return await client.DeleteAsync($"http://localhost:5298/order/{orderId}");
		}

		private async Task<HttpResponseMessage> ReserveInventory(Order order)
		{
			var client = _httpClientFactory.CreateClient();
			return await client.PostAsJsonAsync(
				"http://localhost:5152/Inventory/reserve",
				new { order.ProductId, order.Quantity }
			);
		}

		private async Task<HttpResponseMessage> ReleaseInventory(Order order)
		{
			var client = _httpClientFactory.CreateClient();
			return await client.PostAsJsonAsync(
				"http://localhost:5152/Inventory/release", 
				new { order.ProductId, order.Quantity }
			);
		}

		private async Task<HttpResponseMessage> ProcessPayment(Order order)
		{
			var client = _httpClientFactory.CreateClient();
			return await client.PostAsJsonAsync(
				"http://localhost:5201/Payment",
				new { orderId = order.Id, Amount = order.Quantity * 10 }
			);
		}
	}
}
