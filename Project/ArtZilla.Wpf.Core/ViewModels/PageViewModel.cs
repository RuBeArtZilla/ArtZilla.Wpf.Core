using System;
using System.Diagnostics;

namespace ArtZilla.Wpf {
	public abstract class PageViewModel: ViewModel, IPageViewModel {
		public IPageHostViewModel Host { get; set; }

		public bool IsVisible { get; private set; }

		public PageViewModel()
			=> Debug.Print($"Call {GetType().Name}::{nameof(PageViewModel)}()");

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
}