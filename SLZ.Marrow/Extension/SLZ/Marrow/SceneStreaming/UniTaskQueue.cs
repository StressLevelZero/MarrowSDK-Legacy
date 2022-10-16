using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

namespace SLZ.Marrow.SceneStreaming
{
	public class UniTaskQueue<T>
	{
		private List<UniTask<T>> _taskQueue;

		public void Add(UniTask<T> task)
		{
		}

		public UniTask<T[]> WhenAll()
		{
			return default(UniTask<T[]>);
		}

		public UniTaskQueue()
			: base()
		{
		}
	}
}
