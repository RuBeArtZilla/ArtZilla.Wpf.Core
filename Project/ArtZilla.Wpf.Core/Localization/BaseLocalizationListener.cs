using System;
using System.Windows;

namespace ArtZilla.Wpf {
	public abstract class BaseLocalizationListener : IWeakEventListener, IDisposable {
		protected BaseLocalizationListener()
			=> CultureChangedEventManager.AddListener(LocalizationManager.Instance, this);

		/// <inheritdoc />
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType != typeof(CultureChangedEventManager))
				return false;

			OnCultureChanged();
			return true;
		}

		/// <inheritdoc />
		public void Dispose() {
			CultureChangedEventManager.RemoveListener(LocalizationManager.Instance, this);
			GC.SuppressFinalize(this);
		}

		protected abstract void OnCultureChanged();
	}
}