using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	public interface IRecyle
	{

		void Restart();

		void Shutdown();

	}

	public class RecycleGameObject : MonoBehaviour
	{
		private GameObject poolObj;


		public void setPool(GameObject p)
		{
			poolObj = p;
		}

		public GameObject getPool()
		{
			return poolObj;
		}



		public void Restart()
		{
			gameObject.SetActive(true);

		}

		public void Shutdown()
		{
			gameObject.SetActive(false);


		}

	}
}
