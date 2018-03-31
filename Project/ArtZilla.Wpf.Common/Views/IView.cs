using System;

namespace ArtZilla.Wpf {
	/// <summary>
	/// A base interface for the View classes in the MVVM pattern.
	/// </summary>
	public interface IView { }

	/// <summary>
	/// A base interface for the View classes with ViewModel in the MVVM pattern.
	/// </summary>
	/// <typeparam name="TViewModel">ViewModel</typeparam>
	public interface IView<TViewModel>: IView where TViewModel: IViewModel {
		TViewModel ViewModel { get; set; }
	}
}