
namespace BlazorTable {

	using System;
	using System.Linq.Expressions;

	public partial class DateFilter<TableItem> {

		private DateTime FilterValue { get; set; } = DateTime.Now;

		protected override void OnInitialized() {

			if (this.Column.Type.GetNonNullableType() == typeof(DateTime)) {

				this.Column.FilterControl = this;

				if (this.Column.Filter?.Body is BinaryExpression binaryExpression
					&& binaryExpression.Right is BinaryExpression logicalBinary
					&& logicalBinary.Right is ConstantExpression constant) {

					switch (binaryExpression.Right.NodeType) {
						case ExpressionType.Equal:
							this.Condition = constant.Value == null ? NumberCondition.IsNull : NumberCondition.IsEqualTo;
							break;
						case ExpressionType.NotEqual:
							this.Condition = constant.Value == null ? NumberCondition.IsNotNull : NumberCondition.IsNotEqualTo;
							break;
						case ExpressionType.GreaterThanOrEqual:
							this.Condition = NumberCondition.IsGreaterThanOrEqualTo;
							break;
						case ExpressionType.GreaterThan:
							this.Condition = NumberCondition.IsGreaterThan;
							break;
						case ExpressionType.LessThanOrEqual:
							this.Condition = NumberCondition.IsLessThanOrEqualTo;
							break;
						case ExpressionType.LessThan:
							this.Condition = NumberCondition.IsLessThan;
							break;
					}

					if (constant.Value != null && DateTime.TryParse(constant.Value.ToString(), out DateTime result)) {
						this.FilterValue = result;
					}

				}

			}

		}

		public override Expression<Func<TableItem, bool>> GetFilter() {

			return this.Condition switch {
				NumberCondition.IsEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Equal(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsNotEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.NotEqual(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsGreaterThanOrEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.GreaterThanOrEqual(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsGreaterThan =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.GreaterThan(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsLessThanOrEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.LessThanOrEqual(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsLessThan =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.LessThan(
								Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()),
								Expression.Constant(this.FilterValue))),
						this.Column.Field.Parameters),

				NumberCondition.IsNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.Equal(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				NumberCondition.IsNotNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.NotEqual(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				_ => throw new ArgumentException(this.Condition + " is not defined!"),

			};

		}

	}

}