using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
#if !NET40
using System.Runtime.CompilerServices;
#endif

namespace ArtZilla.Wpf {
	/// <summary> A base class for the View classes in the MVVM pattern. </summary>
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

	/// <summary> A base class for the View classes with Model in the MVVM pattern. </summary>
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

	public abstract class PageWindowViewModel : ViewModel, IPageHostViewModel {
		public abstract string Url { get; }
		public abstract IPageView View { get; protected set; }
		public abstract IPageViewModel ViewModel { get; protected set; }

		public abstract void ChangePage(IPageViewModel newPage);
		public abstract void ChangePage(Type pvmType);
	}

	public abstract class PageWindowViewModel<TPages>
		: PageWindowViewModel, IPageHostViewModel<TPages> where TPages : struct, Enum {
		public abstract TPages? Page { get; }

		public virtual void ChangePage(TPages page) {
			var (vm, view) = CreateByPage(page);
			if (vm != null && view != null) {
				ChangePageTo(vm, view, null, _page); // todo: url
			} else {
				Debug.Print("Can't find view & view model for page " + page);
			}
		}

		private (IPageViewModel vm, IPageView view) CreateByPage(TPages page) {
			var member = typeof(TPages).GetMember(page.ToString());
			var attrVM = member[0].GetCustomAttributes(typeof(ViewModelAttribute), false).FirstOrDefault();
			var attrView = member[0].GetCustomAttributes(typeof(ViewAttribute), false).FirstOrDefault();

			var vmType = (attrVM as ViewModelAttribute)?.ViewModelType
			             ?? throw new Exception("Can't find view model type for " + page);

			var viewType = (attrView as ViewAttribute)?.ViewType
			             ?? throw new Exception("Can't find view type for " + page);

			if (typeof(IPageViewModel).IsAssignableFrom(vmType))
				throw new Exception($"{vmType} doesn't implement {typeof(IPageViewModel)}");

			if (typeof(IPageView).IsAssignableFrom(viewType))
				throw new Exception($"{viewType} doesn't implement {typeof(IPageView)}");

			var vm = (IPageViewModel) Activator.CreateInstance(vmType);
			var view = (IPageView) Activator.CreateInstance(viewType, vm);
			return (vm, view);
		}

		private void ChangePageTo(IPageViewModel vm, IPageView view, string url, TPages? page) {
			try {
				lock (_navigation) {
					var ovm = _viewModel;

					ovm?.BeforeHide();

					// allow UI&NAV for new page
					vm.Dialogs = Dialogs;
					vm.Host = this;

					vm.BeforeShow();

					// assignment all fields
					_viewModel = vm;
					_view = view;
					_page = page;
					_url = url;

					// disposing old page
					if (ovm != null) {
						// disabling UI&NAV
						ovm.Dialogs = null;
						ovm.Host = null;

						ovm.AfterHide();
					}

					vm.AfterShow();
				}

				// notifying subscribers
				RaisePropertyChanged(nameof(Page));
				RaisePropertyChanged(nameof(View));
				RaisePropertyChanged(nameof(ViewModel));
			} catch (Exception e) {
				Debug.Print(e.ToString());
				// todo: ...
			}
		}

		private string _url;
		private TPages? _page;
		private IPageView _view;
		private IPageViewModel _viewModel;

		private readonly object _navigation = new object();
	}

	public abstract class PageViewModel: ViewModel, IPageViewModel {
		public IPageHostViewModel Host { get; set; }

		public bool IsVisible { get; private set; }

		public PageViewModel() => Debug.Print($"Call {GetType().Name}::{nameof(PageViewModel)}()");

		~PageViewModel()
			=> Debug.Print("Finalized " + GetPageDebugCode());

		public virtual void BeforeShow() {
			Debug.Print(GetPageDebugCode() + " call " + nameof(BeforeShow));
			if (IsVisible)
				throw new Exception("Page not hidden");
		}

		public virtual void AfterShow() {
			Debug.Print(GetPageDebugCode() + " call " + nameof(AfterShow));
			Debug.Assert(IsInDesignMode || Host != null, $"Property {nameof(Host)} is null");
			Debug.Assert(IsInDesignMode || Dialogs != null, $"Property {nameof(Dialogs)} is null");

			IsVisible = true;
		}

		public virtual void BeforeHide() {
			Debug.Print(GetPageDebugCode() + " call " + nameof(BeforeHide));
			if (!IsVisible)
				throw new Exception("Page not visible");
		}

		public virtual void AfterHide() {
			Debug.Print(GetPageDebugCode() + " call " + nameof(AfterHide));
			Debug.Assert(IsInDesignMode || Host is null, $"Property {nameof(Host)} isn't null");
			Debug.Assert(IsInDesignMode || Dialogs is null, $"Property {nameof(Dialogs)} isn't null");

			IsVisible = false;
		}

		protected string GetPageDebugCode() => $"{GetType().Name} #" + GetHashCode();
	}

	public abstract class PageViewModel<TModel> : PageViewModel, IPageViewModel<TModel> {
		public TModel Model { get; }

		protected PageViewModel(TModel model) => Model = model;

		protected PageViewModel(TModel model, IDialogs dialogs) : this(model) => Dialogs = dialogs;
	}

}