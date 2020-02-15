using System.Windows;

namespace ArtZilla.Wpf.Demo {
	/// <summary> Interaction logic for App.xaml </summary>
	public partial class App : Application {
		static App() {
			LocalizationManager.Instance.LocalizationProvider = new ResxLocalizationProvider();
		}
	}
}
