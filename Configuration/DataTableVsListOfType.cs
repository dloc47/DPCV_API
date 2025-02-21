using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DPCV_API.Configuration
{
    public static class DataTableVsListOfType
    {
        /// <summary>
        /// Converts a DataTable to a List of the specified type.
        /// </summary>
        public static List<T> ConvertToTargetTypeList<T>(this DataTable dataTable) where T : class, new()
        {
            List<T> list = new List<T>();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;

            foreach (var row in dataTable.AsEnumerable())
            {
                T obj = new T();
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (dataTable.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType));
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Converts a list of objects to a DataTable.
        /// </summary>
        public static DataTable ConvertToDataTable<T>(this IList<T> data)
        {
            DataTable dt = new DataTable(typeof(T).Name);
            if (data == null || !data.Any()) return dt;

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                var values = props.Select(prop => prop.GetValue(item) ?? DBNull.Value).ToArray();
                dt.Rows.Add(values);
            }
            return dt;
        }

        /// <summary>
        /// Converts a DataTable to a List.
        /// </summary>
        public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : class, new()
        {
            return dt.AsEnumerable().Select(row => GetItem<T>(row)).ToList();
        }

        /// <summary>
        /// Converts DataRow array to a List.
        /// </summary>
        public static List<T> ConvertDataRowToList<T>(DataRow[] dataRows) where T : class, new()
        {
            return dataRows.Select(GetItem<T>).ToList();
        }

        /// <summary>
        /// Converts a single DataRow to an object.
        /// </summary>
        private static T GetItem<T>(DataRow dr) where T : class, new()
        {
            T obj = new T();
            Type temp = typeof(T);

            foreach (DataColumn column in dr.Table.Columns)
            {
                PropertyInfo prop = temp.GetProperty(column.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && dr[column] != DBNull.Value)
                {
                    prop.SetValue(obj, Convert.ChangeType(dr[column], prop.PropertyType));
                }
            }
            return obj;
        }
    }
}
