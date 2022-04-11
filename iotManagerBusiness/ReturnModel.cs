namespace IotManagerBusiness
{
	public class ReturnModel<T>
	{
		public T Data { get; set; }
		public ReturnTypeStatus Status { get; set; }
		public string Message { get; set; }
	}
}