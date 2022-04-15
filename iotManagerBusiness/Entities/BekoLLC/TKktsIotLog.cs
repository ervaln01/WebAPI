namespace IotManagerBusiness.Entities.BekoLLC
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("T_KKTS_IOT_LOG")]
	public partial class TKktsIotLog
	{
		[StringLength(50)]
		public string LogType { get; set; }

		[StringLength(50)]
		public string LogStatus { get; set; }

		public string LogText { get; set; }

		[StringLength(50)]
		public string LogOperator { get; set; }
	}
}