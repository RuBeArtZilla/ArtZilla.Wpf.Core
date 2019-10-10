using System;
using System.ComponentModel;
using System.Diagnostics;

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

	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	public sealed class ViewModelAttribute : Attribute {
		public Type ViewModelType { get; }

		public ViewModelAttribute(Type viewModelType) {
			if (!typeof(IViewModel).IsAssignableFrom(viewModelType))
				throw new ArgumentException($"{viewModelType} not implement {typeof(IViewModel)}");
			ViewModelType = viewModelType;
		}
	}

  public interface IPageViewModel : IViewModel {
	  IPageHostViewModel Host { get; set; }

		void BeforeShow();
		void AfterShow();
		void BeforeHide();
		void AfterHide();
	}

	public interface IPageViewModel<out TModel> : IPageViewModel, IViewModel<TModel> {

	}

	public interface IPageHostViewModel : IViewModel {
		string Url { get; } // reserved
		IPageView View{ get; }
		IPageViewModel ViewModel { get; }

		void ChangePage(Type pvmType);
		void ChangePage(IPageViewModel newPage);
	}

	public interface IPageHostViewModel<TPages> : IPageHostViewModel where TPages : struct, Enum {
		TPages? Page { get; }
		void ChangePage(TPages page);
	}
}
