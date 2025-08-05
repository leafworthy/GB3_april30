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

	public static T Get<T>() where T : class
	{
		var type = typeof(T);

		if (_services.TryGetValue(type, out var service)) return (T) service;

		throw new InvalidOperationException($"ServiceLocator: Service of type {type.Name} is not registered.");
	}


}