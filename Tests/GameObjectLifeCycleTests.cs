// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace CodeSmileEditor.Tests
{
	public class GameObjectLifeCycleTests
	{
		private const int IterationCount = 10000;

		[Test, Performance] public void Measure_GameObject_New()
		{
			GameObject go = null;
			Measure.Method(() =>
				{
					go = new GameObject();
				})
				.CleanUp(() => { Object.DestroyImmediate(go); })
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void Measure_GameObject_DestroyImmediate()
		{
			GameObject go = null;
			Measure.Method(() =>
				{
					Object.DestroyImmediate(go);
				})
				.SetUp(() => { go = new GameObject(); })
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void Measure_GameObject_NewAndDestroyImmediate() => Measure.Method(() =>
			{
				var go = new GameObject();
				Object.DestroyImmediate(go);
			})
			.DynamicMeasurementCount()
			.IterationsPerMeasurement(IterationCount)
			.Run();

		[Test, Performance] public void Measure_GameObject_Instantiate()
		{
			var go = new GameObject();
			GameObject dupe = null;
			Measure.Method(() =>
				{
					dupe = Object.Instantiate(go);
				})
				.CleanUp(() => { Object.DestroyImmediate(dupe); })
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}
	}
}
