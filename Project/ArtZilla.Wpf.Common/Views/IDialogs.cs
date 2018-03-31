using System.Threading.Tasks;
#if !NET40
#endif

namespace ArtZilla.Wpf {
	public interface IDialogs {
		void ShowMessage(string message, string title = null);
		Task ShowMessageAsync(string message, string title = null);

		bool AskQuestion(string question, string title = null);
		Task<bool> AskQuestionAsync(string question, string title = null);

		string ShowInput(string message, string title = null);
		Task<string> ShowInputAsync(string message, string title = null);
	}
}