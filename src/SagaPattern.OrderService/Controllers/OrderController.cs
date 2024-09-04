using Microsoft.AspNetCore.Mvc;
using SagaPattern.OrderService.Models;

namespace SagaPattern.OrderService.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly List<Order> _orders;

		public OrderController()
		{
			_orders = 
				[
					new Order(){ Id = 1, ProductId = "Produto 1", Quantity = 10, Status = string.Empty }
				];
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Order order)
		{
			order.Id = _orders.Count + 1;
			order.Status = "Created";
			_orders.Add(order);

			await Task.CompletedTask;
			return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var order = await Task.FromResult(_orders.Find(x => x.Id == id));
			if (order == null)
			{ 
				return NotFound();
			}
			return Ok(order);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Cancel(int id)
		{
			var order = await Task.FromResult(_orders.Find(x => x.Id == id));
			if (order == null)
			{
				return NotFound();
			}
			_orders.Remove(order);
			return NoContent();
		}
	}
}
