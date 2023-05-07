using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DotsRenderer
{
	public unsafe struct UnsafeArray<T> where T : unmanaged
	{
		[NativeDisableUnsafePtrRestriction]
		public T* Ptr;

		public int Length;

		public Allocator Allocator;

		public UnsafeArray(T* ptr, int length, Allocator allocator)
		{
			Ptr = ptr;
			Length = length;
			Allocator = allocator;
		}

		public void Dispose()
		{
			// Can't use internal method, instead its inlined here
			// if(CollectionHelper.ShouldDeallocate(Allocator))
			if(Allocator > Allocator.None)
			{
				AllocatorManager.Free(Allocator, Ptr);
				Allocator = Allocator.Invalid;
			}

			Ptr = null;
			Length = 0;
		}
	}
}