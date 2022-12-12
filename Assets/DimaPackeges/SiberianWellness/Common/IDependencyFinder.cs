 

namespace SiberianWellness.Common
{
	public interface IDependencyFinder
	{
#if UNITY_EDITOR
		void FindDependency();
#endif
	}
}