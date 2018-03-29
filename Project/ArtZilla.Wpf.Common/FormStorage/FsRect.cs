using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace ArtZilla.Wpf {
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
}