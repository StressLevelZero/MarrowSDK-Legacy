using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SLZ.Marrow.SceneStreaming
{
	public class SceneLoadQueue
	{
		private Dictionary<Scene, uint> _loadedScenes;

		private Dictionary<string, SceneInstance> _addressToInstance;

		private HashSet<string> _loadAddressesHash;

		private List<string> _loadAddresses;

		private List<string> _unloadAddresses;

		private List<UniTask<SceneInstance>> _loadTasks;

		private List<UniTask> _unloadTasks;

		private CancellationTokenSource _loadCts;

		private CancellationTokenSource _unloadCts;

		public SceneInstance GetInstance(string address)
		{
			return default(SceneInstance);
		}

		public void AddLoad(string address)
		{
		}

		public void AddUnload(string address)
		{
		}

		public void Cancel()
		{
		}

		public UniTask LoadAll(LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true)
		{
			return default(UniTask);
		}

		public UniTask UnloadAll(bool autoReleaseHandle = true)
		{
			return default(UniTask);
		}

		~SceneLoadQueue()
		{
		}

		public SceneLoadQueue()
			: base()
		{
		}
	}
}
