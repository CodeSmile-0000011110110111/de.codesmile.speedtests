// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Linq;
using Unity.PerformanceTesting;

namespace CodeSmileEditor.Tests
{
	public class ArrayMemCopyTests
	{
		private const Int32 IterationCount = 100;
		private const Int32 ArrayLength = 65535;

		[Test, Performance] public void ForLoop_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					for (var i = 0; i < ArrayLength; i++)
						dest[i] = source[i];
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ArrayCopy_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					Array.Copy(source, dest, ArrayLength);
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void CopyTo_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					// internally calls Array.Copy
					source.CopyTo(dest, 0);
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void Clone_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					dest = (Int64[])source.Clone();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void LinqSelect_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					// this is borderline insane but let's measure it anyway
					dest = source.Select(l => l).ToArray();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void BufferBlockCopy_Int64Array()
		{
			var source = new Int64[ArrayLength];
			var dest = new Int64[ArrayLength];

			Measure.Method(() =>
				{
					Buffer.BlockCopy(source, 0, dest, 0, ArrayLength);
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}
	}
}
