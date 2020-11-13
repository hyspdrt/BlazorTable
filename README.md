# BlazorTable

Blazor Table Component with Sorting, Paging and Filtering

*FORKED:* This is forked version and does not include the build pipeline or actions.


[![Sample Gif](https://raw.githubusercontent.com/IvanJosipovic/BlazorTable/master/BlazorTable.gif)](/BlazorTable.gif)

## Install

- None, source code only
 
## Features
- Column Reordering
- Edit Mode ([Template Switching](https://github.com/IvanJosipovic/BlazorTable/blob/master/src/BlazorTable.Sample.Shared/Pages/EditMode.razor))
- Client Side
	- Paging
	- Sorting
	- Filtering
		- Strings
		- Numbers
		- Dates
		- Enums
		- Custom Component
## Dependencies
- Bootstrap 4 CSS

## Sample
[Example Page](https://github.com/IvanJosipovic/BlazorTable/blob/master/src/BlazorTable.Sample.Shared/Pages/Index.razor)

```csharp
<Table TableItem="PersonData" Items="data" PageSize="10">
	<Column TableItem="PersonData" Title="Id" Field="@(x => x.id)" Sortable="true" Filterable="true" Width="10%" />
	<Column TableItem="PersonData" Title="First Name" Field="@(x => x.first_name)" Sortable="true" Filterable="true" Width="20%" />
	<Column TableItem="PersonData" Title="Last Name" Field="@(x => x.last_name)" Sortable="true" Filterable="true" Width="20%" />
	<Column TableItem="PersonData" Title="Email" Field="@(x => x.email)" Sortable="true" Filterable="true" Width="20%">
		<Template>
			<a href="mailto:@context.email">@context.email</a>
		</Template>
	</Column>
	<Column TableItem="PersonData" Title="Confirmed" Field="@(x => x.confirmed)" Sortable="true" Filterable="true" Width="10%" />
	<Column TableItem="PersonData" Title="Price" Field="@(x => x.price)" Sortable="true" Filterable="true" Width="10%" Format="C" Align="Align.Right" />
	<Column TableItem="PersonData" Title="Created Date" Field="@(x => x.created_date)" Sortable="true" Width="10%">
		<Template>
			@context.created_date.ToShortDateString()
		</Template>
	</Column>
	<Pager ShowPageNumber="true" ShowTotalCount="true" />
</Table>
```
