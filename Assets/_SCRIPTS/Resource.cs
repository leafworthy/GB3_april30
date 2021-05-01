using UnityEngine;

namespace _SCRIPTS
{
	[System.Serializable]
	public class Resource
	{
		public float amount;
		public float max;
		public ResourceType type;

		public Resource(float amount, float max, ResourceType type)
		{
			this.amount = amount;
			this.max = max;
			this.type = type;
		}

		public void Add(float add)
		{
			amount += add;
			amount = Mathf.Min(amount, max);
			amount = Mathf.Max(amount, 0f);
		}

		public void SetMax(float newMax)
		{
			max = newMax;
		}

		public void Add(Resource addedResource)
		{
			if (addedResource.type == type)
			{
				Add(Mathf.Abs(addedResource.amount));
			}
			else
			{
				Debug.Log("wrong type added");

			}

		}

		public void Remove(Resource removedResource)
		{
			if (removedResource.type == type)
			{
				Add(-Mathf.Abs(removedResource.amount));
			}
			else
			{
				Debug.Log("wrong type removed");

			}

		}

		public void Refill()
		{
			amount = max;
		}

	}
}

