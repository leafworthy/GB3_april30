using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public static class VectorUtilities
	{
		public static List<Vector2> GetPositionsWithinRadiusOfPoint(int numberOfPositions, Vector2 basePosition, float radius = 1)
		{
			var positions = new List<Vector2>();

			for (var i = 0; i < numberOfPositions; i++)
			{
				var angle = i * (360f / numberOfPositions) * Mathf.Deg2Rad;
				var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
				positions.Add(basePosition + offset);
			}

			return positions;
		}
	}

	public abstract class Timer
	{
		protected float initialTime;
		protected float Time { get; set; }
		public bool IsRunning { get; protected set; }

		public float Progress => Time / initialTime;

		public Action OnTimerStart = delegate { };
		public Action OnTimerStop = delegate { };

		protected Timer(float value)
		{
			initialTime = value;
			IsRunning = false;
		}

		public void Start()
		{
			Time = initialTime;
			if (!IsRunning)
			{
				IsRunning = true;
				OnTimerStart.Invoke();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				OnTimerStop.Invoke();
			}
		}

		public void Resume() => IsRunning = true;
		public void Pause() => IsRunning = false;

		public abstract void Tick(float deltaTime);
	}

	public class CountdownTimer : Timer
	{
		public CountdownTimer(float value) : base(value)
		{
		}

		public override void Tick(float deltaTime)
		{
			if (IsRunning && Time > 0)
			{
				Time -= deltaTime;
			}

			if (IsRunning && Time <= 0)
			{
				Stop();
			}
		}

		public bool IsFinished => Time <= 0;

		public void Reset() => Time = initialTime;

		public void Reset(float newTime)
		{
			initialTime = newTime;
			Reset();
		}
	}

	public class StopwatchTimer : Timer
	{
		public StopwatchTimer() : base(0)
		{
		}

		public override void Tick(float deltaTime)
		{
			if (IsRunning)
			{
				Time += deltaTime;
			}
		}

		public void Reset() => Time = 0;

		public float GetTime() => Time;
	}
}
