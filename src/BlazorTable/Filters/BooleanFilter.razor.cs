
namespace BlazorTable {

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq.Expressions;

	public partial class BooleanFilter<TableItem> {

		public List<Type> FilterTypes => new List<Type>()
		{
			typeof(bool)
		};

		protected override void OnInitialized() {

			if (this.FilterTypes.Contains(this.Column.Type.GetNonNullableType())) {

				this.Column.FilterControl = this;

				if (this.Column.Filter != null) {
					var nodeType = this.Column.Filter.Body.NodeType;

					if (this.Column.Filter.Body is BinaryExpression binaryExpression
						&& binaryExpression.NodeType == ExpressionType.AndAlso) {
						nodeType = binaryExpression.Right.NodeType;
					}

					switch (nodeType) {
						case ExpressionType.IsTrue:
							this.Condition = BooleanCondition.True;
							break;
						case ExpressionType.IsFalse:
							this.Condition = BooleanCondition.False;
							break;
						case ExpressionType.Equal:
							this.Condition = BooleanCondition.IsNull;
							break;
						case ExpressionType.NotEqual:
							this.Condition = BooleanCondition.IsNotNull;
							break;
					}
				}
			}
		}

		public override Expression<Func<TableItem, bool>> GetFilter() {
			return this.Condition switch {
				BooleanCondition.True =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.IsTrue(Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()))),
						this.Column.Field.Parameters),

				BooleanCondition.False =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.IsFalse(Expression.Convert(this.Column.Field.Body, this.Column.Type.GetNonNullableType()))),
							this.Column.Field.Parameters),

				BooleanCondition.IsNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.Equal(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				BooleanCondition.IsNotNull =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.NotEqual(this.Column.Field.Body, Expression.Constant(null))),
						this.Column.Field.Parameters),

				_ => null,
			};

		}

	}

	public enum BooleanCondition {

		[Description("True")]
		True,

		[Description("False")]
		False,

		[Description("Is null")]
		IsNull,

		[Description("Is not null")]
		IsNotNull
	}

}