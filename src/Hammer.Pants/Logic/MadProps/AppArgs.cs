using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MadProps.AppArgs
{
    public static class AppArgs
    {
        static Regex _pattern = new Regex("[/-](?'key'[^\\s=:]+)"
            + "([=:]("
                + "((?'open'\").+(?'value-open'\"))"
                + "|"
                + "(?'value'.+)"
            + "))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static Regex _uriPattern = new Regex(@"[\\?&](?'key'[^&=]+)(=(?'value'[^&]+))?", RegexOptions.Compiled);
        static Regex _queryStringPattern = new Regex(@"(^|&)(?'key'[^&=]+)(=(?'value'[^&]+))?", RegexOptions.Compiled);

        static IEnumerable<ArgProperty> PropertiesOf<T>()
        {
            return from p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                   let d = p.Attribute<DescriptionAttribute>()
                   let alias = p.Attribute<DisplayAttribute>()
                   select new ArgProperty
                   {
                       Property = p,
                       Name = alias == null || String.IsNullOrWhiteSpace(alias.ShortName) ? p.Name.ToLower() : alias.ShortName,
                       Type = p.PropertyType,
                       Required = p.Attribute<RequiredAttribute>(),
                       RequiresValue = !(p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?)),
                       Description = d != null && !String.IsNullOrWhiteSpace(d.Description) ? d.Description
                          : alias != null ? alias.Description : String.Empty
                   };
        }

        /// <summary>
        /// Parses the arguments in <paramref name="args"/> and creates an instance of <typeparamref name="T"/> with the
        /// corresponding properties populated.
        /// </summary>
        /// <typeparam name="T">The custom type to be populated from <paramref name="args"/>.</typeparam>
        /// <param name="args">Command-line arguments, usually in the form of "/name=value".</param>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T As<T>(this string[] args) where T : new()
        {
            var arguments = (from a in args
                             let match = _pattern.Match(a)
                             where match.Success
                             select new
                             {
                                 Key = match.Groups["key"].Value.ToLower(),
                                 match.Groups["value"].Value
                             }
                            ).ToDictionary(a => a.Key, a => a.Value);

            return arguments.As<T>();
        }

        /// <summary>
        /// Parses the arguments in the supplied string and creates an instance of <typeparamref name="T"/> with the
        /// corresponding properties populated.
        /// The string should be in the format "key1=value1&key2=value2&key3=value3".
        /// </summary>
        /// <typeparam name="T">The custom type to be populated from <paramref name="args"/>.</typeparam>
        /// <param name="args">Command-line arguments, usually in the form of "/name=value".</param>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T As<T>(this string queryString) where T : new()
        {
            var arguments = (from match in _queryStringPattern.Matches(queryString).Cast<Match>()
                             where match.Success
                             select new
                             {
                                 Key = match.Groups["key"].Value.ToLower(),
                                 match.Groups["value"].Value
                             }
                ).ToDictionary(a => a.Key, a => a.Value);

            return arguments.As<T>();
        }

        /// <summary>
        /// Parses the URI parameters in <paramref name="uri"/> and creates an instance of <typeparamref name="T"/> with the
        /// corresponding properties populated.
        /// </summary>
        /// <typeparam name="T">The custom type to be populated from <paramref name="args"/>.</typeparam>
        /// <param name="args">A URI, usually a ClickOnce activation URI.</param>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T As<T>(this Uri uri) where T : new()
        {
            var arguments = (from match in _uriPattern.Matches(uri.ToString()).Cast<Match>()
                             where match.Success
                             select new
                             {
                                 Key = match.Groups["key"].Value.ToLower(),
                                 match.Groups["value"].Value
                             }
                ).ToDictionary(a => a.Key, a => a.Value);

            return arguments.As<T>();
        }

        /// <summary>
        /// Parses the name/value pairs in <paramref name="arguments"/> and creates an instance of <typeparamref name="T"/> with the
        /// corresponding properties populated.
        /// </summary>
        /// <typeparam name="T">The custom type to be populated from <paramref name="args"/>.</typeparam>
        /// <param name="args">The key/value pairs to be parsed.</param>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T As<T>(this Dictionary<string, string> arguments) where T : new()
        {
            T result = new T();

            var props = PropertiesOf<T>().ToList();

            foreach (var arg in arguments)
            {
                var matches = props.Where(p => p.Name.StartsWith(arg.Key)).ToList();
                if (matches.Count == 0)
                {
                    throw new ArgumentException("Unknown argument '" + arg.Key + "'");
                }
                else if (matches.Count > 1)
                {
                    throw new ArgumentException("Ambiguous argument '" + arg.Key + "'");
                }

                var prop = matches[0];

                if (!String.IsNullOrWhiteSpace(arg.Value))
                {
                    if (prop.Type.IsArray)
                    {
                        string v = arg.Value;

                        if (v.StartsWith("{") && v.EndsWith("}"))
                        {
                            v = v.Substring(1, arg.Value.Length - 2);
                        }

                        var values = v.Split(',').ToArray();
                        var array = Array.CreateInstance(prop.Type.GetElementType(), values.Length);
                        for (int i = 0; i < values.Length; i++)
                        {
                            var converter = TypeDescriptor.GetConverter(prop.Type.GetElementType());
                            array.SetValue(converter.ConvertFrom(values[i]), i);
                        }

                        var arrayConverter = new ArrayConverter();
                        prop.Property.SetValue(result, array, null);
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(prop.Type);
                        prop.Property.SetValue(result, converter.ConvertFromString(arg.Value), null);
                    }
                }
                else if (prop.Type == typeof(bool))
                {
                    prop.Property.SetValue(result, true, null);
                }
                else
                {
                    throw new ArgumentException("No value supplied for argument '" + arg.Key + "'");
                }
            }

            foreach (var p in props.Where(p => p.Required != null))
            {
                if (!p.Required.IsValid(p.Property.GetValue(result, null)))
                {
                    throw new ArgumentException("Argument missing: '" + p.Name + "'");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a string describing the arguments necessary to populate an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A class representing the potential application arguments.</typeparam>
        /// <returns>A string describing the arguments necessary to populate an instance of <typeparamref name="T"/></returns>
        public static string HelpFor<T>()
        {
            var props = PropertiesOf<T>().OrderBy(p => p.RequiresValue).ThenBy(p => p.Name).ToList();

            var len = props.Max(p => p.Name.Length);

            var sb = new StringBuilder();

            sb.Append(System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            foreach (var p in props.Where(p => p.Required != null))
            {
                sb.Append(" /" + p.Name + (p.RequiresValue ? "=value" : ""));
            }

            foreach (var p in props.Where(p => p.Required == null))
            {
                sb.Append(" [/" + p.Name + (p.RequiresValue ? "=value" : "") + "]");
            }

            sb.AppendLine();
            sb.AppendLine();
            foreach (var p in props.OrderBy(p => p.Name))
            {
                sb.AppendLine(" /" + p.Name.PadRight(len) + "\t" + p.Description);
            }

            return sb.ToString();
        }

        class ArgProperty
        {
            public PropertyInfo Property { get; set; }
            public string Name { get; set; }
            public RequiredAttribute Required { get; set; }
            public bool RequiresValue { get; set; }
            public Type Type { get; set; }
            public string Description { get; set; }
        }
    }

    public static class PropertyInfoExtensions
    {
        public static T Attribute<T>(this PropertyInfo p)
        {
            return p.GetCustomAttributes(typeof(T), true).Cast<T>().FirstOrDefault();
        }
    }
}
