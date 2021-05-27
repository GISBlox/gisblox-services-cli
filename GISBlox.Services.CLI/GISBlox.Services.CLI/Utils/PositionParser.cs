using GISBlox.Services.SDK.Models;
using System;

namespace GISBlox.Services.CLI.Utils
{
   internal class PositionParser
   {
      /// <summary>
      /// Converts a string into a Coordinate type.
      /// </summary>
      /// <param name="coordinate">A string containing a coordinate pair.</param>
      /// <param name="separator">The character used to separate the points in the coordinate pair.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates are in lat/lon or lon/lat order.</param>
      /// <returns>A Coordinate type.</returns>
      public static Coordinate CoordinateFromString(string coordinate, string separator, CoordinateOrderEnum coordinateOrder)
      {
         if (string.IsNullOrEmpty(coordinate))
         {
            throw new ArgumentNullException(nameof(coordinate));
         }
         string[] pair = GetCoordinatePair(coordinate, separator);
         double a = ParseCoordinatePoint(pair[0]);
         double b = ParseCoordinatePoint(pair[1]);
         return (coordinateOrder == CoordinateOrderEnum.LatLon) ? new Coordinate(a, b) : new Coordinate(b, a);
      }

      /// <summary>
      /// Converts a string into a RDPoint type.
      /// </summary>
      /// <param name="location">A string containing a location pair.</param>
      /// <param name="separator">The character used to separate the points in the location pair.</param>
      /// <returns>An RDPoint type.</returns>
      public static RDPoint RDPointFromString(string location, string separator)
      {
         if (string.IsNullOrEmpty(location))
         {
            throw new ArgumentNullException(nameof(location));
         }
         string[] pair = GetCoordinatePair(location, separator);
         return new RDPoint(ParseRDPoint(pair[0]), ParseRDPoint(pair[1]));
      }

      /// <summary>
      /// Splits the specified position into two strings.
      /// </summary>
      /// <param name="position">A string containint the position to split.</param>
      /// <param name="separator">The character used to separate the position in a location pair.</param>
      /// <returns></returns>
      private static string[] GetCoordinatePair(string position, string separator)
      {
         string[] pair = position.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
         if (pair.Length != 2)
         {
            throw new ArgumentException($"Invalid position: '{ position }'.");
         }
         return pair;
      }

      /// <summary>
      /// Parses the given point into a double.
      /// </summary>
      /// <param name="point">A Ccoordinate point string.</param>
      /// <returns>A double type.</returns>
      private static double ParseCoordinatePoint(string point)
      {
         if (!double.TryParse(point, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out double value))
         {
            throw new ArithmeticException($"Invalid coordinate: point '{ point }' is not of type 'double'.");
         }
         return value;
      }

      /// <summary>
      /// Parses the given point into an int.
      /// </summary>
      /// <param name="point">An RDPoint string.</param>
      /// <returns>An int type.</returns>
      private static int ParseRDPoint(string point)
      {
         if (!int.TryParse(point, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out int value))
         {
            throw new ArithmeticException($"Invalid RDPoint: point '{ point }' is not of type 'int'.");
         }
         return value;         
      }
   }
}
