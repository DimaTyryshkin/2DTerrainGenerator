using System;
using System.Globalization;

public class DateTimeExtension
{
	public static string dateFormat = "dd/MM/yyyy HH:mm zzz"; //"25/08/2021 20:55 +7:00"

	public static DateTime ParseDate(string time)
	{
		CultureInfo provider = CultureInfo.InvariantCulture;
		DateTime dateTime = DateTime.ParseExact(time, dateFormat, provider);
		return dateTime;
	}

}
