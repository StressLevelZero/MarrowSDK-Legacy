using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.SceneStreaming
{
	[CreateAssetMenu(fileName = "Stream Chunk", menuName = "StressLevelZero/StreamChunk", order = 1)]
	public class Chunk : ScriptableObject
	{
		public MarrowScene[] sceneLayers;

		public Chunk[] linkedChunks;

		public Chunk()
			: base()
		{
		}
	}
}
