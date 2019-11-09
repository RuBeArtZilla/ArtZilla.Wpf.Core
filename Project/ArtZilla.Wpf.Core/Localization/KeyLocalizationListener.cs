using System.ComponentModel;

namespace ArtZilla.Wpf {
	public sealed class KeyLocalizationListener : BaseLocalizationListener, INotifyPropertyChanged {
		public object Value {
			get {
				var value = LocalizationManager.GetObject(_key);
				if (value is string format && _args != null)
					value = string.Format(format, _args);
				return value;
			}
		}

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		public KeyLocalizationListener(string key, object[] args) {
			_key = key;
			_args = args;
		}
		
		/// <inheritdoc />
		protected override void OnCultureChanged()
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

		private readonly string _key;
		private readonly object[] _args;
	}
}