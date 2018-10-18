using System;
using System.ComponentModel;

namespace ArtZilla.Wpf {
	/// <summary> A base interface for the ViewModel classes in the MVVM pattern. </summary>
	public interface IViewModel: INotifyPropertyChanged {
		IDialogs Dialogs { get; set; }
		bool IsInDesignMode { get; }
	}

	/// <summary> A base interface for the ViewModel classes with Model in the MVVM pattern. </summary>
	/// <typeparam name="TModel">Model</typeparam>
	public interface IViewModel<out TModel>: IViewModel {
		TModel Model { get; }
	}

  public interface IPageViewModel : IViewModel {
		void BeforeShow();
		void AfterShow();
		void BeforeHide();
		void AfterHide();
	}

	public interface IPageViewModel<out TModel> : IPageViewModel, IViewModel<TModel> {

	}

	public interface IPageHostViewModel : IViewModel {
		void ChangePage(IPageViewModel newPage);
		void ChangePage(Type pvmType);
	}

	public interface IPageHostViewModel<in TPageViewModel> : IPageHostViewModel where TPageViewModel : IPageViewModel {
		void ChangePage(TPageViewModel newPage);
	}
}
