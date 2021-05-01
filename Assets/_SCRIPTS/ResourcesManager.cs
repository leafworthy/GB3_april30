using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _SCRIPTS
{
	public enum ResourceType
	{
		health,
		mana,
		wood,
		none,
		manaPotions
	}
	public class ResourcesManager
	{
		[SerializeField] private Dictionary<ResourceType, Resource> resourcesDictionary = new Dictionary<ResourceType, Resource>();

		public System.Action<Resource> OnResourceEmpty;
		public System.Action<Resource> OnResourceFull;
		public System.Action<Resource, float> OnResourceChange;

		public ResourcesManager(List<Resource> resources)
		{
			foreach (Resource resource in resources)
			{
				AddResource(resource);
			}
		}

		public void AddResource(Resource resource)
		{
			if (resourcesDictionary.ContainsKey(resource.type))
			{
				Resource tempResource = GetResource(resource.type);
				tempResource.Add(resource);
				if (resource.amount == resource.max)
				{
					OnResourceFull?.Invoke(resource);
				}
				SetResource(tempResource);
			}
			else
			{
				resourcesDictionary.Add(resource.type, resource);
			}
			OnResourceChange?.Invoke(resource, resource.amount);
		}

		private void SetResource(Resource resource)
		{
			Resource tempResource = null;
			resourcesDictionary.TryGetValue(resource.type, out tempResource);
			if (tempResource != null)
			{
				tempResource = resource;
			}
			else
			{
				resourcesDictionary.Add(resource.type, resource);
			}
		}

		public void ChangeResource(ResourceType type, float amount)
		{
			if (amount > 0)
			{
				AddResource(type, amount);
			}
			else
			{
				RemoveResource(type, amount);
			}
		}

		private void RemoveResource(Resource resource)
		{
			if (resourcesDictionary.ContainsKey(resource.type))
			{
				Resource tempResource = GetResource(resource.type);
				tempResource.Remove(resource);
				if (resource.amount == resource.max)
				{
					OnResourceEmpty?.Invoke(resource);
				}
				SetResource(tempResource);
			}
			else
			{
				Debug.Log("don't have it");
			}
		}
		private void RemoveResource(ResourceType type, float amount)
		{
			RemoveResource(new Resource(amount, amount, type));
		}
		private void AddResource(ResourceType type, float amount)
		{
			AddResource(new Resource(amount, amount, type));
		}

		private Resource GetResource(ResourceType type)
		{
			Resource resource = null;
			resourcesDictionary.TryGetValue(type, out resource);
			return resource;
		}

		public float GetResourceAmount(ResourceType type)
		{
			return GetResource(type).amount;
		}
		public float GetResourceMax(ResourceType type)
		{
			return GetResource(type).max;
		}


		public void Refill()
		{
			foreach (KeyValuePair<ResourceType, Resource> resource in resourcesDictionary)
			{
				resource.Value.Refill();
			}
		}
	}
}