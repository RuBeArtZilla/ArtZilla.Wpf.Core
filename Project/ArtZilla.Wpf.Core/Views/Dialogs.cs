using System.Threading.Tasks;
using System.Windows;

namespace ArtZilla.Wpf {
	/// <inheritdoc />
	public class Dialogs : IDialogs {
		public Dialogs(Window owner = default) 
			=> Owner = owner;

		/// <inheritdoc />
		public virtual void ShowMessage(string message, string title = null) {
			if (Owner is null)
				MessageBox.Show(message, title);
			else
				MessageBox.Show(Owner, message, title);
		}

		/// <inheritdoc />
		public virtual Task ShowMessageAsync(string message, string title = null) {
			void InnerAction() => ShowMessage(message, title);

#if NET40
			return Task.Factory.StartNew(
				InnerAction,
				System.Threading.CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
#else
			return Task.Run(InnerAction);
#endif
		}

		/// <inheritdoc />
		public virtual bool AskQuestion(string question, string title = null) {
			if (Owner is null)
				return MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
			return MessageBox.Show(Owner, question, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}

		/// <inheritdoc />
		public virtual Task<bool> AskQuestionAsync(string question, string title = null) {
			bool InnerAction() => AskQuestion(question, title);

#if NET40
			return Task.Factory.StartNew(
				InnerAction,
				System.Threading.CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
#else
			return Task.Run(InnerAction);
#endif
		}

		/// <inheritdoc />
		public virtual string ShowInput(string message, string title = null) {
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public virtual Task<string> ShowInputAsync(string message, string title = null) {
			throw new System.NotImplementedException();
		}

		/// <summary> parent window </summary>
		protected readonly Window Owner;
	}
}