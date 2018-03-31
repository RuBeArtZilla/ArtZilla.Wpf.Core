using System.ComponentModel;

namespace ArtZilla.Wpf {
	/// <summary>
	/// A base interface for the ViewModel classes in the MVVM pattern.
	/// </summary>
	public interface IViewModel: INotifyPropertyChanged {
		IDialogs Dialogs { get; set; }

		bool IsInDesignMode { get; }
	}

	/// <summary>
	/// A base interface for the ViewModel classes with Model in the MVVM pattern.
	/// </summary>
	/// <typeparam name="TModel">Model</typeparam>
	public interface IViewModel<TModel>: IViewModel {
		TModel Model { get; }
	}
}