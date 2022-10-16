using System.Collections.Generic;
using UnityEngine;

namespace SLZ.Marrow.SceneStreaming
{
	public class ChunkTrackerGroup : MonoBehaviour
	{
		private List<ChunkTracker> _trackers;

		public HashSet<ChunkTracker> _frozenTrackers;

		public void Add(ChunkTracker tracker)
		{
		}

		public void Remove(ChunkTracker tracker)
		{
		}

		public void Freeze(ChunkTracker tracker)
		{
		}

		public void Unfreeze(ChunkTracker tracker)
		{
		}

		public ChunkTrackerGroup()
			: base()
		{
		}
	}
}
