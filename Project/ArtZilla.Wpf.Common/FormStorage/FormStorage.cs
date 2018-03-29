using System;
using System.Windows;
using Microsoft.Win32;

namespace ArtZilla.Wpf {
	public class FormStorage : IFormStorage {
		public const string PlacementKey = "Placement";

		public string CompanyName { get; set; } = "Foo";
		public string ProductName { get; set; } = "Bar";
		public string FormId { get; set; } = "MainForm";

		public FormStorage() { }
		public FormStorage(string companyName, string productName, string formId) {
			CompanyName = companyName;
			ProductName = productName;
			FormId = formId;
		}

		public bool Save(Window window) {
			try {
				var key = GetKey(false);
				if (key == null)
					return false;

				Save(key, window);
				key.Close();
				return true;
			} catch {
				return false;
			}
		}

		public bool Restore(Window window) {
			try {
				var key = GetKey(true);
				if (key == null)
					return false;

				Restore(key, window);
				key.Close();
				return true;
			} catch {
				return false;
			}
		}

		protected virtual RegistryKey GetKey(bool readOnly) {
			RegistryKey key;

			try {
				if (readOnly) {
					key = Registry.CurrentUser?.OpenSubKey("Software");
					key = key?.OpenSubKey(CompanyName);
					key = key?.OpenSubKey(ProductName);
					key = key?.OpenSubKey(FormId);
				} else {
					key = Registry.CurrentUser?.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);
					key = key?.CreateSubKey(CompanyName);
					key = key?.CreateSubKey(ProductName);
					key = key?.CreateSubKey(FormId);
				}
			} catch {
				return null; // todo: Log there ?
			}

			return key;
		}

		protected virtual bool Save(RegistryKey key, Window window) {
			key.SetValue(PlacementKey, window.GetPlacement());
			return true;
		}

		protected virtual bool Restore(RegistryKey key, Window window) {
			var placement = key.GetValue(PlacementKey)?.ToString();
			window.SetPlacement(placement);
			return true;
		}
	}
}