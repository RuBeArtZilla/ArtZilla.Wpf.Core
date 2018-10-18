using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Serialization;
using ArtZilla.Net.Core.Extensions;

namespace ArtZilla.Wpf {
	public static class FsWindowPlacement {
		private static readonly Encoding Encoding = new UTF8Encoding();
		private static readonly XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces();
		private static readonly XmlSerializer Serializer = new XmlSerializer(typeof (WINDOWPLACEMENT));

		private const int SW_SHOWNORMAL = 1;
		private const int SW_SHOWMINIMIZED = 2;

		[DllImport("user32.dll")]
		private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

		static FsWindowPlacement() => Namespaces.Add("", "");

		public static void SetPlacement(IntPtr windowHandle, string placementXml) {
			if (placementXml.IsBad())
				return;

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

		public static string GetPlacement(IntPtr windowHandle) {
			var placement = new WINDOWPLACEMENT();
			GetWindowPlacement(windowHandle, out placement);

			using (var memoryStream = new MemoryStream())
			using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding)) {
				Serializer.Serialize(xmlTextWriter, placement, Namespaces);
				return Encoding.GetString(memoryStream.ToArray());
			}
		}

		public static void SetPlacement(this Window window, String placementXml)
			=> SetPlacement(new WindowInteropHelper(window).Handle, placementXml);

		public static String GetPlacement(this Window window)
			=> GetPlacement(new WindowInteropHelper(window).Handle);
	}
}