using System.Collections.Generic;
using System.Globalization;

namespace ArtZilla.Wpf {
	/// <summary> interface for localization provider </summary>
	public interface ILocalizationProvider {
		/// <summary> Get localized string by key </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetString(string key);

		/// <summary> Get localized object by key </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object GetObject(string key);

		/// <summary> Collection of available languages </summary>
		IReadOnlyCollection<CultureInfo> Cultures { get; }
	}
}