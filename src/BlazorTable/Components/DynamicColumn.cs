
namespace BlazorTable.Components {

	using Microsoft.AspNetCore.Components;
	using System;
	using System.Linq.Expressions;

	public class DynamicColumn<TableItem> : IColumn<TableItem> {

		public ITable<TableItem> Table { get; set; }

		public string Title { get; set; }

		public string Width { get; set; }

		public bool Sortable { get; set; }

		public bool Filterable { get; set; }

		public string Format { get; set; }

		public bool FilterOpen { get; }

		public Type Type { get; set; }

		public Expression<Func<TableItem, object>> Field { get; set; }

		public string FieldName { get; set; }

		public Expression<Func<TableItem, bool>> Filter { get; set; }

		public RenderFragment<TableItem> EditTemplate { get; set; }

		public RenderFragment<TableItem> Template { get; set; }

		public string SetFooterValue { get; set; }

		public IFilter<TableItem> FilterControl { get; set; }

		public RenderFragment<IColumn<TableItem>> CustomIFilters { get; set; }

		public bool SortColumn { get; set; }

		public bool SortDescending { get; set; }

		public Align Align { get; set; }

		public AggregateType? Aggregate { get; set; }

		public ElementReference FilterRef { get; set; }

		public string Class { get; set; }

		public string ColumnFooterClass { get; set; }

		public bool? DefaultSortColumn { get; set; }

		public bool? DefaultSortDescending { get; set; }

		public string GetFooterValue() {
			throw new NotImplementedException();
		}

		public string Render(TableItem item) {
			throw new NotImplementedException();
		}

		public void SortBy() {
			throw new NotImplementedException();
		}

		public void ToggleFilter() {
			throw new NotImplementedException();
		}

	}

}