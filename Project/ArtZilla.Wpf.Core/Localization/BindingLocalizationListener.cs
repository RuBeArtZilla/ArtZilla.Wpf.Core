using System.Windows.Data;

namespace ArtZilla.Wpf {
	/// <summary> Слушатель изменения культуры при локализации по привязке </summary>
	public class BindingLocalizationListener : BaseLocalizationListener {
		private BindingExpressionBase BindingExpression { get; set; }

		public void SetBinding(BindingExpressionBase bindingExpression)
			=> BindingExpression = bindingExpression;

		/// <inheritdoc />
		protected override void OnCultureChanged() {
			try {
				// Обновляем результат выражения привязки, при этом конвертер вызывается повторно уже для новой культуры
				BindingExpression?.UpdateTarget();
			} catch {
				// ignored
			}
		}
	}
}