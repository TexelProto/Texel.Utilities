using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Texel.Utilities
{
	public abstract class AsyncEventDispatcher<TDel, TData>
		where TDel : Delegate
	{
		[ThreadStatic] private static List<TDel> delegateCopies;

		private readonly List<TDel> listeners = new();

		public void Register(TDel callback)
		{
			lock (this.listeners)
				this.listeners.Add( callback );
		}

		public void Unregister(TDel callback)
		{
			lock (this.listeners)
				this.listeners.Remove( callback );
		}

		public void Clear()
		{
			lock (this.listeners)
				this.listeners.Clear();
		}

		public void Invoke(TData data)
		{
			delegateCopies ??= new List<TDel>();
			delegateCopies.Clear();
			lock (this.listeners)
				delegateCopies.AddRange( this.listeners );

			this.InvokeAll( data, 0, delegateCopies.Count, 1 );
		}

		public void InvokeReverse(TData data)
		{
			delegateCopies ??= new List<TDel>();
			delegateCopies.Clear();
			lock (this.listeners)
				delegateCopies.AddRange( this.listeners );

			this.InvokeAll( data, delegateCopies.Count - 1, -1, -1 );
		}

		private void InvokeAll(TData data, int start, int end, int change)
		{
			List<Exception> exceptions = null;
			for (var i = start; i != end; i += change)
			{
				try
				{
					this.InvokeListener( delegateCopies[i], data );
				}
				catch (Exception e)
				{
					exceptions ??= new List<Exception>();
					exceptions.Add( e );
				}
			}

			if (exceptions != null)
				throw new AggregateException( exceptions );
		}

		public async Task InvokeAsync(TData data)
		{
			await Task.Yield();
			this.Invoke( data );
		}

		public async Task InvokeReverseAsync(TData data)
		{
			await Task.Yield();
			this.InvokeReverse( data );
		}

		protected abstract void InvokeListener(TDel del, TData data);
	}
}