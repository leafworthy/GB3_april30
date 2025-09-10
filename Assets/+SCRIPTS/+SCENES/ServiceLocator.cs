using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
	private static readonly Dictionary<Type, object> _services = new();

	public static void Register<T>(T instance) where T : class
	{
		var type = typeof(T);
		if (_services.ContainsKey(type)) Debug.LogWarning($"ServiceLocator: Service of type {type.Name} is already registered. Overwriting...");

		_services[type] = instance;
		Debug.Log($"ServiceLocator: Service of type {type.Name} registered successfully.");
	}

	public static T Get<T>() where T : MonoBehaviour
	{
		var type = typeof(T);

		if (_services.TryGetValue(type, out var service)) return (T) service;

		var findItem = UnityEngine.Object.FindFirstObjectByType<T>();
		if (findItem != null)
		{
			Register<T>(findItem);
			Debug.Log("found item by type: " + type.Name);
			return findItem;
		}

		if (findItem == null)
		{
			Debug.Log("made new item by type: " + type.Name);
			var go = new GameObject(type.Name);
			var newService = go.AddComponent<T>();
			Register<T>(newService);
			 return newService;
		}

		throw new InvalidOperationException($"ServiceLocator: Service of type {type.Name} is not registered.");
	}


}
