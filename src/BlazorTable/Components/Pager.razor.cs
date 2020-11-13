﻿
namespace BlazorTable {

	using Microsoft.AspNetCore.Components;
	using System.Collections.Generic;

	/// <summary>
	/// BlazorTable Pager
	/// </summary>
	public partial class Pager : ComponentBase {

		/// <summary>
		/// 
		/// </summary>
		[CascadingParameter(Name = "Table")]
		public ITable Table { get; set; }

		/// <summary>
		/// Always show Pager, otherwise only show if TotalPages > 1
		/// </summary>
		[Parameter]
		public bool AlwaysShow { get; set; }

		/// <summary>
		/// Show current page number
		/// </summary>
		[Parameter]
		public bool ShowPageNumber { get; set; }

		/// <summary>
		/// Show total item count
		/// </summary>
		[Parameter]
		public bool ShowTotalCount { get; set; }

		/// <summary>
		/// Page size options
		/// </summary>
		[Parameter]
		public List<int> PageSizes { get; set; } = new List<int>() { 10, 25, 50 };

		/// <summary>
		/// Show Page Size Options
		/// </summary>
		[Parameter]
		public bool ShowPageSizes { get; set; }

		private ElementReference FirstPageElementRef { get; set; }
		private void SetFirstPageButtonFocus() {
			InvokeAsync(async () => {
				await FirstPageElementRef.FocusAsync();
			});
		}
		private void SetPageSize(ChangeEventArgs args) {
			if (int.TryParse(args.Value.ToString(), out int result)) {
				Table.SetPageSize(result);
			}
		}

	}

}