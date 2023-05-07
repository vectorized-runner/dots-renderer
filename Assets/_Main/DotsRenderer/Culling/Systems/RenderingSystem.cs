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
			// TODO-Renderer: Find a way to not call complete on this. Even 1-frame delayed update is ok!
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
				ReadOnlySpan<Matrix4x4> matrixSlice;
				int batchIndex;

				for(batchIndex = 0; batchIndex < fullBatchCount; batchIndex++)
				{
					matrixSlice = matrices.AsReadOnlySpan(batchIndex * MaxDrawCountPerBatch, MaxDrawCountPerBatch)
					                      .Reinterpret<float4x4, Matrix4x4>();
					DrawMeshInstanced(renderMesh, matrixSlice);
				}

				var lastBatchDrawCount = drawCount % MaxDrawCountPerBatch;
				if(lastBatchDrawCount > 0)
				{
					matrixSlice = matrices.AsReadOnlySpan(batchIndex * MaxDrawCountPerBatch, lastBatchDrawCount)
					                            .Reinterpret<float4x4, Matrix4x4>();
					DrawMeshInstanced(renderMesh, matrixSlice);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void DrawMeshInstanced(in RenderMesh renderMesh, ReadOnlySpan<Matrix4x4> matrices)
		{
			Debug.Assert(matrices.Length > 0);
			matrices.CopyTo(MatrixCache);
			Graphics.DrawMeshInstanced(renderMesh.Mesh, renderMesh.SubMeshIndex, renderMesh.Material, MatrixCache,
				matrices.Length);
		}
	}
}