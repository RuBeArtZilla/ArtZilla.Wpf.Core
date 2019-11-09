using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ArtZilla.Net.Core.Extensions;

namespace ArtZilla.Wpf.Helpers {
	public static class GridHelper {
		#region RowCount Property

		/// <summary>
		/// Adds the specified number of Rows to RowDefinitions. 
		/// Default Height is Auto
		/// </summary>
		public static readonly DependencyProperty RowCountProperty =
				DependencyProperty.RegisterAttached(
						"RowCount", typeof(int), typeof(GridHelper),
						new PropertyMetadata(-1, RowCountChanged));

		public static int GetRowCount(DependencyObject obj) => (int)obj.GetValue(RowCountProperty);

		public static void SetRowCount(DependencyObject obj, int value) => obj.SetValue(RowCountProperty, value);

		// Change Event - Adds the Rows
		public static void RowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			var grid = obj as Grid;
			if (grid is null || (int) e.NewValue < 0)
				return;

			grid.RowDefinitions.Clear();
			for (var i = 0; i < (int) e.NewValue; i++)
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

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
						"ColumnCount", typeof(int), typeof(GridHelper),
						new PropertyMetadata(-1, ColumnCountChanged));

		public static int GetColumnCount(DependencyObject obj) => (int)obj.GetValue(ColumnCountProperty);

		public static void SetColumnCount(DependencyObject obj, int value) => obj.SetValue(ColumnCountProperty, value);

		// Change Event - Add the Columns
		public static void ColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			var grid = obj as Grid;
			if (grid is null || (int) e.NewValue < 0)
				return;

			grid.ColumnDefinitions.Clear();
			for (var i = 0; i < (int) e.NewValue; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

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
						"StarRows", typeof(string), typeof(GridHelper),
						new PropertyMetadata(string.Empty, StarRowsChanged));

		public static string GetStarRows(DependencyObject obj) => (string)obj.GetValue(StarRowsProperty);

		public static void SetStarRows(DependencyObject obj, string value) => obj.SetValue(StarRowsProperty, value);

		// Change Event - Makes specified Row's Height equal to Star
		public static void StarRowsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is Grid grid && e.NewValue.ToString().IsGood())
				SetStarRows(grid);
		}

		#endregion

		#region StarColumns Property

		/// <summary>
		/// Makes the specified Column's Width equal to Star. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty StarColumnsProperty =
				DependencyProperty.RegisterAttached(
						"StarColumns", typeof(string), typeof(GridHelper),
						new PropertyMetadata(string.Empty, StarColumnsChanged));

		public static string GetStarColumns(DependencyObject obj) => (string)obj.GetValue(StarColumnsProperty);

		public static void SetStarColumns(DependencyObject obj, string value) => obj.SetValue(StarColumnsProperty, value);

		// Change Event - Makes specified Column's Width equal to Star
		public static void StarColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is Grid grid && e.NewValue.ToString().IsGood())
				SetStarColumns(grid);
		}

		#endregion

		private static void SetStarColumns(Grid grid) {
			var starColumns = GetStarColumns(grid).Split(',');
			var allStars = GetStarColumns(grid).Like("All");

			for (var i = 0; i < grid.ColumnDefinitions.Count; i++) {
				if (allStars || starColumns.Contains(i.ToString()))
					grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
			}
		}

		private static void SetStarRows(Grid grid) {
			var starRows = GetStarRows(grid).Split(',');
			var allStars = GetStarRows(grid).Like("All");

			for (var i = 0; i < grid.RowDefinitions.Count; i++) {
				if (allStars || starRows.Contains(i.ToString()))
					grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
			}
		}
	}
}