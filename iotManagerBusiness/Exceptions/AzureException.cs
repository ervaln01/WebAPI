namespace IotManagerBusiness.Exceptions
{
	using System;

	public class AzureException : Exception
	{
		public AzureException() { }
		public AzureException(string message) : base(message) { }
		public AzureException(string message, Exception inner) : base(message, inner) { }
	}
}