using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[UpdateAfter(typeof(CullingSystem))]
	public partial class RenderingSystem : SystemBase
	{
		static Matrix4x4[] MatrixCache;
		CullingSystem CullingSystem;

		protected override void OnCreate()
		{
			CullingSystem = World.GetExistingSystem<CullingSystem>();
		}

		protected override void OnUpdate()
		{
			var renderMeshes = RendererData.RenderMeshList;
			if(renderMeshes.Count == 0)
				return;
			
			var matricesByRenderMeshIndex = CullingSystem.MatricesByRenderMeshIndex;
			var renderMeshCount = matricesByRenderMeshIndex.Length;

			for(int renderMeshIndex = 0; renderMeshIndex < renderMeshCount; renderMeshIndex++)
			{
				var matrices = matricesByRenderMeshIndex[renderMeshIndex];
				var drawCount = matrices.Length;
				if(drawCount == 0)
					continue;
				
				const int maxDrawCountPerBatch = 1023;
				var renderMesh = renderMeshes[renderMeshIndex];
				var fullBatchCount = drawCount / maxDrawCountPerBatch;
				var lastBatchDrawCount = drawCount % maxDrawCountPerBatch;
				ReadOnlySpan<LocalToWorld> localToWorldSlice;
				int batchIndex;

				for(batchIndex = 0; batchIndex < fullBatchCount; batchIndex++)
				{
					localToWorldSlice =
						matrices.AsReadOnlySpan(batchIndex * maxDrawCountPerBatch, maxDrawCountPerBatch);
					DrawMeshInstanced(renderMesh, localToWorldSlice);
				}

				localToWorldSlice = matrices.AsReadOnlySpan(batchIndex * maxDrawCountPerBatch, lastBatchDrawCount);
				DrawMeshInstanced(renderMesh, localToWorldSlice);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void DrawMeshInstanced(in RenderMesh renderMesh, ReadOnlySpan<LocalToWorld> matrices)
		{
			var typeCast = MemoryMarshal.Cast<LocalToWorld, Matrix4x4>(matrices);
			typeCast.CopyTo(MatrixCache);
			Graphics.DrawMeshInstanced(renderMesh.Mesh, renderMesh.SubMeshIndex, renderMesh.Material, MatrixCache,
				matrices.Length);
		}
	}
}