
namespace BlazorTable {

	using Microsoft.AspNetCore.Components;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;

	public partial class CustomSelect<TableItem> : ICustomSelect {

		protected override void OnInitialized() {

			this.Column.FilterControl = this;

			if (this.Column.Filter?.Body is BinaryExpression binaryExpression
				&& binaryExpression.Right is BinaryExpression logicalBinary
				&& logicalBinary.Right is ConstantExpression constant) {
				switch (logicalBinary.NodeType) {
					case ExpressionType.Equal:
						this.Condition = constant.Value == null ? CustomSelectCondition.IsNull : CustomSelectCondition.IsEqualTo;
						break;
					case ExpressionType.NotEqual:
						this.Condition = constant.Value == null ? CustomSelectCondition.IsNotNull : CustomSelectCondition.IsNotEqualTo;
						break;
				}

				this.FilterValue = constant.Value;
			}
		}

		public override Expression<Func<TableItem, bool>> GetFilter() {

			return this.Condition switch {
				CustomSelectCondition.IsEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Equal(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(Convert.ChangeType(this.FilterValue, this.Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
						this.Column.Field.Parameters),

				CustomSelectCondition.IsNotEqualTo => Expression.Lambda<Func<TableItem, bool>>(
					Expression.AndAlso(
						this.Column.Field.Body.CreateNullChecks(),
						Expression.NotEqual(
							Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
							Expression.Constant(Convert.ChangeType(this.FilterValue, this.Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
					this.Column.Field.Parameters),

				CustomSelectCondition.IsNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.Equal(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				CustomSelectCondition.IsNotNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.NotEqual(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				_ => throw new ArgumentException(this.Condition + " is not defined!"),
			};
		}

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		private readonly List<KeyValuePair<string, object>> Items = new List<KeyValuePair<string, object>>();

		private object FilterValue { get; set; }

		public void AddSelect(string key, object value) {

			this.Items.Add(new KeyValuePair<string, object>(key, value));

			if (this.FilterValue == null) {
				this.FilterValue = this.Items.FirstOrDefault().Value;
			}

			this.StateHasChanged();

		}

		public void OnItemChanged(ChangeEventArgs args) {
			this.FilterValue = args.Value;
		}

	}

	public enum CustomSelectCondition {

		[Description("Is equal to")]
		IsEqualTo,

		[Description("Is not equal to")]
		IsNotEqualTo,

		[Description("Is null")]
		IsNull,

		[Description("Is not null")]
		IsNotNull
	}

}