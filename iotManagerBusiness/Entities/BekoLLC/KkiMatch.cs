namespace IotManagerBusiness.Entities.BekoLLC
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("KKI_MATCH")]
	public partial class KkiMatch
	{
		[Required]
		[Column("COMPONENTCODE")]
		[StringLength(4)]
		public string Componentcode { get; set; }

		[Required]
		[Column("PRODUCT")]
		[StringLength(10)]
		public string Product { get; set; }

		[Required]
		[Column("SERIAL")]
		[StringLength(12)]
		public string Serial { get; set; }

		[Required]
		[Column("MATERIAL")]
		[StringLength(10)]
		public string Material { get; set; }

		[Required]
		[Column("BARCODE")]
		[StringLength(300)]
		public string Barcode { get; set; }

		[Required]
		[Column("MODEL")]
		[StringLength(300)]
		public string Model { get; set; }

		[Required]
		[Column("USERID")]
		[StringLength(8)]
		public string Userid { get; set; }

		[Column("SYSDATE", TypeName = "datetime")]
		public DateTime Sysdate { get; set; }
	}
}