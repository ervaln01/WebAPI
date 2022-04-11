namespace IotManagerBusiness
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	class DbParam
	{
		public string ParamName { get; set; }
		public object ParamValue { get; set; }
		public SqlDbType ParamType { get; set; }

		public DbParam(string paramName, object paramValue, Type paramType)
		{
			ParamType = GetDBType(paramType);
			ParamName = paramName;
			if (paramValue == null)
			{
				ParamValue = DBNull.Value;
				return;
			}

			if (!paramType.Equals(typeof(DateTime)))
			{
				ParamValue = paramValue;
				return;
			}

			ParamValue = Convert.ToDateTime(paramValue) == DateTime.MinValue ? DBNull.Value : paramValue;
		}

		private static SqlDbType GetDBType(Type type)
		{
			var param = new SqlParameter();
			var tc = System.ComponentModel.TypeDescriptor.GetConverter(param.DbType);
			if (tc.CanConvertFrom(type))
			{
				param.DbType = (DbType)tc.ConvertFrom(type.Name);
				return param.SqlDbType;
			}

			try
			{
				param.DbType = (DbType)tc.ConvertFrom(type.Name);
			}
			catch { }
			return param.SqlDbType;
		}
	}
}