using System;
using System.Windows;

namespace ArtZilla.Wpf {
	public interface IFormStorage {
		Boolean Save(Window window);
		Boolean Restore(Window window);
	}
}