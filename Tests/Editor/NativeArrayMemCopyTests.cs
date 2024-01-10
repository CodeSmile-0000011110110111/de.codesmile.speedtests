// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.PerformanceTesting;

namespace CodeSmileEditor.Tests
{
	public class NativeArrayMemCopyTests
	{
		private const Int32 IterationCount = 100;
		private const Int32 ArrayLength = 65535;
		private const Int32 ParallelJobBatchCount = 16;

		/// <summary>
		///     NOTE: With safety checks enabled this way of copying will be far slower than managed array for loops.
		/// </summary>
		[Test, Performance] public void ForLoop()
		{
			var source = new NativeArray<Int64>(ArrayLength, Allocator.Temp);
			var dest = new NativeArray<Int64>(ArrayLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			Measure.Method(() =>
				{
					for (var i = 0; i < ArrayLength; i++)
						dest[i] = source[i];
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void CopyTo()
		{
			var source = new NativeArray<Int64>(ArrayLength, Allocator.Temp);
			var dest = new NativeArray<Int64>(ArrayLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			Measure.Method(() =>
				{
					// Note: CopyFrom is the exact same thing, just with source/dest flipped
					source.CopyTo(dest);
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void CopyTo_DirectMemCpy_NoSafetyChecks()
		{
			var source = new NativeArray<Int64>(ArrayLength, Allocator.Temp);
			var dest = new NativeArray<Int64>(ArrayLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			Measure.Method(() =>
				{
					unsafe
					{
						var sourcePtr = source.GetUnsafeReadOnlyPtr();
						var destPtr = dest.GetUnsafePtr();
						UnsafeUtility.MemCpy(destPtr, sourcePtr, ArrayLength * UnsafeUtility.SizeOf<Int64>());
					}
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ForLoop_IJob()
		{
			CopyJob job = default;

			Measure.Method(() =>
				{
					job.Schedule().Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyJob { source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ForLoop_IJobBurstCompiled()
		{
			CopyJobBurstCompiled job = default;

			Measure.Method(() =>
				{
					job.Schedule().Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyJobBurstCompiled { source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ForLoop_IJobBurstCompiled_DisabledSafety()
		{
			CopyJobBurstCompiledDisabledSafety job = default;

			Measure.Method(() =>
				{
					job.Schedule().Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyJobBurstCompiledDisabledSafety { source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ForLoop_IParallelJobBurstCompiled()
		{
			CopyParallelJobBurstCompiled job = default;

			Measure.Method(() =>
				{
					job.Schedule(ArrayLength, ParallelJobBatchCount).Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyParallelJobBurstCompiled { source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance] public void ForLoop_IParallelJobBurstCompiled_ReadWriteAttributes()
		{
			CopyParallelJobBurstCompiledReadWriteAttributes job = default;

			Measure.Method(() =>
				{
					job.Schedule(ArrayLength, ParallelJobBatchCount).Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyParallelJobBurstCompiledReadWriteAttributes { source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance]
		public void ForLoop_IParallelJobBurstCompiled_ReadWriteAttribs_DisabledSafety()
		{
			CopyParallelJobBurstCompiledReadWriteAttributesDisabledSafety job = default;

			Measure.Method(() =>
				{
					job.Schedule(ArrayLength, ParallelJobBatchCount).Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyParallelJobBurstCompiledReadWriteAttributesDisabledSafety
						{ source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		[Test, Performance]
		public void ForLoop_IParallelJobBurstCompiled_ReadWriteAttribs_DisabledSafety_HighBatchCount()
		{
			CopyParallelJobBurstCompiledReadWriteAttributesDisabledSafety job = default;

			Measure.Method(() =>
				{
					job.Schedule(ArrayLength, 4096).Complete();
				})
				.SetUp(() =>
				{
					var source = new NativeArray<Int64>(ArrayLength, Allocator.TempJob);
					var dest = new NativeArray<Int64>(ArrayLength, Allocator.TempJob,
						NativeArrayOptions.UninitializedMemory);
					job = new CopyParallelJobBurstCompiledReadWriteAttributesDisabledSafety
						{ source = source, dest = dest };
				})
				.CleanUp(() =>
				{
					job.source.Dispose();
					job.dest.Dispose();
				})
				.DynamicMeasurementCount()
				.IterationsPerMeasurement(IterationCount)
				.Run();
		}

		private struct CopyJob : IJob
		{
			public NativeArray<Int64> source;
			public NativeArray<Int64> dest;

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}

		[BurstCompile]
		private struct CopyJobBurstCompiled : IJob
		{
			public NativeArray<Int64> source;
			public NativeArray<Int64> dest;

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}

		[BurstCompile(DisableSafetyChecks = true)]
		private struct CopyJobBurstCompiledDisabledSafety : IJob
		{
			public NativeArray<Int64> source;
			public NativeArray<Int64> dest;

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}

		[BurstCompile]
		private struct CopyParallelJobBurstCompiled : IJobParallelFor
		{
			public NativeArray<Int64> source;
			public NativeArray<Int64> dest;

			public void Execute(Int32 index) => dest[index] = source[index];

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}

		[BurstCompile]
		private struct CopyParallelJobBurstCompiledReadWriteAttributes : IJobParallelFor
		{
			[ReadOnly] public NativeArray<Int64> source;
			[WriteOnly] public NativeArray<Int64> dest;

			public void Execute(Int32 index) => dest[index] = source[index];

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}

		[BurstCompile(DisableSafetyChecks = true)]
		private struct CopyParallelJobBurstCompiledReadWriteAttributesDisabledSafety : IJobParallelFor
		{
			[ReadOnly] public NativeArray<Int64> source;
			[WriteOnly] public NativeArray<Int64> dest;

			public void Execute(Int32 index) => dest[index] = source[index];

			public void Execute()
			{
				for (var i = 0; i < ArrayLength; i++)
					dest[i] = source[i];
			}
		}
	}
}
