using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DotsRenderer
{
    [BurstCompile]
    public struct ChunkCullingJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<RenderMeshIndex> RenderMeshIndexHandle;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;
        [ReadOnly] public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsHandle;
        [ReadOnly] public ComponentTypeHandle<ChunkWorldRenderBounds> ChunkWorldRenderBoundsHandle;
        [ReadOnly] public NativeArray<Plane> FrustumPlanes;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            throw new System.NotImplementedException();
        }
    }

    public partial class ChunkCullingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}