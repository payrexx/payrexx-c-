
using System.Collections.Generic;

namespace Demo
{
	public class PayrexxGateway
	{
		public int Id { get; set; }
		public int Amount { get; set; }
		public float? VatRate { get; set; }
		public string Sku { get; set; }
		public string Currency { get; set; }
		public string Purpose { get; set; }
		public int[] Psp { get; set; }
		public string[] Pm { get; set; }
		public bool PreAuthorization { get; set; }
		public bool Reservation { get; set; }
		public string ReferenceId { get; set; }
		public Dictionary<string, PayrexxField> Fields { get; set; }
		public string ConcardisOrderId { get; set; }
		public string SuccessRedirectUrl { get; set; }
		public string FailedRedirectUrl { get; set; }
		public string CancelRedirectUrl { get; set; }
		public bool SkipResultPage { get; set; }
		public bool ChargeOnAuthorization { get; set; }
		public int Validity { get; set; }
		public string[] ButtonText { get; set; }
		public string LookAndFeelProfile { get; set; }
		public string SuccessMessage { get; set; }
		public object[] Cart { get; set; }
		public string Hash { get; set; }
		public string Link { get; set; }
		public int CreatedAt { get; set; }
	}
}