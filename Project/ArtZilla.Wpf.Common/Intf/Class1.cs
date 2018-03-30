using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
#if !NET40
using System.Runtime.CompilerServices;
#endif

namespace ArtZilla.Wpf {
	public interface IDialogs {
		void ShowMessage(string message, string title = null);
		Task ShowMessageAsync(string message, string title = null);

		bool AskQuestion(string question, string title = null);
		Task<bool> AskQuestionAsync(string question, string title = null);

		string ShowInput(string message, string title = null);
		Task<string> ShowInputAsync(string message, string title = null);
	}

	public interface IViewModel: INotifyPropertyChanged {
		IDialogs Dialogs { get; set; }
	}

	public interface IViewModel<TModel>: IViewModel {
		TModel Model { get; }
	}

	public interface IView { }
	public interface IView<TViewModel>: IView where TViewModel : IViewModel {
		TViewModel ViewModel { get; set; }
	}

	public abstract class ViewModel: ViewModelBase, IViewModel {
		public IDialogs Dialogs { get; set; }

		protected ViewModel() { }

		protected ViewModel(IDialogs dialogs)
			=> Dialogs = dialogs;

		/// <summary>
		/// Checks if a property already matches a desired value.  Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
#if NET40
		protected bool SetProperty<T>(ref T storage, T value, string propertyName = null) {
#else
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
#endif
			if (Equals(storage, value))
				return false;

			storage = value;
			RaisePropertyChanged(propertyName);
			return true;
		}
	}

	public abstract class ViewModel<TModel>: ViewModel, IViewModel<TModel> {
		public TModel Model { get; }

		protected ViewModel(TModel model)
			=> Model = model;

		protected ViewModel(TModel model, IDialogs coordinator) : base(coordinator)
			=> Model = model;

		protected void UpdateModel<TValue>(Expression<Func<TModel, TValue>> selector, object value, string vmPropertyName = null) {
			Expression body = selector;
			if (body is LambdaExpression)
				body = ((LambdaExpression)body).Body;

			var property = (PropertyInfo)((MemberExpression)body).Member;
			if (property.GetValue(Model, null) == value)
				return;

			property.SetValue(Model, value, null);
			var propertyName = vmPropertyName ?? property.Name;
			RaisePropertyChanged(propertyName);
			AfterModelUpdated(propertyName);
		}

		protected void UpdateProperty<TObject, TProperty>(TObject @object, Expression<Func<TObject, TProperty>> selector, object value, string vmPropertyName = null) {
			Expression body = selector;
			if (body is LambdaExpression)
				body = ((LambdaExpression)body).Body;

			var property = (PropertyInfo)((MemberExpression)body).Member;
			if (property.GetValue(@object, null) == value)
				return;

			property.SetValue(@object, value, null);
			var propertyName = vmPropertyName ?? property.Name;
			RaisePropertyChanged(propertyName);
			AfterModelUpdated(propertyName);
		}

		protected virtual void AfterModelUpdated(string propertyName) {
			// nothing to do
		}
	}

	public abstract class DesignViewModel<TModel>: ViewModel<TModel> where TModel : new() {
		protected DesignViewModel()
			: base(IsInDesignModeStatic
				? new TModel()
				: throw new Exception("This ctor only for design mode")) { }

		protected DesignViewModel(TModel model) : base(model) { }

		protected DesignViewModel(TModel model, IDialogs coordinator) : base(model, coordinator) { }
	}
}