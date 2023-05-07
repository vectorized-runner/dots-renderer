using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(CullingGroup), OrderLast = true)]
	public partial class RenderingSystem : SystemBase
	{
		static Matrix4x4[] MatrixCache;
		ChunkCullingSystem CullingSystem;
		
		const int MaxDrawCountPerBatch = 1023;

		protected override void OnCreate()
		{
			CullingSystem = World.GetExistingSystem<ChunkCullingSystem>();
			MatrixCache = new Matrix4x4[MaxDrawCountPerBatch];
		}

		protected override void OnUpdate()
		{
			// TODO-Renderer: Find a way to not call complete on this. Maybe 1 frame delayed rendering is ok?
			CullingSystem.FinalJobHandle.Complete();

			var renderMeshes = CullingSystem.RenderMeshes;
			if(renderMeshes.Count == 0)
				return;
			
			var matricesByRenderMeshIndex = CullingSystem.MatrixArrayByRenderMeshIndex;
			var renderMeshCount = matricesByRenderMeshIndex.Length;

			for(int renderMeshIndex = 0; renderMeshIndex < renderMeshCount; renderMeshIndex++)
			{
				var matrices = matricesByRenderMeshIndex[renderMeshIndex];
				var drawCount = matrices.Length;
				if(drawCount == 0)
					continue;
				
				var renderMesh = renderMeshes[renderMeshIndex];
				var fullBatchCount = drawCount / MaxDrawCountPerBatch;
				int batchIndex;

				for(batchIndex = 0; batchIndex < fullBatchCount; batchIndex++)
				{
					var span = matrices.AsSpan(batchIndex * MaxDrawCountPerBatch, MaxDrawCountPerBatch);
					var m4x4 = span.Reinterpret<float4x4, Matrix4x4>();
					AssertValidMatrices(m4x4);
					DrawMeshInstanced(renderMesh, m4x4);
				}
				
				var lastBatchDrawCount = drawCount % MaxDrawCountPerBatch;
				if(lastBatchDrawCount > 0)
				{
					var span = matrices.AsSpan(batchIndex * MaxDrawCountPerBatch, lastBatchDrawCount);
					var m4x4 = span.Reinterpret<float4x4, Matrix4x4>();
					AssertValidMatrices(m4x4);
					DrawMeshInstanced(renderMesh, m4x4);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void DrawMeshInstanced(in RenderMesh renderMesh, ReadOnlySpan<Matrix4x4> matrices)
		{
			Debug.Assert(matrices.Length > 0 && matrices.Length <= 1023);
			matrices.CopyTo(MatrixCache);
			Graphics.DrawMeshInstanced(renderMesh.Mesh, renderMesh.SubMeshIndex, renderMesh.Material, MatrixCache,
				matrices.Length);
		}

		public static void AssertValidMatrices(Span<Matrix4x4> matrices)
		{
			foreach (ref var matrix in matrices)
			{
				ref var f4x4 = ref Unsafe.As<Matrix4x4, float4x4>(ref matrix);
				AssertValid(f4x4.c3.xyz);
			}
		}

		public static void AssertValid(float3 position)
		{
			Debug.Assert(!math.any(math.isnan(position)));
			Debug.Assert(!math.any(math.isinf(position)));
		}
	}
}