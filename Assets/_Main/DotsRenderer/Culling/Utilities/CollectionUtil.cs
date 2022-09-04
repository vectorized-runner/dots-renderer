using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DotsRenderer
{
	public static unsafe class CollectionUtil
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T* GetTypedPtr<T>(this NativeArray<T> array) where T : unmanaged
		{
			return (T*)array.GetUnsafePtr();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T* GetTypedPtr<T>(this NativeList<T> list) where T : unmanaged
		{
			return (T*)list.GetUnsafePtr();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T ElementAsRef<T>(this NativeList<T> list, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(list.GetUnsafePtr(), index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T ElementAsReadonlyRef<T>(this NativeList<T> list, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(list.GetUnsafeReadOnlyPtr(), index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T ElementAsRef<T>(this NativeArray<T> array, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T ElementAsReadonlyRef<T>(this NativeArray<T> array, int index) where T : unmanaged
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafeReadOnlyPtr(), index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this NativeList<T> list, int startIndex, int length) where T : unmanaged
		{
			return GetSpan(list.GetTypedPtr(), startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeList<T> list, int startIndex, int length)
			where T : unmanaged
		{
			return GetReadOnlySpan(list.GetTypedPtr(), startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this NativeArray<T> array, int startIndex, int length) where T : unmanaged
		{
			return GetSpan(array.GetTypedPtr(), startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeArray<T> array, int startIndex, int length)
			where T : unmanaged
		{
			return GetReadOnlySpan(array.GetTypedPtr(), startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> GetReadOnlySpan<T>(T* ptr, int startIndex, int length) where T : unmanaged
		{
			return new ReadOnlySpan<T>(ptr + startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> GetSpan<T>(T* ptr, int startIndex, int length) where T : unmanaged
		{
			return new Span<T>(ptr + startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this NativeArray<T> array) where T : unmanaged
		{
			return AsSpan(array, 0, array.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeArray<T> array) where T : unmanaged
		{
			return AsReadOnlySpan(array, 0, array.Length);
		}
	}
}