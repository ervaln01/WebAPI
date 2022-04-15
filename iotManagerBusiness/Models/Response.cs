namespace IotManagerBusiness.Models
{
	using IotManagerBusiness.Enums;

	public class Response<T>
	{
		public T Data { get; set; }
		public StatusType Status { get; set; }
		public string Message { get; set; }
		public Response() { }

		public Response(T data, StatusType status, string message)
		{
			Data = data;
			Status = status;
			Message = message;
		}
	}
}