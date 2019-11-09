using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ArtZilla.Net.Core.Extensions;
using ArtZilla.Wpf.Demo.Properties;

namespace ArtZilla.Wpf.Demo {
	public class ResxLocalizationProvider : ILocalizationProvider {
		public readonly static string[] Locals
			= {"en-US", "ru-RU"};

		public readonly static IReadOnlyDictionary<string, CultureInfo> Aliases
			= Locals.ToDictionary(l => l, l => new CultureInfo(l));

		/// <inheritdoc />
		public IReadOnlyCollection<CultureInfo> Cultures { get; }
			= Aliases.Values.ToArray();

		/// <inheritdoc />
		public string GetString(string key) 
			=> Resources.ResourceManager.GetString(key) ?? _fallback.GetValueOrDefault(key);

		/// <inheritdoc />
		public object GetObject(string key)
			=> Resources.ResourceManager.GetObject(key);

		private static Dictionary<string, string> _fallback
			= EnumTraits<LocStrings>.EnumValues
				.Select(i => i.ToString())
				.ToDictionary(
					i => i,
					i => typeof(LocStrings)
						.GetMember(i)[0]
						.GetCustomAttributes(typeof(DescriptionAttribute), false)
						.OfType<DescriptionAttribute>()
						.First().Description
				);
	}
}

namespace ArtZilla.Wpf.Demo.L {
	// ReSharper disable once InconsistentNaming
	// ReSharper disable once UnusedMember.Global
	public class lExtension : LocalizeExtension {
		static lExtension() 
			=> LocalizationManager.Instance.LocalizationProvider ??= new ResxLocalizationProvider();

		public lExtension(LocStrings key) => Key = key.ToString();
	}
}