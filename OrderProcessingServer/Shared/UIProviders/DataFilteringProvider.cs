using System.Collections.Generic;
using System.Linq;
using System;
using OrderProcessingServer.Shared.Dto;

namespace OrderProcessingServer.Shared.UIProviders
{
    /// <summary>
    /// Shared filtering service for GenericGrid and GenericCards components
    /// </summary>
    public class DataFilteringProvider<TItem>
    {
        public static readonly string[] TextOperations = { "Contains", "StartsWith", "EndsWith", "Equals" };

        /// <summary>
        /// Represents the filter state for a column
        /// </summary>
        public class FilterState
        {
            public string TextOp { get; set; } = "Contains";
            public string TextValue { get; set; } = string.Empty;
            public string EnumValue { get; set; } = string.Empty;
            public bool IsEnum { get; set; } = false;
        }

        /// <summary>
        /// Applies all active filters to the given items
        /// </summary>
        public static List<TItem> ApplyFilters(IEnumerable<TItem> items, Dictionary<string, FilterState> columnFilters)
        {
            if (items == null) return new List<TItem>();

            IEnumerable<TItem> query = items;

            // Apply all column filters
            foreach (var filter in columnFilters)
            {
                var field = filter.Key;
                var filterState = filter.Value;

                if (filterState.IsEnum)
                {
                    if (!string.IsNullOrWhiteSpace(filterState.EnumValue))
                    {
                        query = query.Where(item => string.Equals(
                            GetValue(item, field)?.ToString() ?? string.Empty, 
                            filterState.EnumValue, 
                            StringComparison.OrdinalIgnoreCase));
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(filterState.TextValue))
                    {
                        var searchTerm = filterState.TextValue.Trim();
                        query = query.Where(item => MatchText(
                            GetValue(item, field)?.ToString() ?? string.Empty, 
                            searchTerm, 
                            filterState.TextOp));
                    }
                }
            }

            return query.ToList();
        }

        /// <summary>
        /// Applies sorting to the given items
        /// </summary>
        public static List<TItem> ApplySort(List<TItem> items, GridColumnDto sortColumn, bool ascending)
        {
            if (items == null || !items.Any() || sortColumn == null) 
                return items ?? new List<TItem>();

            var sampleItem = items.FirstOrDefault(i => GetValue(i, sortColumn.Field) != null);
            var sampleValue = sampleItem != null ? GetValue(sampleItem, sortColumn.Field) : null;

            if (sampleValue != null)
            {
                var valueType = sampleValue.GetType();
                var typeCode = Type.GetTypeCode(valueType);

                // Handle numeric types
                bool isNumeric = typeCode == TypeCode.Byte || typeCode == TypeCode.SByte || 
                                typeCode == TypeCode.Int16 || typeCode == TypeCode.UInt16 || 
                                typeCode == TypeCode.Int32 || typeCode == TypeCode.UInt32 ||
                                typeCode == TypeCode.Int64 || typeCode == TypeCode.UInt64 || 
                                typeCode == TypeCode.Single || typeCode == TypeCode.Double || 
                                typeCode == TypeCode.Decimal;

                if (isNumeric)
                {
                    return ascending
                        ? items.OrderBy(i => {
                            var v = GetValue(i, sortColumn.Field);
                            try { return Convert.ToDecimal(v ?? 0); }
                            catch { return decimal.MinValue; }
                        }).ToList()
                        : items.OrderByDescending(i => {
                            var v = GetValue(i, sortColumn.Field);
                            try { return Convert.ToDecimal(v ?? 0); }
                            catch { return decimal.MinValue; }
                        }).ToList();
                }

                // Handle DateTime
                if (typeCode == TypeCode.DateTime)
                {
                    return ascending
                        ? items.OrderBy(i => {
                            var v = GetValue(i, sortColumn.Field);
                            return v is DateTime dt ? dt : DateTime.MinValue;
                        }).ToList()
                        : items.OrderByDescending(i => {
                            var v = GetValue(i, sortColumn.Field);
                            return v is DateTime dt ? dt : DateTime.MinValue;
                        }).ToList();
                }
            }

            // Default string sorting
            return ascending
                ? items.OrderBy(i => GetValue(i, sortColumn.Field)?.ToString() ?? string.Empty).ToList()
                : items.OrderByDescending(i => GetValue(i, sortColumn.Field)?.ToString() ?? string.Empty).ToList();
        }

        /// <summary>
        /// Retrieves the value of a field from an item using reflection
        /// Supports nested properties using dot notation (e.g., "Customer.Name")
        /// </summary>
        public static object? GetValue(TItem? item, string field)
        {
            if (item == null) return null;

            var parts = (field ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
            object? current = item;

            foreach (var part in parts)
            {
                if (current == null) return null;
                var property = current.GetType().GetProperty(part);
                if (property == null) return null;
                current = property.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// Matches text value against a search term using the specified operation
        /// </summary>
        public static bool MatchText(string value, string searchTerm, string operation)
        {
            if (value == null) return false;

            return operation switch
            {
                "Contains" => value.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0,
                "StartsWith" => value.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase),
                "EndsWith" => value.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase),
                "Equals" => string.Equals(value, searchTerm, StringComparison.OrdinalIgnoreCase),
                _ => value.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 // Default to Contains
            };
        }

        /// <summary>
        /// Checks if any filters are currently active
        /// </summary>
        public static bool HasActiveFilters(Dictionary<string, FilterState> columnFilters)
        {
            return columnFilters.Any(filter => 
                !string.IsNullOrWhiteSpace(filter.Value.TextValue) || 
                !string.IsNullOrWhiteSpace(filter.Value.EnumValue));
        }

        /// <summary>
        /// Creates a new FilterState for a column
        /// </summary>
        public static FilterState CreateFilterState(bool isEnum = false)
        {
            return new FilterState { IsEnum = isEnum };
        }

        /// <summary>
        /// Updates or creates a text filter for a column
        /// </summary>
        public static void UpdateTextFilter(Dictionary<string, FilterState> columnFilters, string field, string value, string operation = "Contains")
        {
            if (!columnFilters.ContainsKey(field))
            {
                columnFilters[field] = CreateFilterState(isEnum: false);
            }
            columnFilters[field].TextValue = value;
            columnFilters[field].TextOp = operation;
        }

        /// <summary>
        /// Updates or creates an enum filter for a column
        /// </summary>
        public static void UpdateEnumFilter(Dictionary<string, FilterState> columnFilters, string field, string value)
        {
            if (!columnFilters.ContainsKey(field))
            {
                columnFilters[field] = CreateFilterState(isEnum: true);
            }
            columnFilters[field].EnumValue = value;
        }

        /// <summary>
        /// Removes a filter for a specific column
        /// </summary>
        public static void ClearColumnFilter(Dictionary<string, FilterState> columnFilters, string field)
        {
            columnFilters.Remove(field);
        }

        /// <summary>
        /// Clears all filters
        /// </summary>
        public static void ClearAllFilters(Dictionary<string, FilterState> columnFilters)
        {
            columnFilters.Clear();
        }
    }
}
