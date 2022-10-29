using System.Collections.Generic;

namespace SiberianWellness.Common
{
    public class EditorDirectory<T>
    {
        public bool IsFoldOutOnBrouser;
        
        public Dictionary<string, EditorDirectory<T>> directories = new Dictionary<string, EditorDirectory<T>>();
        
        public List<T> files = new List<T>();

        public EditorDirectory<T> GetOrCreate(string dirName)
        {
            return directories.GetOrCreate(dirName, ()=> new EditorDirectory<T>());
        }

        public void AddFile(T file)
        {
            files.Add(file);
        }
    }
}
