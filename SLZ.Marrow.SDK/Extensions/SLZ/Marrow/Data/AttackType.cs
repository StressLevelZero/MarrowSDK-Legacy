using System;

namespace SLZ.Marrow.Data
{
	[Flags]
	public enum AttackType
	{
		Piercing = 1,
		Blunt = 2,
		Explosive = 4,
		Fire = 8,
		Slicing = 0x10,
		Stabbing = 0x20,
		None = 0x40,
		Ice = 0x80,
		Electric = 0x100
	}
}
