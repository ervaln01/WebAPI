namespace IotManagerBusiness.Entities.Etiket
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("T_ATR_VALUE_tr_TR")]
	public partial class TAtrValueTrTr
	{
		[Required]
		[Column("SKUNUMBER")]
		[StringLength(18)]
		public string SkuNumber { get; set; }

		[Required]
		[Column("RECORDID")]
		[StringLength(10)]
		public string RecordId { get; set; }

		[Required]
		[Column("ATR_TYPE")]
		[StringLength(20)]
		public string AtrType { get; set; }

		[Required]
		[Column("ATR_CODE")]
		[StringLength(50)]
		public string AtrCode { get; set; }

		[Required]
		[Column("ATR_VALID")]
		[StringLength(255)]
		public string AtrValid { get; set; }

		[Column("ATR_VALUE")]
		[StringLength(500)]
		public string AtrValue { get; set; }

		[Column("LOOKUPCODE")]
		[StringLength(50)]
		public string LookupCode { get; set; }

		[Column("DESCRIPTION")]
		[StringLength(255)]
		public string Description { get; set; }

		[Column("ATR_VALUE_MDM")]
		[StringLength(500)]
		public string AtrValueMdm { get; set; }

		[Column("ATR_CODE_ONAY")]
		public int? AtrCodeOnay { get; set; }

		[Column("MULTIVALUE")]
		public bool? MultiValue { get; set; }

		[Column("ATR_VALUE_MDM_EN")]
		[StringLength(500)]
		public string AtrValueMdmEn { get; set; }

		[Column("DESCRIPTION_EN")]
		[StringLength(500)]
		public string DescriptionEn { get; set; }
	}
}