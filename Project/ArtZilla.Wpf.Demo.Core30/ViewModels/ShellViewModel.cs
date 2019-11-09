namespace ArtZilla.Wpf.Demo.ViewModels {
	public enum Pages { Home, Language, Controls }

	public class MainWindowViewModel : PageHostViewModel<Pages> { }
  public class HomePageViewModel : PageViewModel { }
	public class LanguagePageViewModel : PageViewModel { }
	public class ControlsPageViewModel : PageViewModel { }
}