using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms
{
	
	
	public class DockLayoutHandler : MacLayout<NSView, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		Control childToAdd;

		public override NSView Control {
			get {
				return Widget.Container != null ? (NSView)Widget.Container.ContainerObject : null;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public Eto.Drawing.Padding Padding {
			get { return padding; }
			set {
				padding = value;
				if (Widget.Container != null)
					UpdateParentLayout ();
			}
		}
		
		public override Size GetPreferredSize (Size availableSize)
		{
			if (child != null)
			{
				return child.GetPreferredSize (availableSize) + Padding.Size;
			}
			else return Size.Empty;
		}

		public override void AttachedToContainer ()
		{
			base.AttachedToContainer ();
			if (childToAdd != null)
				this.Content = childToAdd;
			UpdateParentLayout ();
		}

		public override void LayoutChildren ()
		{
			if (child == null) return;
			
			NSView parent = this.Control;
			
			NSView childControl = child.GetContainerView ();
			var frame = parent.Frame;
			
			if (frame.Width > padding.Horizontal && frame.Height > padding.Vertical)
			{
				frame.X = padding.Left;
				frame.Width -= padding.Horizontal;
				frame.Y = padding.Bottom;
				frame.Height -= padding.Vertical;
			}
			else {
				frame.X = 0;
				frame.Y = 0;
			}
			
			if (childControl.Frame != frame)
				childControl.Frame = frame;
		}
				
		public Control Content {
			get {
				return this.child;
			}
			set {
				if (Widget.Container == null) {
					childToAdd = value;
					return;
				}

				if (this.child != null) { 
					this.child.GetContainerView ().RemoveFromSuperview(); 
				}
				if (value != null)
				{
					this.child = value;
					NSView childControl = child.GetContainerView ();
					childControl.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

					NSView parent = this.Control;
					parent.AddSubview(childControl);
				}
				else
					this.child = null;
				if (Widget.Loaded || Widget.Container.Loaded) {
					UpdateParentLayout ();
				}
			}
		}

		public override void OnLoadComplete ()
		{
			base.OnLoadComplete ();
			
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver(NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as DockLayoutHandler;
				handler.LayoutChildren();
			});
		}

		public override void SetContainerSize (SD.SizeF size)
		{
			size += Padding.Size.ToSDSizeF ();
			
			base.SetContainerSize (size);
		}
	}
}
