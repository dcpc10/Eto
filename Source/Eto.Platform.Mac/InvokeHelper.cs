using System;
namespace Eto.Platform.Mac
{
		class InvokeHelper
		{
			public Delegate Delegate { get; set; }
			public object[] Args { get; set; }
			public object Return { get; private set; }
			
			public void Action()
			{
				this.Return = Delegate.DynamicInvoke(Args);
			}
		}
}

