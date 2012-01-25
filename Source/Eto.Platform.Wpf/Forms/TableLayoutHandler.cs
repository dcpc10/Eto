﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;

namespace Eto.Platform.Wpf.Forms
{
	public class TableLayoutHandler : WpfLayout<swc.Grid, TableLayout>, ITableLayout
	{
		Eto.Drawing.Size spacing;
		sw.Style itemStyle;
		public void CreateControl (int cols, int rows)
		{
			Control = new swc.Grid ();
			//Control.Background = System.Windows.Media.Brushes.Blue;
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add (new swc.ColumnDefinition {
				Width = new System.Windows.GridLength (1, i == cols - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add (new swc.RowDefinition {
				Height = new System.Windows.GridLength (1, i == rows - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
		}

		public void SetColumnScale (int column, bool scale)
		{
			Control.ColumnDefinitions[column].Width = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
			if (scale) {
				var controls = Control.Children.Cast<sw.FrameworkElement> ().Where (r => swc.Grid.GetColumn (r) == column);
				foreach (var control in controls) {
					control.Width = double.NaN;
				}
			}
		}

		public void SetRowScale (int row, bool scale)
		{
			Control.RowDefinitions[row].Height = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
			if (scale) {
				var controls = Control.Children.Cast<sw.FrameworkElement> ().Where (r => swc.Grid.GetRow (r) == row);
				foreach (var control in controls) {
					control.Height = double.NaN;
				}
			}
		}

		public Eto.Drawing.Size Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
				if (itemStyle == null) {
					itemStyle = new sw.Style (typeof (sw.FrameworkElement));
				}
				itemStyle.Setters.Clear ();
				itemStyle.Setters.Add (new sw.Setter (sw.FrameworkElement.MarginProperty, new sw.Thickness (0, 0, value.Width, value.Height)));
				itemStyle.Setters.Add (new sw.Setter (sw.FrameworkElement.VerticalAlignmentProperty, sw.VerticalAlignment.Stretch));
				itemStyle.Setters.Add (new sw.Setter (sw.FrameworkElement.HorizontalAlignmentProperty, sw.HorizontalAlignment.Stretch));
				//itemStyle.Setters.Add (new W.Setter (W.FrameworkElement.WidthProperty, W.FrameworkElement.WidthProperty.DefaultMetadata.DefaultValue));
				//itemStyle.Setters.Add (new W.Setter (W.FrameworkElement.HeightProperty, W.FrameworkElement.HeightProperty.DefaultMetadata.DefaultValue));
			}
		}

		bool IsColumnScale (int column)
		{
			return Control.ColumnDefinitions[column].Width.GridUnitType == sw.GridUnitType.Star;
		}

		bool IsRowScale (int row)
		{
			return Control.RowDefinitions[row].Height.GridUnitType == sw.GridUnitType.Star;
		}

		void SetScale (sw.FrameworkElement c, int x, int y)
		{
			if (IsColumnScale (x)) c.Width = double.NaN;
			if (IsRowScale (y)) c.Height = double.NaN;
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public void Add (Control child, int x, int y)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			var c = control as sw.FrameworkElement;
			c.Style = itemStyle;
			c.SetValue (swc.Grid.ColumnProperty, x);
			c.SetValue (swc.Grid.RowProperty, y);
			SetScale (c, x, y);
			Control.Children.Add (c);
		}

		public void Move (Control child, int x, int y)
		{
			var control = (sw.FrameworkElement)child.ControlObject;
			control.SetValue (swc.Grid.ColumnProperty, x);
			control.SetValue (swc.Grid.RowProperty, y);
			SetScale (control, x, y);
		}

		public void Remove (Control child)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			Control.Children.Remove (control);
		}
	}
}
