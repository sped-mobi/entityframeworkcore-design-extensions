// -----------------------------------------------------------------------
// <copyright file="EfDesignerHelper.cs" company="Sped Mobi">
//     Copyright © 2019 Sped Mobi All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    /// <summary>
    /// Defines the <see cref="EfDesignerHelper" />
    /// </summary>
    public class EfDesignerHelper : ICSharpHelper
    {
        /// <summary>
        /// Defines the _builtInTypes
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, string> _builtInTypes = new Dictionary<Type, string>()
    {
      {
        typeof (bool),
        "bool"
      },
      {
        typeof (byte),
        "byte"
      },
      {
        typeof (sbyte),
        "sbyte"
      },
      {
        typeof (char),
        "char"
      },
      {
        typeof (short),
        "short"
      },
      {
        typeof (int),
        "int"
      },
      {
        typeof (long),
        "long"
      },
      {
        typeof (ushort),
        "ushort"
      },
      {
        typeof (uint),
        "uint"
      },
      {
        typeof (ulong),
        "ulong"
      },
      {
        typeof (decimal),
        "decimal"
      },
      {
        typeof (float),
        "float"
      },
      {
        typeof (double),
        "double"
      },
      {
        typeof (string),
        "string"
      },
      {
        typeof (object),
        "object"
      }
    };

        /// <summary>
        /// Defines the _keywords
        /// </summary>
        private static readonly IReadOnlyCollection<string> _keywords = new string[81]
        {
      "__arglist",
      "__makeref",
      "__reftype",
      "__refvalue",
      "abstract",
      "as",
      "base",
      "bool",
      "break",
      "byte",
      "case",
      "catch",
      "char",
      "checked",
      "class",
      "const",
      "continue",
      "decimal",
      "default",
      "delegate",
      "do",
      "double",
      "else",
      "enum",
      "event",
      "explicit",
      "extern",
      "false",
      "finally",
      "fixed",
      "float",
      "for",
      "foreach",
      "goto",
      "if",
      "implicit",
      "in",
      "int",
      "interface",
      "internal",
      "is",
      "lock",
      "long",
      "namespace",
      "new",
      "null",
      "object",
      "operator",
      "out",
      "override",
      "params",
      "private",
      "protected",
      "public",
      "readonly",
      "ref",
      "return",
      "sbyte",
      "sealed",
      "short",
      "sizeof",
      "stackalloc",
      "static",
      "string",
      "struct",
      "switch",
      "this",
      "throw",
      "true",
      "try",
      "typeof",
      "uint",
      "ulong",
      "unchecked",
      "unsafe",
      "ushort",
      "using",
      "virtual",
      "void",
      "volatile",
      "while"
        };

        /// <summary>
        /// Defines the _literalFuncs
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, Func<EfDesignerHelper, object, string>> _literalFuncs = new Dictionary<Type, Func<EfDesignerHelper, object, string>>()
    {
      {
        typeof (bool),
         (c, v) => c.Literal((bool) v)
      },
      {
        typeof (byte),
         (c, v) => c.Literal((byte) v)
      },
      {
        typeof (byte[]),
         (c, v) => c.Literal((byte[]) v)
      },
      {
        typeof (char),
         (c, v) => c.Literal((char) v)
      },
      {
        typeof (DateTime),
         (c, v) => c.Literal((DateTime) v)
      },
      {
        typeof (DateTimeOffset),
         (c, v) => c.Literal((DateTimeOffset) v)
      },
      {
        typeof (decimal),
         (c, v) => c.Literal((decimal) v)
      },
      {
        typeof (double),
         (c, v) => c.Literal((double) v)
      },
      {
        typeof (float),
         (c, v) => c.Literal((float) v)
      },
      {
        typeof (Guid),
         (c, v) => c.Literal((Guid) v)
      },
      {
        typeof (int),
         (c, v) => c.Literal((int) v)
      },
      {
        typeof (long),
         (c, v) => c.Literal((long) v)
      },
      {
        typeof (NestedClosureCodeFragment),
         (c, v) => c.Fragment((NestedClosureCodeFragment) v)
      },
      {
        typeof (object[]),
         (c, v) => c.Literal( (object[]) v)
      },
      {
        typeof (object[,]),
         (c, v) => c.Literal((object[,]) v)
      },
      {
        typeof (sbyte),
         (c, v) => c.Literal((sbyte) v)
      },
      {
        typeof (short),
         (c, v) => c.Literal((short) v)
      },
      {
        typeof (string),
         (c, v) => c.Literal((string) v)
      },
      {
        typeof (TimeSpan),
         (c, v) => c.Literal((TimeSpan) v)
      },
      {
        typeof (uint),
         (c, v) => c.Literal((uint) v)
      },
      {
        typeof (ulong),
         (c, v) => c.Literal((ulong) v)
      },
      {
        typeof (ushort),
         (c, v) => c.Literal((ushort) v)
      },
      {
        typeof (BigInteger),
         (c, v) => c.Literal((BigInteger) v)
      }
    };

        /// <summary>
        /// Defines the _relationalTypeMappingSource
        /// </summary>
        private readonly IRelationalTypeMappingSource _relationalTypeMappingSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfDesignerHelper"/> class.
        /// </summary>
        /// <param name="relationalTypeMappingSource">The relationalTypeMappingSource<see cref="IRelationalTypeMappingSource"/></param>
        public EfDesignerHelper(
          [NotNull] IRelationalTypeMappingSource relationalTypeMappingSource)
        {
            _relationalTypeMappingSource = relationalTypeMappingSource;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="properties">The properties<see cref="IReadOnlyList{string}"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Lambda(IReadOnlyList<string> properties)
        {
            Check.NotNull<IReadOnlyList<string>>(properties, nameof(properties));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("x => ");
            if (properties.Count == 1)
            {
                stringBuilder.Append("x.").Append(properties[0]);
            }
            else
            {
                stringBuilder.Append("new { ");
                stringBuilder.Append(string.Join(", ", properties.Select<string, string>(p => "x." + p)));
                stringBuilder.Append(" }");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Reference(Type type)
        {
            return Reference(type, false);
        }

        /// <summary>
        /// The Reference
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <param name="useFullName">The useFullName<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string Reference(Type type, bool useFullName)
        {
            Check.NotNull<Type>(type, nameof(type));
            if (_builtInTypes.TryGetValue(type, out string str))
                return str;
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return Reference(SharedTypeExtensions.UnwrapNullableType(type)) + "?";
            StringBuilder stringBuilder = new StringBuilder();
            if (type.IsArray)
            {
                stringBuilder.Append(Reference(type.GetElementType())).Append("[");
                int arrayRank = type.GetArrayRank();
                for (int index = 1; index < arrayRank; ++index)
                    stringBuilder.Append(",");
                stringBuilder.Append("]");
                return stringBuilder.ToString();
            }
            if (type.IsNested)
                stringBuilder.Append(Reference(type.DeclaringType)).Append(".");
            stringBuilder.Append(useFullName ? type.DisplayName(true) : type.ShortDisplayName());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <param name="scope">The scope<see cref="ICollection{string}"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Identifier(string name, ICollection<string> scope = null)
        {
            Check.NotEmpty(name, nameof(name));
            StringBuilder stringBuilder = new StringBuilder();
            int startIndex = 0;
            for (int index = 0; index < name.Length; ++index)
            {
                if (!IsIdentifierPartCharacter(name[index]))
                {
                    if (startIndex != index)
                        stringBuilder.Append(name, startIndex, index - startIndex);
                    startIndex = index + 1;
                }
            }
            if (startIndex != name.Length)
                stringBuilder.Append(name.Substring(startIndex));
            if (stringBuilder.Length == 0 || !IsIdentifierStartCharacter(stringBuilder[0]))
                stringBuilder.Insert(0, "_");
            string str1 = stringBuilder.ToString();
            if (scope != null)
            {
                string str2 = str1;
                int num = 0;
                while (scope.Contains(str2))
                    str2 = str1 + num++;
                scope.Add(str2);
                str1 = str2;
            }
            if (!_keywords.Contains<string>(str1))
                return str1;
            return "@" + str1;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="name">The name<see cref="string[]"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Namespace(params string[] name)
        {
            Check.NotNull<string[]>(name, nameof(name));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string name1 in ((IEnumerable<string>)name).Where<string>(p => !string.IsNullOrEmpty(p)).SelectMany<string, string>(p => p.Split(new char[1]
         {
        '.'
         }, StringSplitOptions.RemoveEmptyEntries)))
            {
                string str = Identifier(name1, null);
                if (!string.IsNullOrEmpty(str))
                    stringBuilder.Append(str).Append('.');
            }
            if (stringBuilder.Length <= 0)
                return "_";
            return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(string value)
        {
            if (!value.Contains<char>('\n') && !value.Contains<char>('\r'))
                return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            return "@\"" + value.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(bool value)
        {
            return !value ? "false" : "true";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="byte"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(byte value)
        {
            return "(byte)" + value;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="values">The values<see cref="byte[]"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(byte[] values)
        {
            return "new byte[] { " + string.Join<byte>(", ", values) + " }";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="char"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(char value)
        {
            return "'" + (value == '\'' ? "\\'" : value.ToString()) + "'";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="DateTime"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(DateTime value)
        {
            return string.Format(CultureInfo.InvariantCulture, "new DateTime({0}, {1}, {2}, {3}, {4}, {5}, {6}, DateTimeKind.{7})", value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind) + (value.Ticks % 10000L == 0L ? "" : string.Format(CultureInfo.InvariantCulture, ".AddTicks({0})", value.Ticks % 10000L));
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="DateTimeOffset"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(DateTimeOffset value)
        {
            return "new DateTimeOffset(" + Literal(value.DateTime) + ", " + Literal(value.Offset) + ")";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="decimal"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "m";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="double"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(double value)
        {
            return EnsureDecimalPlaces(value);
        }

        /// <summary>
        /// The EnsureDecimalPlaces
        /// </summary>
        /// <param name="number">The number<see cref="double"/></param>
        /// <returns>The <see cref="string"/></returns>
        private static string EnsureDecimalPlaces(double number)
        {
            string str = number.ToString("G17", CultureInfo.InvariantCulture);
            if (double.IsNaN(number))
                return "double.NaN";
            if (double.IsNegativeInfinity(number))
                return "double.NegativeInfinity";
            if (double.IsPositiveInfinity(number))
                return "double.PositiveInfinity";
            if (str.Contains("E") || str.Contains("e") || str.Contains("."))
                return str;
            return str + ".0";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="float"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "f";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="Guid"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(Guid value)
        {
            return "new Guid(\"" + value + "\")";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="long"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "L";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="sbyte"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(sbyte value)
        {
            return "(sbyte)" + value;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="short"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(short value)
        {
            return "(short)" + value;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="TimeSpan"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(TimeSpan value)
        {
            if (value.Ticks % 10000L != 0L)
                return string.Format(CultureInfo.InvariantCulture, "new TimeSpan({0})", value.Ticks);
            return string.Format(CultureInfo.InvariantCulture, "new TimeSpan({0}, {1}, {2}, {3}, {4})", value.Days, value.Hours, value.Minutes, value.Seconds, value.Milliseconds);
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="uint"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(uint value)
        {
            return ((int)value).ToString() + "u";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="ulong"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(ulong value)
        {
            return ((long)value).ToString() + "ul";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="ushort"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(ushort value)
        {
            return "(ushort)" + value;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="BigInteger"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(BigInteger value)
        {
            return "BigInteger.Parse(\"" + value.ToString(NumberFormatInfo.InvariantInfo) + "\", NumberFormatInfo.InvariantInfo)";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value<see cref="T?"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal<T>(T? value) where T : struct
        {
            return UnknownLiteral(value);
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values<see cref="IReadOnlyList{T}"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal<T>(IReadOnlyList<T> values)
        {
            return Array(values);
        }

        /// <summary>
        /// The Array
        /// </summary>
        /// <param name="values">The values<see cref="IEnumerable"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string Array(IEnumerable values)
        {
            return "new[] { " + string.Join(", ", values.Cast<object>().Select<object, string>(new Func<object, string>(UnknownLiteral))) + " }";
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="values">The values<see cref="IReadOnlyList{object}"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(IReadOnlyList<object> values)
        {
            return Literal(values, false);
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="values">The values<see cref="IReadOnlyList{object}"/></param>
        /// <param name="vertical">The vertical<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(IReadOnlyList<object> values, bool vertical)
        {
            if (!vertical)
                return "new object[] { " + string.Join(", ", values.Select<object, string>(new Func<object, string>(UnknownLiteral))) + " }";
            IndentedStringBuilder indentedStringBuilder = new IndentedStringBuilder();
            indentedStringBuilder.AppendLine("new object[]").AppendLine("{");
            using (indentedStringBuilder.Indent())
            {
                for (int index = 0; index < values.Count; ++index)
                {
                    if (index != 0)
                        indentedStringBuilder.AppendLine(",");
                    indentedStringBuilder.Append(UnknownLiteral(values[index]));
                }
            }
            indentedStringBuilder.AppendLine().Append("}");
            return indentedStringBuilder.ToString();
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="values">The values<see cref="object[,]"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(object[,] values)
        {
            IndentedStringBuilder indentedStringBuilder = new IndentedStringBuilder();
            indentedStringBuilder.AppendLine("new object[,]").AppendLine("{");
            using (indentedStringBuilder.Indent())
            {
                int length1 = values.GetLength(0);
                int length2 = values.GetLength(1);
                for (int index1 = 0; index1 < length1; ++index1)
                {
                    if (index1 != 0)
                        indentedStringBuilder.AppendLine(",");
                    indentedStringBuilder.Append("{ ");
                    for (int index2 = 0; index2 < length2; ++index2)
                    {
                        if (index2 != 0)
                            indentedStringBuilder.Append(", ");
                        indentedStringBuilder.Append(UnknownLiteral(values[index1, index2]));
                    }
                    indentedStringBuilder.Append(" }");
                }
            }
            indentedStringBuilder.AppendLine().Append("}");
            return indentedStringBuilder.ToString();
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="Enum"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Literal(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
                return GetSimpleEnumValue(type, name);
            return GetCompositeEnumValue(type, value);
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        protected virtual string GetSimpleEnumValue(Type type, string name)
        {
            return Reference(type) + "." + name;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <param name="flags">The flags<see cref="Enum"/></param>
        /// <returns>The <see cref="string"/></returns>
        protected virtual string GetCompositeEnumValue(Type type, Enum flags)
        {
            HashSet<Enum> source = new HashSet<Enum>(GetFlags(flags));
            foreach (Enum @enum in source.ToList<Enum>())
            {
                Enum currentValue = @enum;
                IReadOnlyCollection<Enum> flags1 = GetFlags(currentValue);
                if (flags1.Count > 1)
                    source.ExceptWith(flags1.Where<Enum>(v => !Equals(v, currentValue)));
            }
            return source.Aggregate<Enum, string>(null, (previous, current) =>
            {
                if (previous != null)
                    return previous + " | " + GetSimpleEnumValue(type, Enum.GetName(type, current));
                return GetSimpleEnumValue(type, Enum.GetName(type, current));
            });
        }

        /// <summary>
        /// The GetFlags
        /// </summary>
        /// <param name="flags">The flags<see cref="Enum"/></param>
        /// <returns>The <see cref="IReadOnlyCollection{Enum}"/></returns>
        internal static IReadOnlyCollection<Enum> GetFlags(Enum flags)
        {
            List<Enum> enumList = new List<Enum>();
            Type type = flags.GetType();
            object obj = Enum.ToObject(type, 0);
            foreach (Enum flag in Enum.GetValues(type))
            {
                if (!flag.Equals(obj) && flags.HasFlag(flag))
                    enumList.Add(flag);
            }
            return enumList;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="value">The value<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string UnknownLiteral(object value)
        {
            if (value == null || value == DBNull.Value)
                return "null";
            Type type = value.GetType();
            if (_literalFuncs.TryGetValue(SharedTypeExtensions.UnwrapNullableType(type), out Func<EfDesignerHelper, object, string> func))
                return func(this, value);
            if (value is Enum @enum)
                return Literal(@enum);
            if (value is Array array)
                return Array(array);
            RelationalTypeMapping mapping = _relationalTypeMappingSource.FindMapping(type);
            if (mapping == null)
                throw new InvalidOperationException();
            StringBuilder builder = new StringBuilder();
            Expression codeLiteral = mapping.GenerateCodeLiteral(value);
            if (!HandleExpression(codeLiteral, builder, false))
                throw new NotSupportedException();
            return builder.ToString();
        }

        /// <summary>
        /// The HandleExpression
        /// </summary>
        /// <param name="expression">The expression<see cref="Expression"/></param>
        /// <param name="builder">The builder<see cref="StringBuilder"/></param>
        /// <param name="simple">The simple<see cref="bool"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool HandleExpression(Expression expression, StringBuilder builder, bool simple = false)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
                    if (methodCallExpression.Method.IsStatic)
                        builder.Append(Reference(methodCallExpression.Method.DeclaringType, true));
                    else if (!HandleExpression(methodCallExpression.Object, builder, false))
                        return false;
                    builder.Append('.').Append(methodCallExpression.Method.Name);
                    return HandleArguments(methodCallExpression.Arguments, builder);
                case ExpressionType.Constant:
                    object obj1 = ((ConstantExpression)expression).Value;
                    StringBuilder stringBuilder = builder;
                    object obj2;
                    if (simple && obj1 != null)
                    {
                        Type type = obj1.GetType();
                        bool? nullable = (object)type != null ? new bool?(SharedTypeExtensions.IsNumeric(type)) : new bool?();
                        bool flag = true;
                        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
                        {
                            obj2 = obj1;
                            goto label_13;
                        }
                    }
                    obj2 = UnknownLiteral(obj1);
                label_13:
                    stringBuilder.Append(obj2);
                    return true;
                case ExpressionType.Convert:
                    builder.Append('(').Append(Reference(expression.Type, true)).Append(')');
                    return HandleExpression(((UnaryExpression)expression).Operand, builder, false);
                case ExpressionType.New:
                    builder.Append("new ").Append(Reference(expression.Type, true));
                    return HandleArguments(((NewExpression)expression).Arguments, builder);
                case ExpressionType.NewArrayInit:
                    builder.Append("new ").Append(Reference(expression.Type.GetElementType())).Append("[] { ");
                    HandleList(((NewArrayExpression)expression).Expressions, builder, true);
                    builder.Append(" }");
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The HandleArguments
        /// </summary>
        /// <param name="argumentExpressions">The argumentExpressions<see cref="IEnumerable{Expression}"/></param>
        /// <param name="builder">The builder<see cref="StringBuilder"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool HandleArguments(IEnumerable<Expression> argumentExpressions, StringBuilder builder)
        {
            builder.Append('(');
            if (!HandleList(argumentExpressions, builder, false))
                return false;
            builder.Append(')');
            return true;
        }

        /// <summary>
        /// The HandleList
        /// </summary>
        /// <param name="argumentExpressions">The argumentExpressions<see cref="IEnumerable{Expression}"/></param>
        /// <param name="builder">The builder<see cref="StringBuilder"/></param>
        /// <param name="simple">The simple<see cref="bool"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool HandleList(
          IEnumerable<Expression> argumentExpressions,
          StringBuilder builder,
          bool simple = false)
        {
            string str = string.Empty;
            foreach (Expression argumentExpression in argumentExpressions)
            {
                builder.Append(str);
                if (!HandleExpression(argumentExpression, builder, simple))
                    return false;
                str = ", ";
            }
            return true;
        }

        /// <summary>
        /// This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="fragment">The fragment<see cref="MethodCallCodeFragment"/></param>
        /// <returns>The <see cref="string"/></returns>
        public virtual string Fragment(MethodCallCodeFragment fragment)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (MethodCallCodeFragment callCodeFragment = fragment; callCodeFragment != null; callCodeFragment = callCodeFragment.ChainedCall)
            {
                stringBuilder.Append(".").Append(callCodeFragment.Method).Append("(");
                for (int index = 0; index < callCodeFragment.Arguments.Count; ++index)
                {
                    if (index != 0)
                        stringBuilder.Append(", ");
                    stringBuilder.Append(UnknownLiteral(callCodeFragment.Arguments[index]));
                }
                stringBuilder.Append(")");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// The Fragment
        /// </summary>
        /// <param name="fragment">The fragment<see cref="NestedClosureCodeFragment"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string Fragment(NestedClosureCodeFragment fragment)
        {
            return fragment.Parameter + " => " + fragment.Parameter + Fragment(fragment.MethodCall);
        }

        /// <summary>
        /// The IsIdentifierStartCharacter
        /// </summary>
        /// <param name="ch">The ch<see cref="char"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsIdentifierStartCharacter(char ch)
        {
            if (ch < 'a')
            {
                if (ch < 'A')
                    return false;
                if (ch > 'Z')
                    return ch == '_';
                return true;
            }
            if (ch <= 'z')
                return true;
            if (ch > '\x007F')
                return IsLetterChar(CharUnicodeInfo.GetUnicodeCategory(ch));
            return false;
        }

        /// <summary>
        /// The IsIdentifierPartCharacter
        /// </summary>
        /// <param name="ch">The ch<see cref="char"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsIdentifierPartCharacter(char ch)
        {
            if (ch < 'a')
            {
                if (ch >= 'A')
                {
                    if (ch > 'Z')
                        return ch == '_';
                    return true;
                }
                if (ch >= '0')
                    return ch <= '9';
                return false;
            }
            if (ch <= 'z')
                return true;
            if (ch <= '\x007F')
                return false;
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (IsLetterChar(unicodeCategory))
                return true;
            switch (unicodeCategory)
            {
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.Format:
                case UnicodeCategory.ConnectorPunctuation:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The IsLetterChar
        /// </summary>
        /// <param name="cat">The cat<see cref="UnicodeCategory"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private static bool IsLetterChar(UnicodeCategory cat)
        {
            switch (cat)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.LetterNumber:
                    return true;
                default:
                    return false;
            }
        }
    }
}
