using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Platform.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class WpfMenuItem<C, W> : WidgetHandler<C, W>, IMenuActionItem, swi.ICommand
		where C : swc.MenuItem
		where W : MenuActionItem
	{
        Image image;
		swi.RoutedCommand command = new swi.RoutedCommand ();
		bool openingHandled;

		protected void Setup ()
		{
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Icon = image.ToWpfImage (16);
			}
		}

		public string Text
		{
			get { return Conversions.ConvertMneumonicFromWPF (Control.Header); }
			set { Control.Header = value.ToWpfMneumonic (); }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Key Shortcut
		{
			get
			{
				var keyBinding = Control.InputBindings.OfType<swi.KeyBinding> ().FirstOrDefault ();
				if (keyBinding != null)
					return KeyMap.Convert (keyBinding.Key, keyBinding.Modifiers);
				return Key.None;
			}
			set
			{
				Control.InputBindings.Clear ();
				if (value != Key.None) {
					var key = KeyMap.ConvertKey (value);
					var modifier = KeyMap.ConvertModifier (value);
					Control.InputBindings.Add (new swi.KeyBinding { Key = key, Modifiers = modifier, Command = this });
					Control.InputGestureText = value.ToShortcutString ();
				}
				else
					Control.InputGestureText = null;
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				OnCanExecuteChanged (EventArgs.Empty);
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case MenuActionItem.ValidateEvent:
				// handled by parent
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert (index, item.ControlObject);
			if (!openingHandled) {
				Control.SubmenuOpened += HandleContextMenuOpening;
				openingHandled = true;
			}
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove (item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}

		bool swi.ICommand.CanExecute (object parameter)
		{
			return this.Enabled;
		}

		void HandleContextMenuOpening (object sender, sw.RoutedEventArgs e)
		{
			var submenu = Widget as ISubMenuWidget;
			if (submenu != null) {
				foreach (var item in submenu.MenuItems.OfType<MenuActionItem>()) {
					item.OnValidate (EventArgs.Empty);
				}
			}
		}


		public event EventHandler CanExecuteChanged;

		protected virtual void OnCanExecuteChanged (EventArgs e)
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged (this, e);
		}

		void swi.ICommand.Execute (object parameter)
		{
			Widget.OnClick (EventArgs.Empty);
		}
	}
}
