using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Warehouse;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SLZ.Marrow.SceneStreaming
{
	public class StreamSession
	{
		private readonly LevelCrateReference _level;

		public readonly LevelCrate Level;

		private readonly LevelCrateReference _loadLevel;

		public readonly LevelCrate LoadLevel;

		private readonly SceneLoadQueue _persistentQueue;

		private Action _doLevelLoad;

		private Action _doLevelUnload;

		private Action _willPersistentSceneLoad;

		private Action _doPersistentSceneLoad;

		private bool acceptingPlacers;

		private List<SpawnableCratePlacer> _initialPlacers;

		private List<UniTask> _initialPlacings;

		private List<PlayerMarker> _playerMarkers;

		public StreamStatus Status { get; private set; }

		public ChunkLoader ChunkLoader { get; private set; }

		public StreamSession(LevelCrateReference level, LevelCrateReference loadLevel = default(LevelCrateReference))
			: base()
		{
		}

		public void AddDoPersistentLoadCallback(Action cb)
		{
		}

		public void AddWillPersistentLoadCallback(Action cb)
		{
		}

		public void AddDoLevelLoadCallback(Action cb)
		{
		}

		public void AddDoLevelUnloadCallback(Action cb)
		{
		}

		public UniTask Load()
		{
			return default(UniTask);
		}

		public void End()
		{
		}

		public StreamSession Refresh()
		{
			return null;
		}

		public void RegisterInitialPlacer(SpawnableCratePlacer placer)
		{
		}

		public void RegisterPlayerMarker(PlayerMarker playerMarker)
		{
		}

		private Chunk WarmupTriggersReturnPlayerChunk()
		{
			return null;
		}

		private UniTask LoadPersistentScenes(bool autoActivate = true)
		{
			return default(UniTask);
		}

		private UniTask UnloadAllOtherScenes(Scene loadingScene)
		{
			return default(UniTask);
		}
	}
}
