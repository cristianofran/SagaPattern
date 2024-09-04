using Microsoft.AspNetCore.Mvc;
using SagaPattern.PaymentService.Models;
using System.Text.Json;

namespace SagaPattern.PaymentService.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly List<Payment> _payments;

		public PaymentController()
		{
			_payments = new List<Payment>();
		}

		[HttpPost]
		public async Task<IActionResult> Process([FromBody] dynamic request)
		{
			var j = ((JsonElement)request);
			var orderId = Convert.ToInt32(j.GetProperty("orderId").ToString());
			var amount = Convert.ToInt32(j.GetProperty("amount").ToString());

			var payment = new Payment
			{
				OrderId = orderId,
				Amount = amount,
				Status = "Processed"
			};
			_payments.Add(payment);

			await Task.CompletedTask;
			return Ok(payment);
		}
	}
}
