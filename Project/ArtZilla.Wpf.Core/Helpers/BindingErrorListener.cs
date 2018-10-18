using System;
using System.Diagnostics;

namespace ArtZilla.Wpf.Helpers {
	public class BindingErrorListener : TraceListener {
		public static void Listen(Action<string> logAction)
			=> PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener { _logAction = logAction });

		public override void Write(string message) { }

		public override void WriteLine(string message) => _logAction(message);

		private Action<string> _logAction;
	}
}