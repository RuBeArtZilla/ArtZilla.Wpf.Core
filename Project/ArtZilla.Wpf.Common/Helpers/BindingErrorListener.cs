using System;
using System.Diagnostics;

namespace ArtZilla.Wpf.Common.Helpers {
	public class BindingErrorListener : TraceListener {
		private Action<String> logAction;

		public static void Listen(Action<String> logAction) {
			PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener { logAction = logAction });
		}

		public override void Write(String message) { }

		public override void WriteLine(String message) {
			logAction(message);
		}
	}
}