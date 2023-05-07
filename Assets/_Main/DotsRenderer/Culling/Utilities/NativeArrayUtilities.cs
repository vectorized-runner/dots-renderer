using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DotsRenderer
{
	public static unsafe class NativeArrayUtilities
	{
		public static T* GetTypedPtr<T>(this NativeArray<T> array) where T : unmanaged
		{
			return (T*)array.GetUnsafePtr();
		}

		public static ref T ElementAsRef<T>(this NativeArray<T> array, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
		}

		public static ref readonly T ElementAsReadonlyRef<T>(this NativeArray<T> array, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafeReadOnlyPtr(), index);
		}

		public static Span<T> AsSpan<T>(this NativeArray<T> array, int startIndex, int length) where T : unmanaged
		{
			var typedPtr = (T*)array.GetUnsafePtr();
			var startAddress = typedPtr + startIndex;
			return new Span<T>(startAddress, length);
		}

		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeArray<T> array, int startIndex, int length)
			where T : unmanaged
		{
			var typedPtr = (T*)array.GetUnsafeReadOnlyPtr();
			var startAddress = typedPtr + startIndex;
			return new ReadOnlySpan<T>(startAddress, length);
		}

		public static Span<T> AsSpan<T>(this NativeArray<T> array) where T : unmanaged
		{
			return AsSpan(array, 0, array.Length);
		}

		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeArray<T> array) where T : unmanaged
		{
			return AsReadOnlySpan(array, 0, array.Length);
		}
	}
}