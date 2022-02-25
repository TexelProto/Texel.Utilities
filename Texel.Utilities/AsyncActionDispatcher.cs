using System;
using System.Threading.Tasks;

namespace Texel.Utilities
{
	public struct Unit { }

	public class AsyncActionDispatcher : AsyncEventDispatcher<Action, Unit>
	{
		public void Invoke() => this.Invoke( default );
		public Task InvokeAsync() => this.InvokeAsync( default );

		public void InvokeReverse() => this.InvokeReverse( default );
		public Task InvokeReverseAsync() => this.InvokeReverseAsync( default );

		protected override void InvokeListener(Action del, Unit data)
		{
			del();
		}
	}
}