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
	/// <summary>
	/// A base class for the View classes in the MVVM pattern.
	/// </summary>
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

	/// <summary>
	/// A base class for the View classes with Model in the MVVM pattern.
	/// </summary>
	/// <typeparam name="TModel">Model</typeparam>
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
}