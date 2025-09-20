using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
	private static readonly Dictionary<Type, object> _services = new();

	public static void Register<T>(T instance) where T : class
	{
		var type = typeof(T);

		_services[type] = instance;
	}

	public static T Get<T>() where T : MonoBehaviour
	{
		var type = typeof(T);

		if (_services.TryGetValue(type, out var service)) return (T) service;

		var findItem = UnityEngine.Object.FindFirstObjectByType<T>();
		if (findItem != null)
		{
			Register<T>(findItem);
			return findItem;
		}

		if (findItem == null)
		{
			var go = new GameObject(type.Name);
			var newService = go.AddComponent<T>();
			Register<T>(newService);
			 return newService;
		}

		throw new InvalidOperationException($"ServiceLocator: Service of type {type.Name} is not registered.");
	}


}
