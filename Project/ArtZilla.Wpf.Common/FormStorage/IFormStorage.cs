using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace ArtZilla.Wpf.Common {
	[Serializable,StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT {
		[XmlAttribute]
		public Int32 length;
		[XmlAttribute]
		public Int32 flags;
		[XmlAttribute]
		public Int32 showCmd;
		public FsPoint minPosition;
		public FsPoint maxPosition;
		public FsRect normalPosition;
	}

	[Serializable,StructLayout(LayoutKind.Sequential)]
	public struct FsPoint {
		[XmlAttribute]
		public Int32 X;
		[XmlAttribute]
		public Int32 Y;

		public FsPoint(Int32 x, Int32 y) {
			X = x;
			Y = y;
		}
	}

	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FsRect {
		[XmlAttribute]
		public Int32 Left;
		[XmlAttribute]
		public Int32 Top;
		[XmlAttribute]
		public Int32 Right;
		[XmlAttribute]
		public Int32 Bottom;

		public FsRect(Int32 left, Int32 top, Int32 right, Int32 bottom) {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}

	public static class FsWindowPlacement {
		static Encoding Encoding = new UTF8Encoding();
		static XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces();
		static XmlSerializer Serializer = new XmlSerializer(typeof (WINDOWPLACEMENT));

		const Int32 SW_SHOWNORMAL = 1;
		const Int32 SW_SHOWMINIMIZED = 2;

		[DllImport("user32.dll")]
		static extern Boolean SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		static extern Boolean GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

		static FsWindowPlacement() {
			Namespaces.Add("", "");
		}

		public static void SetPlacement(IntPtr windowHandle, String placementXml) {
			if (String.IsNullOrEmpty(placementXml)) return;

			try {
				WINDOWPLACEMENT placement;
				var bytes = Encoding.GetBytes(placementXml);
				using (var memoryStream = new MemoryStream(bytes))
					placement = (WINDOWPLACEMENT) Serializer.Deserialize(memoryStream);

				placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
				placement.flags = 0;
				placement.showCmd = (placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd);
				SetWindowPlacement(windowHandle, ref placement);
			} catch (InvalidOperationException) {
				// Parsing placement XML failed. Fail silently.
			}
		}

		public static String GetPlacement(IntPtr windowHandle) {
			var placement = new WINDOWPLACEMENT();
			GetWindowPlacement(windowHandle, out placement);

			using (var memoryStream = new MemoryStream()) {
				using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding)) {
					Serializer.Serialize(xmlTextWriter, placement, Namespaces);
					return Encoding.GetString(memoryStream.ToArray());
				}
			}
		}

		public static void SetPlacement(this Window window, String placementXml)
			=> SetPlacement(new WindowInteropHelper(window).Handle, placementXml);

		public static String GetPlacement(this Window window)
			=> GetPlacement(new WindowInteropHelper(window).Handle);
	}

	public interface IFormStorage {
		Boolean Save(Window window);
		Boolean Restore(Window window);
	}

	public class FormStorage : IFormStorage {
		public const String PlacementKey = "Placement";

		public String CompanyName { get; set; } = "Foo";
		public String ProductName { get; set; } = "Bar";
		public String FormId { get; set; } = "MainForm";

		public FormStorage() { }
		public FormStorage(String companyName, String productName, String formId) {
			CompanyName = companyName;
			ProductName = productName;
			FormId = formId;
		}

		public Boolean Save(Window window) {
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

		public Boolean Restore(Window window) {
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

		protected virtual RegistryKey GetKey(Boolean readOnly) {
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
			} catch (Exception e) {
				return null; //todo: Log there
			}

			return key;
		}

		protected virtual Boolean Save(RegistryKey key, Window window) {
			key.SetValue(PlacementKey, window.GetPlacement());
			return true;
		}

		protected virtual Boolean Restore(RegistryKey key, Window window) {
			var placement = key.GetValue(PlacementKey)?.ToString();
			window.SetPlacement(placement);
			return true;
		}
	}
}