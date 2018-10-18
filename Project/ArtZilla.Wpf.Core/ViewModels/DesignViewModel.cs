using System;
#if !NET40
using System.Runtime.CompilerServices;
#endif

namespace ArtZilla.Wpf {
	public abstract class DesignViewModel<TModel>: ViewModel<TModel> where TModel : new() {
		protected DesignViewModel()
			: base(IsInDesignModeStatic
				? new TModel()
				: throw new Exception("This ctor only for design mode")) { }

		protected DesignViewModel(TModel model) : base(model) { }

		protected DesignViewModel(TModel model, IDialogs coordinator) : base(model, coordinator) { }
	}
}