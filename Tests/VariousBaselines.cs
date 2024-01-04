// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using Unity.PerformanceTesting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
	public class VariousBaselines
	{
		[Test] [Performance] public void Measure_EmptyCodeBlock() =>
			Measure.Method(() => {}).DynamicMeasurementCount().Run();

		[Test] [Performance] public void Measure_DebugLog_TestMessage() =>
			Measure.Method(() => { Debug.Log("test"); }).DynamicMeasurementCount().Run();


	}
}
