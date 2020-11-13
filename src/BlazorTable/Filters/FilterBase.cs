
namespace BlazorTable {

	using Microsoft.AspNetCore.Components;
	using System;
	using System.Linq.Expressions;

	public abstract class FilterBase<TableItem, TEnum> : ComponentBase, IFilter<TableItem, TEnum> where TEnum : struct, Enum {

		[CascadingParameter(Name = "Column")]
		public IColumn<TableItem> Column { get; set; }

		public TEnum Condition { get; set; }

		public TEnum[] Options {
			get {
				return Enum.GetValues<TEnum>();
			}
		}

		public virtual void OnFilterChange(ChangeEventArgs args) {
			this.Condition = Enum.Parse<TEnum>(args.Value.ToString());
		}

		public abstract Expression<Func<TableItem, bool>> GetFilter();

	}

}