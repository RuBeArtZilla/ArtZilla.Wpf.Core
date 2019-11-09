using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
#if NET40
using ReadOnlyCollectionsExtensions;
#endif

namespace ArtZilla.Wpf {
	/// <summary> Localization manager </summary>
	public sealed class LocalizationManager {
		public static LocalizationManager Instance => _localizationManager ??= new LocalizationManager();

		public ILocalizationProvider LocalizationProvider { get; set; }

		/// <summary> Get localized string by key </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetString(string key) {
			if (Instance.LocalizationProvider is { } provider)
				return provider.GetString(key);
			return string.IsNullOrEmpty(key) ? "[NULL]" : "[" + key + "]";
		}

		/// <summary> Get localized object by key </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static object GetObject(string key) {
			if (Instance.LocalizationProvider is { } provider)
				return provider.GetObject(key);
			return string.IsNullOrEmpty(key) ? "[NULL]" : "[" + key + "]";
		}

#if NET40
		/// <summary> Collection of available languages </summary>
		public static IReadOnlyCollection<CultureInfo> Cultures
			=> Instance.LocalizationProvider?.Cultures ?? new List<CultureInfo> { CurrentCulture }.AsReadOnlyCollection();
#else
		/// <summary> Collection of available languages </summary>
		public static IReadOnlyCollection<CultureInfo> Cultures 
			=> Instance.LocalizationProvider?.Cultures ?? new List<CultureInfo>{CurrentCulture}.AsReadOnly();
#endif

		/// <summary> Current localization </summary>
		public static CultureInfo CurrentCulture {
			get => Thread.CurrentThread.CurrentUICulture;
			set {
				if (Equals(value, Thread.CurrentThread.CurrentUICulture))
					return;

				Thread.CurrentThread.CurrentUICulture = value;
#if !NET40
				CultureInfo.DefaultThreadCurrentUICulture = value;
#endif
				Instance.OnCultureChanged();
			}
		}

		/// <summary> Event occurs when culture changed </summary>
		public event EventHandler CultureChanged;

		private LocalizationManager() { }

		/// <summary> Change language by three letter ISO language name </summary>
		/// <param name="value"></param>
		public static void ChangeLocalization(string value) {
			var cc = CurrentCulture;
			var fc = Cultures.FirstOrDefault(c => c.ThreeLetterISOLanguageName == value) ?? new CultureInfo(value);
			if (Equals(cc, fc))
				return;
			CurrentCulture = fc;
		}

		private void OnCultureChanged()
			=> CultureChanged?.Invoke(this, EventArgs.Empty);

		private static LocalizationManager _localizationManager;
	}
}