using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/Color Scheme")]
	public class HouseColorScheme : ScriptableObject
	{
		[FormerlySerializedAs("purple")] public Color walls_exterior;
		[FormerlySerializedAs("blue")] public Color walls_interior;
		[FormerlySerializedAs("red")] public Color rug_main_room;
		[FormerlySerializedAs("green")] public Color rug_side_room;
		[FormerlySerializedAs("white")] public Color walls_side_room;
		public Color kitchen_floor;
		public Color kitchen_counters;
		public Color kitchen_table;
		public Color kitchen_fence;
		public Color bedroom_floor;
		public Color back_door;
		public Color kitchen_door;
		public Color front_door;
		public Color bedroom_door;
		public Color bathroom_door;
		public Color tv;
		public Color lampshade;
		public Color couch;

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
				 case  ColorType.kitchen_floor:
					return kitchen_floor;
				case ColorType.bedroom_floor:
					 return bedroom_floor;
				case ColorType.outer_fence:
					return kitchen_fence;
				case ColorType.back_door:
					return back_door;
				case ColorType.kitchen_door:
					return kitchen_door;
				case ColorType.front_door:
					return front_door;
				case ColorType.bedroom_door:
					return bedroom_door;
				case ColorType.bathroom_door:
					return bathroom_door;
				case ColorType.tv:
					return tv;
				case ColorType.lampshade:
					return lampshade;
				case ColorType.couch:
					return couch;
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
			kitchen_table,
			kitchen_floor,
			bedroom_floor,
			outer_fence,
			back_door,
			kitchen_door,
			front_door,
			bedroom_door,
			bathroom_door,
			tv,
			lampshade,
			couch
		}
	}
}
