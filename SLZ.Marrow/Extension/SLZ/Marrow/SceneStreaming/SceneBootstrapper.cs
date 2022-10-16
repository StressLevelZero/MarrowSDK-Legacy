using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.SceneStreaming
{
	public class SceneBootstrapper : MonoBehaviour
	{
		public LevelCrateReference levelCrateRef;

		public LevelCrateReference loadLevelCrateRef;

		private UniTaskVoid Start()
		{
			return default(UniTaskVoid);
		}

		public SceneBootstrapper()
			: base()
		{
		}
	}
}
