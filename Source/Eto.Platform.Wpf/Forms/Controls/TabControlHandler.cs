using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TabControlHandler : WpfControl<swc.TabControl, TabControl>, ITabControl
	{
		bool disableSelectedIndexChanged;
		public TabControlHandler ()
		{
			Control = new swc.TabControl ();
			Control.Loaded += delegate {
				Control.SelectionChanged += delegate {
					if (!disableSelectedIndexChanged)
						Widget.OnSelectedIndexChanged (EventArgs.Empty);
				};
			};
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public void InsertTab (int index, TabPage page)
		{
			if (index == -1)
				Control.Items.Add (page.ControlObject);
			else
				Control.Items.Insert (index, page.ControlObject);
			if (Widget.Loaded && Control.Items.Count == 1)
				SelectedIndex = 0;
		}

		public void ClearTabs ()
		{
			Control.Items.Clear ();
		}

		public void RemoveTab (int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try {
				Control.Items.Remove (page.ControlObject);
				if (Widget.Loaded)
					Widget.OnSelectedIndexChanged (EventArgs.Empty);
			} finally {
				disableSelectedIndexChanged = false;
			}
		}
	}
}
