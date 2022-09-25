using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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
		public static Span<T> AsSpan<T>(this UnsafeList<T> list, int startIndex, int length) where T : unmanaged
		{
			return GetSpan(list.Ptr, startIndex, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this UnsafeList<T> list, int startIndex, int length)
			where T : unmanaged
		{
			return GetReadOnlySpan(list.Ptr, startIndex, length);
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
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T2> Reinterpret<T1, T2>(this ReadOnlySpan<T1> items) where T2 : unmanaged where T1 : unmanaged
		{
			Debug.Assert(UnsafeUtility.SizeOf<T1>() == UnsafeUtility.SizeOf<T2>());
			return MemoryMarshal.Cast<T1, T2>(items);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<TTo> Reinterpret<TFrom, TTo>(this Span<TFrom> items) where TTo : unmanaged where TFrom : unmanaged
		{
			Debug.Assert(UnsafeUtility.SizeOf<TFrom>() == UnsafeUtility.SizeOf<TTo>());
			return MemoryMarshal.Cast<TFrom, TTo>(items);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo Reinterpret<TFrom, TTo>(ref TFrom from) where TFrom : unmanaged where TTo : unmanaged
		{
			Debug.Assert(UnsafeUtility.SizeOf<TFrom>() == UnsafeUtility.SizeOf<TTo>());
			return ref Unsafe.As<TFrom, TTo>(ref from);
		}

	}
}