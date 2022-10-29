using System; 
using SiberianWellness.NotNullValidation.Internal;
using SiberianWellness.Validation; 
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace SiberianWellness.NotNullValidation
{
	public class IsntNull
	{ 
		public static TimeSpan totalTime = new TimeSpan();

		public static void Assert(Object mb)
		{
			return;
			using (RecursiveValidator.StartSearch(new NotNullFieldValidator(),false))
			{
				RecursiveValidator.ValidateObject(mb);
			}
		}
	}
}