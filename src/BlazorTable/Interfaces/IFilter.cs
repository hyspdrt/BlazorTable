
namespace BlazorTable {

	using Microsoft.AspNetCore.Components;
	using System;
	using System.Linq.Expressions;

	/// <summary>
	/// Filter Component Interface
	/// </summary>
	/// <typeparam name="TableItem"></typeparam>
	public interface IFilter<TableItem> {

		/// <summary>
		/// The Cascaded Column Parameter
		/// </summary>
		IColumn<TableItem> Column { get; set; }

		/// <summary>
		/// Gets the Filter Expression used to filter records.
		/// </summary>
		/// <returns></returns>
		Expression<Func<TableItem, bool>> GetFilter();

		/// <summary>
		/// Handle user selection.
		/// </summary>
		/// <param name="args"></param>
		void OnFilterChange(ChangeEventArgs args);

	}

	/// <summary>
	/// Filter Component Interface
	/// </summary>
	/// <typeparam name="TableItem"></typeparam>
	public interface IFilter<TableItem, TEnum> : IFilter<TableItem> where TEnum : struct, Enum {

		/// <summary>
		/// The selected condition value
		/// </summary>
		TEnum Condition { get; set; }

		/// <summary>
		/// Collection of Conditions to populate the Select Options with.
		/// </summary>
		TEnum[] Options { get; }

	}

}