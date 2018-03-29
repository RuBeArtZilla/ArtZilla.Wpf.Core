using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace ArtZilla.Wpf {
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
}