using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TextAreaHandler : GtkControl<Gtk.TextView, TextArea>, ITextArea
	{
		bool sendSelectionChanged = true;
		Range? lastSelection;
		int? lastCaretIndex;
		Gtk.ScrolledWindow scroll;

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}
		
		public TextAreaHandler ()
		{
			scroll = new Gtk.ScrolledWindow ();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new Gtk.TextView ();
			this.Size = TextArea.DefaultSize;
			scroll.Add (Control);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				Control.Buffer.Changed += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			case TextArea.SelectionChangedEvent:
				Control.Buffer.MarkSet += (o, args) => {
					var selection = this.Selection;
					if (sendSelectionChanged && selection != lastSelection) {
						Widget.OnSelectionChanged (EventArgs.Empty);
						lastSelection = selection;
					}
				};
				break;
			case TextArea.CaretIndexChangedEvent:
				Control.Buffer.MarkSet += (o, args) => {
					var caretIndex = this.CaretIndex;
					if (sendSelectionChanged && caretIndex != lastCaretIndex) {
						Widget.OnCaretIndexChanged (EventArgs.Empty);
						lastCaretIndex = caretIndex;
					}
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override string Text {
			get { return Control.Buffer.Text; }
			set { Control.Buffer.Text = value; }
		}
		
		public bool ReadOnly {
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}
		
		public bool Wrap {
			get { return Control.WrapMode != Gtk.WrapMode.None; }
			set { Control.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			var end = Control.Buffer.EndIter;
			Control.Buffer.Insert (ref end, text);
			if (scrollToCursor) {
				var mark = Control.Buffer.CreateMark (null, end, false);
				Control.ScrollToMark (mark, 0, false, 0, 0);
			}
		}
		
		public string SelectedText
		{
			get {
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds (out start, out end)) {
					return Control.Buffer.GetText (start, end, false);
				}
				else return null;
			}
			set {
				sendSelectionChanged = false;
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds (out start, out end)) {
					var startOffset = start.Offset;
					Control.Buffer.Delete (ref start, ref end);
					if (value != null) {
						Control.Buffer.Insert (ref start, value);
						start = Control.Buffer.GetIterAtOffset (startOffset);
						end = Control.Buffer.GetIterAtOffset(startOffset + value.Length);
						Control.Buffer.SelectRange (start, end);
					}
				}
				else if (value != null)
					Control.Buffer.InsertAtCursor (value);
				Widget.OnSelectionChanged (EventArgs.Empty);
				sendSelectionChanged = true;
			}
		}

		public Range Selection
		{
			get {
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds (out start, out end))
					return new Range(start.Offset, end.Offset - start.Offset);
				else
					return new Range (Control.Buffer.CursorPosition, 0);
			}
			set {
				sendSelectionChanged = false;
				var start = Control.Buffer.GetIterAtOffset(value.Start);
				var end = Control.Buffer.GetIterAtOffset(value.Start + value.Length);
				Control.Buffer.SelectRange (start, end);
				Widget.OnSelectionChanged (EventArgs.Empty);
				sendSelectionChanged = true;
			}
		}

		public void SelectAll ()
		{
			Control.Buffer.SelectRange (Control.Buffer.StartIter, Control.Buffer.EndIter);
		}

		public int CaretIndex
		{
			get { return Control.Buffer.GetIterAtMark (Control.Buffer.InsertMark).Offset; }
			set {
				var ins = Control.Buffer.GetIterAtOffset (value);
				Control.Buffer.SelectRange(ins, ins);
			}
		}
	}
}
