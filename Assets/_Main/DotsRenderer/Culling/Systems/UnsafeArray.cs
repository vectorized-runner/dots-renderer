using System;
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

		public T this[int index]
		{
			get => UnsafeUtility.ReadArrayElement<T>(Ptr, index);
			set => UnsafeUtility.WriteArrayElement(Ptr, index, value);
		}

		public ref T ElementAsRef(int index) => ref UnsafeUtility.ArrayElementAsRef<T>(Ptr, index);

		public Span<T> AsSpan(int startIndex, int length)
		{
			return CollectionUtil.GetSpan(Ptr, startIndex, length);
		}

		public ReadOnlySpan<T> AsReadOnlySpan(int startIndex, int length)
		{
			return CollectionUtil.GetSpan(Ptr, startIndex, length);
		}

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