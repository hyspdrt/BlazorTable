
namespace BlazorTable {

	using System;
	using System.ComponentModel;
	using System.Linq.Expressions;

	public partial class StringFilter<TableItem> {

		protected override void OnInitialized() {

			if (this.Column.Type == typeof(string)) {

				this.Column.FilterControl = this;

				if (this.Column.Filter != null) {

					bool NotCondition = false;

					Expression method = this.Column.Filter.Body;

					if (method is BinaryExpression binary) {
						method = binary.Right;
					}

					if (method is BinaryExpression binary2) {
						NotCondition = binary2.NodeType == ExpressionType.LessThanOrEqual;
						method = binary2.Left;
					}

					if (method is UnaryExpression unary1) {
						NotCondition = unary1.NodeType == ExpressionType.Not;
						method = unary1.Operand;
					}

					if (method is MethodCallExpression methodCall) {
						if (methodCall.Arguments[0] is ConstantExpression constantExpression) {
							this.FilterText = constantExpression.Value?.ToString();
						}

						this.Condition = GetConditionFromMethod(methodCall.Method.Name, NotCondition);
					}
				}
			}
		}

		public override Expression<Func<TableItem, bool>> GetFilter() {

			this.FilterText = this.FilterText?.Trim();

			if (this.Condition != StringCondition.IsNullOrEmpty && this.Condition != StringCondition.IsNotNulOrEmpty && string.IsNullOrEmpty(this.FilterText)) {
				return null;
			}

			return this.Condition switch {
				StringCondition.Contains =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.GreaterThanOrEqual(
								Expression.Call(
									Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
									typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
									new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
								Expression.Constant(0))),
						this.Column.Field.Parameters),

				StringCondition.DoesNotContain =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.LessThanOrEqual(
								Expression.Call(
									Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
									typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
									new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
								Expression.Constant(-1))),
						this.Column.Field.Parameters),

				StringCondition.StartsWith =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Call(
								Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
								typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) }),
								new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
						this.Column.Field.Parameters),

				StringCondition.EndsWith =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Call(
								Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
								typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) }),
								new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
						this.Column.Field.Parameters),

				StringCondition.IsEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Call(
								Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
								typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
								new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
						this.Column.Field.Parameters),

				StringCondition.IsNotEqualTo =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(),
							Expression.Not(
								Expression.Call(
									Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes),
									typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
									new[] { Expression.Constant(this.FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }))),
						this.Column.Field.Parameters),

				StringCondition.IsNullOrEmpty =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.Call(
								typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) }),
							Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes))),
						this.Column.Field.Parameters),

				StringCondition.IsNotNulOrEmpty =>
					Expression.Lambda<Func<TableItem, bool>>(
						Expression.AndAlso(
							this.Column.Field.Body.CreateNullChecks(true),
							Expression.Not(
								Expression.Call(
									typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) }),
							Expression.Call(this.Column.Field.Body, "ToString", Type.EmptyTypes)))),
						this.Column.Field.Parameters),

				_ => throw new ArgumentException(this.Condition + " is not defined!"),

			};

		}


		private string FilterText { get; set; }

		public Type FilterType => typeof(string);

		private static StringCondition GetConditionFromMethod(string method, bool not) {

			if (not) {
				return method switch {
					nameof(string.IndexOf) => StringCondition.DoesNotContain,
					nameof(string.Equals) => StringCondition.IsNotEqualTo,
					nameof(string.IsNullOrEmpty) => StringCondition.IsNotNulOrEmpty,
					_ => throw new InvalidOperationException("Shouldn't be here"),
				};
			}

			return method switch {
				nameof(string.IndexOf) => StringCondition.Contains,
				nameof(string.StartsWith) => StringCondition.StartsWith,
				nameof(string.EndsWith) => StringCondition.EndsWith,
				nameof(string.Equals) => StringCondition.IsEqualTo,
				nameof(string.IsNullOrEmpty) => StringCondition.IsNullOrEmpty,
				_ => throw new InvalidOperationException("Shouldn't be here"),
			};
		}

	}

	public enum StringCondition {

		[Description("Contains")]
		Contains,

		[Description("Does not contain")]
		DoesNotContain,

		[Description("Starts with")]
		StartsWith,

		[Description("Ends with")]
		EndsWith,

		[Description("Is equal to")]
		IsEqualTo,

		[Description("Is not equal to")]
		IsNotEqualTo,

		[Description("Is null or empty")]
		IsNullOrEmpty,

		[Description("Is not null or empty")]
		IsNotNulOrEmpty
	}

}