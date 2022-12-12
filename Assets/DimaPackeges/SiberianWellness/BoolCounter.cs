using System.Collections.Generic;
using SiberianWellness.Common;
using UnityEngine.Assertions;

namespace SiberianWellness
{
	public class BoolCounter
	{
		int counter;

		public bool IsZero => counter == 0;

		public void Reset()
		{
			counter = 0;
		}

		public void Increase()
		{
			counter++;
		}

		public void Decrease()
		{
			counter--;
		}
	}

	public class BoolCounter2
	{
		public LinkedList<Value> values = new LinkedList<Value>();

		public bool IsZero => values.Count == 0;

		public class Value
		{
			public readonly string                name;
			public          bool                  Expired { get; private set; }
			public          LinkedListNode<Value> thisNode;

			List<string> log;

			public Value(string name)
			{
				Assert.IsFalse(string.IsNullOrWhiteSpace(name));
				this.name = name;
			}

			public void Log(string line)
			{
				if (log == null)
					log = new List<string>(4);

				log.Add(line);
			}

			public void Return()
			{
				Assert.IsFalse(Expired);

				Expired = true;
				thisNode.List.Remove(thisNode);
			}

			public string Print()
			{
				if (log == null)
					return name;
				else
					return log.ToStringMultilineWithIndex(name, tabCount: 1);
			}
		}

		public Value Add(string valueName = "NoName")
		{
			var newValue = new Value(valueName);
			newValue.thisNode = values.AddLast(newValue);
			return newValue;
		}
	}
}