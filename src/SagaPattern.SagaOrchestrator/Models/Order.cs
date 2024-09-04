namespace SagaPattern.SagaOrchestrator.Models
{
	public class Order
	{
		public int Id { get; set; } = 0;
		public string? ProductId { get; set; } = string.Empty;
		public int Quantity { get; set; } = 0;
		public string? Status { get; set; } = string.Empty;
	}
}
