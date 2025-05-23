﻿using System.Collections.Generic;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightCollider2D
{
	public class LightColliderLayer<T>
	{
		public List<T>[] layerList;

		public LightColliderLayer()
		{
			layerList = new List<T>[10];

			for(int i = 0; i < 10; i++)
			{
				layerList[i] = new List<T>();
			}
		}

		public int Update(int targetLayer, int newLayer, T obj)
		{
			if (targetLayer != newLayer)
			{
				if (targetLayer > -1)
				{
					layerList[targetLayer].Remove(obj);
				}

				targetLayer = newLayer;

				layerList[targetLayer].Add(obj);
			}

			return(targetLayer);
		}

		public void Remove(int targetLayer, T obj)
		{
			if (targetLayer > -1)
			{
				layerList[targetLayer].Remove(obj);
			}
		}
	}
}
