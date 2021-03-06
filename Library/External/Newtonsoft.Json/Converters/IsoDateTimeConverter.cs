/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
  /// <summary>
  /// Converts a <see cref="DateTime"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
  /// </summary>
  public class IsoDateTimeConverter : DateTimeConverterBase
  {
    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

    private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
    private string _dateTimeFormat;
    private CultureInfo _culture;

    /// <summary>
    /// Gets or sets the date time styles used when converting a date to and from JSON.
    /// </summary>
    /// <value>The date time styles used when converting a date to and from JSON.</value>
    public DateTimeStyles DateTimeStyles
    {
      get { return _dateTimeStyles; }
      set { _dateTimeStyles = value; }
    }

    /// <summary>
    /// Gets or sets the date time format used when converting a date to and from JSON.
    /// </summary>
    /// <value>The date time format used when converting a date to and from JSON.</value>
    public string DateTimeFormat
    {
      get { return _dateTimeFormat ?? string.Empty; }
      set { _dateTimeFormat = StringUtils.NullEmptyString(value); }
    }

    /// <summary>
    /// Gets or sets the culture used when converting a date to and from JSON.
    /// </summary>
    /// <value>The culture used when converting a date to and from JSON.</value>
    public CultureInfo Culture
    {
      get { return _culture ?? CultureInfo.CurrentCulture; }
      set { _culture = value; }
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      string text;

      if (value is DateTime)
      {
        DateTime dateTime = (DateTime)value;

        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
          || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          dateTime = dateTime.ToUniversalTime();

        text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
      }
#if !PocketPC && !NET20
      else if (value is DateTimeOffset)
      {
        DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
          || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          dateTimeOffset = dateTimeOffset.ToUniversalTime();

        text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
      }
#endif
      else
      {
        throw new Exception("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)));
      }

      writer.WriteValue(text);
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
    {
      bool nullable = ReflectionUtils.IsNullableType(objectType);
      Type t = (nullable)
        ? Nullable.GetUnderlyingType(objectType)
        : objectType;

      if (reader.TokenType == JsonToken.Null)
      {
        if (!ReflectionUtils.IsNullableType(objectType))
          throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
 
        return null;
      }

      if (reader.TokenType != JsonToken.String)
        throw new Exception("Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

      string dateText = reader.Value.ToString();

      if (string.IsNullOrEmpty(dateText) && nullable)
        return null;

#if !PocketPC && !NET20
      if (t == typeof(DateTimeOffset))
      {
        if (!string.IsNullOrEmpty(_dateTimeFormat))
          return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
        else
          return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
      }
#endif

      if (!string.IsNullOrEmpty(_dateTimeFormat))
        return DateTime.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
      else
        return DateTime.Parse(dateText, Culture, _dateTimeStyles);
    }
  }
}