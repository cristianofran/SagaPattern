using Microsoft.AspNetCore.Mvc;
using SagaPattern.InventoryService.Models;
using System.Text.Json;

namespace SagaPattern.InventoryService.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly List<Inventory> _inventories;

		public InventoryController()
		{
			_inventories = 
			[
				new Inventory { ProductId = "Product1", Stock = 100 },
				new Inventory { ProductId = "Product2", Stock = 200 }
			];
		}

		[HttpPost("reserve")]
		public async Task<IActionResult> Reserve([FromBody] dynamic request)
		{
			var j = ((JsonElement)request);
			var id = j.GetProperty("productId").ToString();
			var quantity = Convert.ToInt32(j.GetProperty("quantity").ToString());

			var invetory = _inventories.FirstOrDefault(x => x.ProductId == id);
			if (invetory == null || invetory.Stock < quantity)
			{
				return BadRequest("Insufficient stock.");
			}
			invetory.Stock -= quantity;

			await Task.CompletedTask;
			return Ok();
		}

		[HttpPost("release")]
		public async Task<IActionResult> Release([FromBody] dynamic request)
		{
			var j = ((JsonElement)request);
			var id = j.GetProperty("productId").ToString();
			var quantity = Convert.ToInt32(j.GetProperty("quantity").ToString());

			var invetory = _inventories.FirstOrDefault(x => x.ProductId == id);
			if (invetory == null)
			{
				return NotFound();
			}
			invetory.Stock += quantity;

			await Task.CompletedTask;
			return Ok();
		}
	}
}
