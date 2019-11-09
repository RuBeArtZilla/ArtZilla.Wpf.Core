using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ArtZilla.Wpf {
	/// <inheritdoc cref="IPageHostViewModel"/>
	public abstract class PageHostViewModel : ViewModel, IPageHostViewModel {
		/// <inheritdoc />
		public abstract string Url { get; }

		/// <inheritdoc />
		public abstract IPageView View { get; }

		/// <inheritdoc />
		public abstract IPageViewModel ViewModel { get; }

		/// <inheritdoc />
		public abstract void ChangePage(IPageViewModel newPage);

		/// <inheritdoc />
		public abstract void ChangePage(Type pvmType);

		protected virtual void BeforePageChanged()
			=> Debug.WriteLine(nameof(BeforePageChanged));

		protected virtual void AfterPageChanged()
			=> Debug.WriteLine(nameof(AfterPageChanged));
	}

	/// <inheritdoc cref="IPageHostViewModel"/>
	public abstract class PageHostViewModel<TPages>
		: PageHostViewModel, IPageHostViewModel<TPages> where TPages : struct, Enum {
		/// <inheritdoc />
		public override string Url => _page.ToString();

		/// <inheritdoc />
		public virtual TPages? Page => _page;

		/// <inheritdoc />
		public override IPageView View => _view;

		/// <inheritdoc />
		public override IPageViewModel ViewModel => _viewModel;

		/// <inheritdoc />
		public override void ChangePage(IPageViewModel newPage) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override void ChangePage(Type pvmType) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public virtual void ChangePage(TPages page) {
			var (vm, view) = CreateByPage(page);
			if (vm == null || view == null)
				throw new Exception("Can't find view & view model for page " + page);

			ChangePageTo(vm, view, null, _page); // todo: url
		}

		protected virtual (IPageViewModel vm, IPageView view) CreateByPage(TPages page) {
			var member = typeof(TPages).GetMember(page.ToString());
			var attrVM = member[0].GetCustomAttributes(typeof(ViewModelAttribute), false).FirstOrDefault();
			var attrView = member[0].GetCustomAttributes(typeof(ViewAttribute), false).FirstOrDefault();

			var vmType = (attrVM as ViewModelAttribute)?.ViewModelType
			             ?? Array.Find(Assembly.GetAssembly(typeof(TPages)).GetTypes(),
			                           t => t.Name.EndsWith("." + page + "PageVM")
			             )
			             ?? throw new Exception("Can't find view model type for " + page);

			var viewType = (attrView as ViewAttribute)?.ViewType
			               ?? Array.Find(Assembly.GetAssembly(typeof(TPages)).GetTypes(),
			                             t => t.Name.EndsWith("." + page + "Page")
			               )
			               ?? throw new Exception("Can't find view type for " + page);

			if (!typeof(IPageViewModel).IsAssignableFrom(vmType))
				throw new Exception($"{vmType} doesn't implement {typeof(IPageViewModel)}");

			if (!typeof(IPageView).IsAssignableFrom(viewType))
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
}