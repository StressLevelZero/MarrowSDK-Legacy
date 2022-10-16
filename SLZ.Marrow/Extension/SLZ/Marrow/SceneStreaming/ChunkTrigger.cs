using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace SLZ.Marrow.SceneStreaming
{
	[RequireComponent(typeof(BoxCollider))]
	public class ChunkTrigger : MonoBehaviour
	{
		public Chunk chunk;

		public BoxCollider trigger;

		public bool ignorePlayer;

		public UnityEvent OnChunkLoaded;

		public UnityEvent OnChunkUnloaded;

		[SerializeField]
		private LayerMask _layers;

		[HideInInspector]
		public bool isActive;

		private List<ChunkTracker> _trackers;

		private HashSet<ChunkTracker> _trackersSet;

		public StreamStatus Status { get; private set; }

		private void Reset()
		{
		}

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		private void OnTriggerEnter(Collider other)
		{
		}

		private void OnTriggerExit(Collider other)
		{
		}

		public void RemoveTracker(ChunkTracker tracker)
		{
		}

		public bool WarmupHasPlayer()
		{
			return default(bool);
		}

		private void HandleTriggerEnter(Collider other)
		{
		}

		public void OnLoad()
		{
		}

		public void OnUnload()
		{
		}

		public ChunkTrigger()
			: base()
		{
		}
	}
}
