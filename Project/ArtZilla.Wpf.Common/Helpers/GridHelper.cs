using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ArtZilla.Sharp.Lib.Extensions;

namespace ArtZilla.Wpf.Common.Helpers {
	public class GridHelper {
		#region RowCount Property

		/// <summary>
		/// Adds the specified number of Rows to RowDefinitions. 
		/// Default Height is Auto
		/// </summary>
		public static readonly DependencyProperty RowCountProperty =
				DependencyProperty.RegisterAttached(
						"RowCount", typeof(Int32), typeof(GridHelper),
						new PropertyMetadata(-1, RowCountChanged));

		// Get
		public static Int32 GetRowCount(DependencyObject obj) {
			return (Int32) obj.GetValue(RowCountProperty);
		}

		// Set
		public static void SetRowCount(DependencyObject obj, Int32 value) {
			obj.SetValue(RowCountProperty, value);
		}

		// Change Event - Adds the Rows
		public static void RowCountChanged(
				DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (!(obj is Grid) || (Int32) e.NewValue < 0)
				return;

			var grid = (Grid)obj;
			grid.RowDefinitions.Clear();

			for (var i = 0; i < (Int32) e.NewValue; i++)
				grid.RowDefinitions.Add(
						new RowDefinition { Height = GridLength.Auto });

			SetStarRows(grid);
		}

		#endregion

		#region ColumnCount Property

		/// <summary>
		/// Adds the specified number of Columns to ColumnDefinitions. 
		/// Default Width is Auto
		/// </summary>
		public static readonly DependencyProperty ColumnCountProperty =
				DependencyProperty.RegisterAttached(
						"ColumnCount", typeof(Int32), typeof(GridHelper),
						new PropertyMetadata(-1, ColumnCountChanged));

		// Get
		public static Int32 GetColumnCount(DependencyObject obj) {
			return (Int32) obj.GetValue(ColumnCountProperty);
		}

		// Set
		public static void SetColumnCount(DependencyObject obj, Int32 value) {
			obj.SetValue(ColumnCountProperty, value);
		}

		// Change Event - Add the Columns
		public static void ColumnCountChanged(
				DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (!(obj is Grid) || (Int32) e.NewValue < 0)
				return;

			var grid = (Grid)obj;
			grid.ColumnDefinitions.Clear();

			for (var i = 0; i < (Int32) e.NewValue; i++)
				grid.ColumnDefinitions.Add(
						new ColumnDefinition() { Width = GridLength.Auto });

			SetStarColumns(grid);
		}

		#endregion

		#region StarRows Property

		/// <summary>
		/// Makes the specified Row's Height equal to Star. 
		/// Can set on multiple Rows
		/// </summary>
		public static readonly DependencyProperty StarRowsProperty =
				DependencyProperty.RegisterAttached(
						"StarRows", typeof(String), typeof(GridHelper),
						new PropertyMetadata(String.Empty, StarRowsChanged));

		// Get
		public static String GetStarRows(DependencyObject obj) {
			return (String) obj.GetValue(StarRowsProperty);
		}

		// Set
		public static void SetStarRows(DependencyObject obj, String value) {
			obj.SetValue(StarRowsProperty, value);
		}

		// Change Event - Makes specified Row's Height equal to Star
		public static void StarRowsChanged(
				DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (!(obj is Grid) || String.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetStarRows((Grid) obj);
		}

		#endregion

		#region StarColumns Property

		/// <summary>
		/// Makes the specified Column's Width equal to Star. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty StarColumnsProperty =
				DependencyProperty.RegisterAttached(
						"StarColumns", typeof(String), typeof(GridHelper),
						new PropertyMetadata(String.Empty, StarColumnsChanged));

		// Get
		public static String GetStarColumns(DependencyObject obj) {
			return (String) obj.GetValue(StarColumnsProperty);
		}

		// Set
		public static void SetStarColumns(DependencyObject obj, String value) {
			obj.SetValue(StarColumnsProperty, value);
		}

		// Change Event - Makes specified Column's Width equal to Star
		public static void StarColumnsChanged(
				DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (!(obj is Grid) || String.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetStarColumns((Grid) obj);
		}

		#endregion

		private static void SetStarColumns(Grid grid) {
			var starColumns = GetStarColumns(grid).Split(',');
			var allStars = GetStarColumns(grid).Like("All");

			for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
				if (allStars || starColumns.Contains(i.ToString()))
					grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
		}

		private static void SetStarRows(Grid grid) {
			var starRows = GetStarRows(grid).Split(',');
			var allStars = GetStarRows(grid).Like("All");

			for (var i = 0; i < grid.RowDefinitions.Count; i++)
				if (allStars || starRows.Contains(i.ToString()))
					grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
		}
	}
}