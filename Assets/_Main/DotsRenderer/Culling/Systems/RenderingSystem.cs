using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[UpdateAfter(typeof(ChunkCullingSystem))]
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
			// TODO-Optimization: Find a way to not call complete on this. Even 1-frame delayed update is ok!
			CullingSystem.FinalJobHandle.Complete();
			
			var renderMeshes = RendererData.RenderMeshList;
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
				ReadOnlySpan<LocalToWorld> localToWorldSlice;
				int batchIndex;

				for(batchIndex = 0; batchIndex < fullBatchCount; batchIndex++)
				{
					localToWorldSlice = matrices.AsReadOnlySpan(batchIndex * MaxDrawCountPerBatch, MaxDrawCountPerBatch)
					                            .Reinterpret<float4x4, LocalToWorld>();
					DrawMeshInstanced(renderMesh, localToWorldSlice);
				}

				var lastBatchDrawCount = drawCount % MaxDrawCountPerBatch;
				if(lastBatchDrawCount > 0)
				{
					localToWorldSlice = matrices.AsReadOnlySpan(batchIndex * MaxDrawCountPerBatch, lastBatchDrawCount)
					                            .Reinterpret<float4x4, LocalToWorld>();
					DrawMeshInstanced(renderMesh, localToWorldSlice);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void DrawMeshInstanced(in RenderMesh renderMesh, ReadOnlySpan<LocalToWorld> matrices)
		{
			Debug.Assert(matrices.Length > 0);
			var typeCast = MemoryMarshal.Cast<LocalToWorld, Matrix4x4>(matrices);
			typeCast.CopyTo(MatrixCache);
			Graphics.DrawMeshInstanced(renderMesh.Mesh, renderMesh.SubMeshIndex, renderMesh.Material, MatrixCache,
				matrices.Length);
		}
	}
}