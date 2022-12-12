using System;

namespace SiberianWellness.Common
{
	public static class EnumExtension
	{ 
		public static string GetString(this Enum e)
		{ 
			// TODO можно кэшировать имена
			return e.ToString();
		}
		
		public static bool HasFlag(this Enum val, Enum flags)
		{
			long ulongVal   = Convert.ToInt64(val);
			long ulongFlags = Convert.ToInt64(flags);

			if (ulongFlags == 0)
			{
				return false;
			}

			return (ulongVal & ulongFlags) == ulongFlags;
		}
	}
}