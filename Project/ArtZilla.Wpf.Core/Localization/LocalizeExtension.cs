using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ArtZilla.Wpf {
	// todo: translate to English
	[ContentProperty(nameof(ArgumentBindings))]
	public class LocalizeExtension : MarkupExtension {
		/// <summary> Ключ локализованной строки </summary>
		public string Key { get; set; }

		/// <summary> Привязка для ключа локализованной строки </summary>
		public Binding KeyBinding { get; set; }

		/// <summary> Аргументы форматируемой локализованный строки </summary>
		public IEnumerable<object> Arguments { get; set; }

		/// <summary> Привязки аргументов форматируемой локализованный строки </summary>
		public Collection<BindingBase> ArgumentBindings {
			get => _arguments ??= new Collection<BindingBase>();
			set => _arguments = value;
		}

		public LocalizeExtension() { }

		public LocalizeExtension(string key) => Key = key;

		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (Key != null && KeyBinding != null)
				throw new ArgumentException($"Нельзя одновременно задать {nameof(Key)} и {nameof(KeyBinding)}");
			if (Key is null && KeyBinding is null)
				throw new ArgumentException($"Необходимо задать {nameof(Key)} или {nameof(KeyBinding)}");
			if (Arguments != null && ArgumentBindings.Any())
				throw new ArgumentException($"Нельзя одновременно задать {nameof(Arguments)} и {nameof(ArgumentBindings)}");

			var target = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));
			if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
				return this;

			// Если заданы привязка ключа или список привязок аргументов,
			// то используем BindingLocalizationListener
			if (KeyBinding != null || ArgumentBindings.Count > 0) {
				var listener = new BindingLocalizationListener();

				// Создаем привязку для слушателя
				var listenerBinding = new Binding {Source = listener};
				var keyBinding = KeyBinding ?? new Binding {Source = Key};
				var multiBinding = new MultiBinding {
					Converter = new BindingLocalizationConverter(),
					ConverterParameter = Arguments,
					Bindings = {listenerBinding, keyBinding}
				};

				// Добавляем все переданные привязки аргументов
				foreach (var binding in ArgumentBindings)
					multiBinding.Bindings.Add(binding);

				var value = multiBinding.ProvideValue(serviceProvider);
				// Сохраняем выражение привязки в слушателе
				listener.SetBinding(value as BindingExpressionBase);
				return value;
			}

			// Если задан ключ, то используем KeyLocalizationListener
			if (string.IsNullOrEmpty(Key))
				return default;

			{
				using var listener = new KeyLocalizationListener(Key, Arguments?.ToArray());

				// Если локализация навешана на DependencyProperty объекта DependencyObject или на Setter
				if (target.TargetObject is DependencyObject
				    && target.TargetProperty is DependencyProperty
				    || target.TargetObject is Setter) {
					var binding = new Binding(nameof(KeyLocalizationListener.Value)) {
						Source = listener,
						UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
					};
					return binding.ProvideValue(serviceProvider);
				}

				// Если локализация навешана на Binding, то возвращаем слушателя
				if (target.TargetObject is Binding targetBinding
				    && target.TargetProperty != null
				    && target.TargetProperty.GetType().FullName == "System.Reflection.RuntimePropertyInfo"
				    && target.TargetProperty.ToString() == "System.Object Source") {
					targetBinding.Path = new PropertyPath(nameof(KeyLocalizationListener.Value));
					targetBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					return listener;
				}

				// Иначе возвращаем локализованную строку
				return listener.Value;
			}
		}

		private Collection<BindingBase> _arguments;
	}
}