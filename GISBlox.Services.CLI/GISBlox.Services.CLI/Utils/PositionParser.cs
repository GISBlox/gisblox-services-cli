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
      public static Coordinate CoordinateFromString(string coordinate, string separator, CoordinateOrder coordinateOrder)
      {
         if (string.IsNullOrEmpty(coordinate))
         {
            throw new ArgumentNullException(nameof(coordinate));
         }
         string[] pair = GetCoordinatePair(coordinate, separator);
         double a = ParseCoordinatePoint(pair[0]);
         double b = ParseCoordinatePoint(pair[1]);
         return (coordinateOrder == CoordinateOrder.LatLon) ? new Coordinate(a, b) : new Coordinate(b, a);
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

      private static string[] GetCoordinatePair(string position, string separator)
      {
         string[] pair = position.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
         if (pair.Length != 2)
         {
            throw new ArgumentException($"Invalid position: '{ position }'.");
         }
         return pair;
      }

      private static double ParseCoordinatePoint(string point)
      {
         if (!double.TryParse(point, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out double value))
         {
            throw new ArithmeticException($"Invalid coordinate: point '{ point }' is not of type 'double'.");
         }
         return value;
      }

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
