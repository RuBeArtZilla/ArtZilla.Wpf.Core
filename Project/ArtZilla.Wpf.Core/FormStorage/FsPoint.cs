using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace ArtZilla.Wpf {
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
}