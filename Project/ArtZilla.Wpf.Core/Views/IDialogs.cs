using System.Threading.Tasks;

namespace ArtZilla.Wpf {
	/// <summary> </summary>
	public interface IDialogs {
		/// <summary> </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		void ShowMessage(string message, string title = null);

		/// <summary> </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		Task ShowMessageAsync(string message, string title = null);

		/// <summary> </summary>
		/// <param name="question"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		bool AskQuestion(string question, string title = null);

		/// <summary> </summary>
		/// <param name="question"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		Task<bool> AskQuestionAsync(string question, string title = null);

		/// <summary> </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		string ShowInput(string message, string title = null);

		/// <summary> </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		Task<string> ShowInputAsync(string message, string title = null);
	}
}