using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArtZilla.Wpf.Helpers;

namespace ArtZilla.Wpf.Tests.Net40 {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: Window {
		private readonly IFormStorage _storage = new FormStorage("ArtZilla", "ArtZilla.Wpf.Common", "Net40");

		public MainWindow() {
			BindingErrorListener.Listen(m => MessageBox.Show(m));
			InitializeComponent();
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			_storage.Restore(this);
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			_storage.Save(this);
		}
	}
}
