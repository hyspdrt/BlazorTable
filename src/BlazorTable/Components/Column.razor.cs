
namespace BlazorTable {

	using Microsoft.AspNetCore.Components;
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;

	/// <summary>
	/// Table Column
	/// </summary>
	/// <typeparam name="TableItem"></typeparam>
	public partial class Column<TableItem> : ComponentBase, IColumn<TableItem> {

		/// <summary>
		/// Parent Table
		/// </summary>
		[CascadingParameter(Name = "Table")]
		public ITable<TableItem> Table { get; set; }

		private string _title;

		/// <summary>
		/// Title (Optional, will use Field Name if null)
		/// </summary>
		[Parameter]
		public string Title {
			get { return _title ?? this.FieldName; }
			set { _title = value; }
		}

		/// <summary>
		/// Width auto|value|initial|inherit
		/// </summary>
		[Parameter]
		public string Width { get; set; }

		/// <summary>
		/// Column can be sorted
		/// </summary>
		[Parameter]
		public bool Sortable { get; set; } = true;

		/// <summary>
		/// Column can be filtered
		/// </summary>
		[Parameter]
		public bool Filterable { get; set; } = true;

		/// <summary>
		/// Normal Item Template
		/// </summary>
		[Parameter]
		public RenderFragment<TableItem> Template { get; set; }

		/// <summary>
		/// Edit Mode Item Template
		/// </summary>
		[Parameter]
		public RenderFragment<TableItem> EditTemplate { get; set; }

		/// <summary>
		/// Set custom Footer column value 
		/// </summary>
		[Parameter]
		public string SetFooterValue { get; set; }

		/// <summary>
		/// Place custom controls which implement IFilter
		/// </summary>
		[Parameter]
		public RenderFragment<IColumn<TableItem>> CustomIFilters { get; set; }

		/// <summary>
		/// Field which this column is for<br />
		/// Required when Sortable = true<br />
		/// Required when Filterable = true
		/// </summary>
		[Parameter]
		public Expression<Func<TableItem, object>> Field { get; set; }

		/// <summary>
		/// Field which this column is for<br />
		/// Required when Sortable = true<br />
		/// Required when Filterable = true
		/// </summary>
		[Parameter]
		public string FieldName { get; set; }

		private bool HasField {
			get {
				return this.Field != null || !string.IsNullOrWhiteSpace(this.FieldName);
			}
		}

		/// <summary>
		/// Horizontal alignment
		/// </summary>
		[Parameter]
		public Align Align { get; set; }

		/// <summary>
		/// Aggregates table column for the footer. It can only be applied to numerical fields (e.g. int, long decimal, double, etc.).
		/// </summary>
		[Parameter]
		public AggregateType? Aggregate { get; set; }

		/// <summary>
		/// Set the format for values if no template
		/// </summary>
		[Parameter]
		public string Format { get; set; }

		/// <summary>
		/// Column CSS Class
		/// </summary>
		[Parameter]
		public string Class { get; set; }

		/// <summary>
		/// Column Footer CSS Class
		/// </summary>
		[Parameter]
		public string ColumnFooterClass { get; set; }

		/// <summary>
		/// Filter expression
		/// </summary>
		public Expression<Func<TableItem, bool>> Filter { get; set; }

		/// <summary>
		/// True if this is the default Sort Column
		/// </summary>
		[Parameter]
		public bool? DefaultSortColumn { get; set; }

		/// <summary>
		/// Direction of default sorting
		/// </summary>
		[Parameter]
		public bool? DefaultSortDescending { get; set; }

		/// <summary>
		/// True if this is the current Sort Column
		/// </summary>
		public bool SortColumn { get; set; }

		/// <summary>
		/// Direction of sorting
		/// </summary>
		public bool SortDescending { get; set; }

		/// <summary>
		/// Filter Panel is open
		/// </summary>
		public bool FilterOpen { get; private set; }

		/// <summary>
		/// Column Data Type
		/// </summary>
		[Parameter]
		public Type Type { get; set; }

		/// <summary>
		/// Filter Icon Element
		/// </summary>
		public ElementReference FilterRef { get; set; }

		/// <summary>
		/// Currently applied Filter Control
		/// </summary>
		public IFilter<TableItem> FilterControl { get; set; }

		protected override void OnInitialized() {

			var tc = this.Table as Table<TableItem>;
			tc.AssociateColumn(this);

			if (DefaultSortDescending.HasValue) {
				this.SortDescending = DefaultSortDescending.Value;
			}

			if (DefaultSortColumn.HasValue) {
				this.SortColumn = DefaultSortColumn.Value;
			}

		}



		protected override void OnParametersSet() {

			ResolveFieldName();

			if (Sortable && !this.HasField ||
				Filterable && !this.HasField) {
				throw new InvalidOperationException($"Column {Title} Property parameter is null");
			}

			if (Title == null && !this.HasField) {
				throw new InvalidOperationException("A Column has both Title and Field parameters null");
			}

			if (Type == null) {
				Type = Field?.GetPropertyMemberInfo().GetMemberUnderlyingType();
			}

		}

		/// <summary>
		/// Opens/Closes the Filter Panel
		/// </summary>
		public void ToggleFilter() {
			FilterOpen = !FilterOpen;
			Table.Refresh();
		}

		/// <summary>
		/// Sort by this column
		/// </summary>
		public void SortBy() {

			if (Sortable) {

				if (SortColumn) {
					SortDescending = !SortDescending;
				}

				Table.Columns.ForEach(x => x.SortColumn = false);

				SortColumn = true;

				Table.Update();

			}

		}

		/// <summary>
		/// Returns aggregation of this column for the table footer based on given type: Sum, Average, Count, Min, or Max.
		/// </summary>
		/// <returns>string results</returns>
		public string GetFooterValue() {
			if (!this.Table.ShowFooter) {
				return string.Empty;
			}
			if (!this.Aggregate.HasValue) {
				return string.Empty;
			}
			if (this.Table.ItemsQueryable == null) {
				return string.Empty;
			}
			if (string.IsNullOrEmpty(this.FieldName)) {
				return string.Empty;
			}
			var val = this.Aggregate.Value switch {
				AggregateType.Count => this.Table.ItemsQueryable.Count(),
				AggregateType.Min => this.Table.ItemsQueryable.AsEnumerable().Min(c => c.GetMemberValue(this.FieldName)),
				AggregateType.Max => this.Table.ItemsQueryable.AsEnumerable().Max(c => c.GetMemberValue(this.FieldName)),
				_ => this.Table.ItemsQueryable.Aggregate(this.FieldName, this.Aggregate.Value)
			};
			var final = string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", val);
			return final;
		}

		private void ResolveFieldName() {

			if (this.Field != null && string.IsNullOrWhiteSpace(this.FieldName)) {
				var memberInfo = this.Field.GetPropertyMemberInfo();
				if (memberInfo != null) {
					this.FieldName = memberInfo.Name;
					return;
				}
			}

			if (Field == null && !string.IsNullOrWhiteSpace(this.FieldName)) {
				var type = typeof(TableItem);
				var param = Expression.Parameter(type, "item");
				var prop = type.GetMemberInfo(this.FieldName);
				var memberExpression = Expression.MakeMemberAccess(param, prop);
				var exp = Expression.Convert(memberExpression, typeof(object));
				this.Field = Expression.Lambda<Func<TableItem, object>>(exp, param);
			}

		}

		/// <summary>
		/// Render a default value if no template
		/// </summary>
		/// <param name="data">data row</param>
		/// <returns></returns>
		public string Render(TableItem data) {

			if (data == null) {
				return string.Empty;
			}

			if (Field == null) {
				return string.Empty;
			}

			if (renderCompiled == null) {
				renderCompiled = Field.Compile();
			}

			object value = null;

			try {
				value = renderCompiled.Invoke(data);
			} catch (NullReferenceException) {
			}

			if (value == null) {
				return string.Empty;
			}

			if (string.IsNullOrEmpty(Format)) {
				return value.ToString();
			}

			return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", value);
		}

		/// <summary>
		/// Save compiled renderCompiled property to avoid repeated Compile() calls
		/// </summary>
		private Func<TableItem, object> renderCompiled;

		internal void Init(IColumnOptions<TableItem> options) {

			_shouldRender = false;

			this.Aggregate = options.Aggregate;
			this.Align = options.Align;
			this.Class = options.Class;
			this.ColumnFooterClass = options.ColumnFooterClass;
			this.Field = options.Field;
			this.FieldName = options.FieldName;
			this.Filter = options.Filter;
			this.Filterable = options.Filterable;
			this.Format = options.Format;
			this.Sortable = options.Sortable;
			this.Title = options.Title;
			this.Type = options.Type;
			this.Width = options.Width;

			if (this.Type == null) {
				this.Type = this.Field?.GetPropertyMemberInfo().GetMemberUnderlyingType();
			}

			this.ResolveFieldName();

			_shouldRender = true;

		}
		bool _shouldRender = true;

		protected override bool ShouldRender() {
			if (_shouldRender) {
				return base.ShouldRender();
			}
			return false;
		}

	}

}