using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "Animation Curve Data", menuName = "Variables/Animation Curve", order = 2)]
	public class CurveData : ScriptableObject
	{
		public AnimationCurve value;

		public CurveData()
			: base()
		{
		}
	}
}
