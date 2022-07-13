using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace IoTGatewayCore.U
{
	public class LS<T>
	{
		private readonly ILogger<T> _l;

		public LS(ILogger<T> l)
		{
			_l = l;
		}

		public void Log(LogLevel level, string txt) => _l.Log(level, txt);

		public void TokenCheck(LogLevel level, string who, string token) => NullCheck(level, who, token, nameof(token));

		public void NullCheck(LogLevel level, string who, string obj, string objName)
		{
			if (string.IsNullOrWhiteSpace(obj))
				_l.Log(level, $"({who ?? "-"}) {objName} Is Null Or WhiteSpace");
		}

		public void MessageSrcTypeUnknown(LogLevel level, string who, string objName, Type type)
		{
			_l.Log(level, $"({who ?? "-"}) {objName} Is Unknown! Type={type ?? null}");
		}

		public void NullCheck(LogLevel level, string who, object obj, params object[] args)
		{
			if (obj == null)
			{
				var argsString = string.Empty;
				foreach (var arg in args)
				{
					var value = arg.GetType().IsPrimitive ? arg.ToString() : JsonSerializer.Serialize(arg);
					argsString += $"{nameof(arg)}={value} ";
				}
				_l.Log(level, $"({who ?? "-"}) {obj.GetType()} Is Null\t{argsString}");
			}
		}

		//public static void LogRoutine(EventArgs eventArgs)
		//{
		//	var logMeesage = GenerateLog(eventArgs);
		//	StoreLog(logMeesage);
		//	OutputLog(logMeesage);
		//}

		//public static void LogRoutine(Message message)
		//{
		//	var logMeesage = GenerateLog(message);
		//	StoreLog(logMeesage);
		//	OutputLog(logMeesage);
		//}

		//private static Message GenerateLog(EventArgs eventArgs)
		//{
		//	return new Message();
		//}

		//private static Message GenerateLog(Message message)
		//{
		//	return message;
		//}

		//private static void StoreLog(Message message)
		//{

		//}

		//private static void OutputLog(Message message)
		//{

		//}
	}
}