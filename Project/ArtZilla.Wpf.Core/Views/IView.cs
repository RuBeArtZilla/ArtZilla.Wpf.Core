using System;

namespace ArtZilla.Wpf {
	/// <summary> A base interface for the View classes in the MVVM pattern. </summary>
	public interface IView { }

	/// <summary> A base interface for the View classes with ViewModel in the MVVM pattern. </summary>
	/// <typeparam name="TViewModel">ViewModel</typeparam>
	public interface IView<TViewModel>: IView where TViewModel: IViewModel {
		TViewModel ViewModel { get; set; }
	}

	public interface IPageView : IView {

	}

	public interface IPageView<TPageViewModel> : IPageView, IView<TPageViewModel> where TPageViewModel : IPageViewModel {

	}

	public interface IPageHostView : IView {

	}

	public interface IPageHostView<TPageHostViewModel> : IPageHostView, IView<TPageHostViewModel> where TPageHostViewModel : IPageHostViewModel {

	}
}