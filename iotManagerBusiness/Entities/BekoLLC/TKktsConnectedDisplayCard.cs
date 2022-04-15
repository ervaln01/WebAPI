namespace IotManagerBusiness.Entities.BekoLLC
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("T_KKTS_CONNECTED_DISPLAY_CARD")]
	public partial class TKktsConnectedDisplayCard
	{
		[Key]
		[Column("PRODUCT")]
		[StringLength(10)]
		public string Product { get; set; }

		[Key]
		[Column("SERIAL")]
		[StringLength(12)]
		public string Serial { get; set; }

		[Column("LINE")]
		public int Line { get; set; }

		[Required]
		[Column("DISPLAY_CARD")]
		[StringLength(255)]
		public string DisplayCard { get; set; }

		[Key]
		[Column("REGISTER_STATE")]
		public int RegisterState { get; set; }

		[Column("REGISTER_TIME", TypeName = "datetime")]
		public DateTime RegisterTime { get; set; }

		[Column("INSERT_TIME", TypeName = "datetime")]
		public DateTime InsertTime { get; set; }

		[Column("HAS_SHIPMENT")]
		public int HasShipment { get; set; }

		[Column("SHIPMENT_DATE", TypeName = "datetime")]
		public DateTime ShipmentDate { get; set; }

		public TKktsConnectedDisplayCard() { }
		public TKktsConnectedDisplayCard(string product, string serial, int line, string card, int state) 
		{
			Product = product;
			Serial = serial;
			Line = line;
			DisplayCard = card;
			RegisterState = state;
			RegisterTime = DateTime.Now;
		}
	}
}