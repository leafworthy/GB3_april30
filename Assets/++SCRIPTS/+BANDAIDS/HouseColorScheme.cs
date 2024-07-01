using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS._BANDAIDS
{
	[CreateAssetMenu(menuName = "My Assets/Color Scheme")]
	public class HouseColorScheme : ScriptableObject
	{
		[FormerlySerializedAs("purple")] public Color walls_exterior;
		[FormerlySerializedAs("blue")] public Color walls_interior;
		[FormerlySerializedAs("red")] public Color rug_main_room;
		[FormerlySerializedAs("green")] public Color rug_side_room;
		[FormerlySerializedAs("white")] public Color walls_side_room;
		public Color kitchen_counters;
		public Color kitchen_table;

		public Color GetColor(ColorType colorType)
		{
			switch (colorType)
			{
				case ColorType.walls_exterior:
					return walls_exterior;
				case ColorType.walls_interior:
					return walls_interior;
				case ColorType.rug_main_room:
					return rug_main_room;
				case ColorType.rug_side_room:
					return rug_side_room;
				case ColorType.walls_side_room:
					return walls_side_room;
				case ColorType.kitchen_counters:
					return kitchen_counters;
				case ColorType.kitchen_table:
					return kitchen_table;
			}

			return Color.white;
		}

		public enum ColorType
		{
			walls_exterior,
			walls_interior,
			rug_main_room,
			rug_side_room,
			walls_side_room,
			kitchen_counters,
			kitchen_table
		}
	}
}