using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

namespace SLZ.Marrow.SceneStreaming
{
	public class ChunkLoader
	{
		private SceneLoadQueue _sceneQueue;

		private List<Chunk> _activeChunks;

		private List<Chunk> _chunksToLoad;

		private List<Chunk> _chunksToUnload;

		private List<Chunk> _occupiedChunks;

		private HashSet<string> _wasLoadedOnce;

		private static Dictionary<string, List<ChunkTrigger>> _chunkToTrigger;

		private Chunk _bufferedChunk;

		private bool _isLoading;

		public List<ChunkTrigger> Triggers { get; private set; }

		public List<ChunkTracker> Trackers { get; private set; }

		public void SetOccupiedChunk(Chunk chunk)
		{
		}

		public void RemoveOccupiedChunk(Chunk chunk)
		{
		}

		public void RegisterTrigger(ChunkTrigger trigger)
		{
		}

		public void UnregisterTrigger(ChunkTrigger trigger)
		{
		}

		public void RegisterTracker(ChunkTracker tracker)
		{
		}

		public void UnregisterTracker(ChunkTracker tracker)
		{
		}

		private void SolveChunks(Chunk chunk)
		{
		}

		public UniTask SetActive(Chunk chunk)
		{
			return default(UniTask);
		}

		private UniTask Load(Chunk chunk)
		{
			return default(UniTask);
		}

		private UniTask UnloadChunks()
		{
			return default(UniTask);
		}

		private UniTask LoadChunks()
		{
			return default(UniTask);
		}

		public ChunkLoader()
			: base()
		{
		}
	}
}
