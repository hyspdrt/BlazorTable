
namespace BlazorTable {

	using System;
	using System.Linq.Expressions;

	public sealed class ColumnOptions<TableItem> : IColumnOptions<TableItem> {

		public string Title { get; set; }

		public string Width { get; set; }

		public bool Sortable { get; set; }

		public bool Filterable { get; set; }

		public string Format { get; set; }

		public Type Type { get; set; }

		public Expression<Func<TableItem, object>> Field { get; set; }

		public string FieldName { get; set; }

		public Expression<Func<TableItem, bool>> Filter { get; set; }

		public Align Align { get; set; }

		public AggregateType? Aggregate { get; set; }

		public string Class { get; set; }

		public string ColumnFooterClass { get; set; }

	}

}