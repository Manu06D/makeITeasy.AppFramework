using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public static class ObjectExtension
    {
        public static object ToObject(this IDictionary<string, object> input, object output)
        {
            Type inputType = output.GetType();

            foreach (KeyValuePair<string, object> item in input)
            {
                try
                {
                    PropertyInfo property = inputType.GetProperty(item.Key);
                    if (property != null)
                    {
                        if (property.PropertyType?.BaseType == typeof(Array))
                        {
                            //if (item.Value?.GetType() == typeof(JArray) && item.Value is JArray j)
                            //{
                            //    object x = j.ToObject(property.PropertyType);
                            //    property.SetValue(output, x, null);
                            //}
                        }
                        else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            object converted = DateTime.Parse(item.Value.ToString());
                            property.SetValue(output, converted, null);
                        }
                        else if (property.PropertyType.IsEnum)
                        {
                            object converted = GetEnumValue(item, property.PropertyType);
                            property.SetValue(output, converted, null);
                        }
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type targetType = property.PropertyType.GetGenericArguments()[0];
                            object converted = null;
                            if (targetType.IsEnum)
                            {
                                converted = GetEnumValue(item, targetType);
                            }
                            else
                            {
                                converted = Convert.ChangeType(item.Value.ToString(), targetType);
                            }

                            if (converted != null)
                            {
                                property.SetValue(output, converted, null);
                            }
                        }
                        else
                        {
                            object converted = Convert.ChangeType(item.Value.ToString(), property.PropertyType);

                            if (converted is string)
                            {
                                converted = (converted as string).Trim();
                            }

                            property.SetValue(output, converted, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO Add log
                    string error = ex.Message;
                }
            }
            return output;
        }

        private static object GetEnumValue(KeyValuePair<string, object> item, Type targetType) => Enum.Parse(targetType, item.Value.ToString());
    }
}
