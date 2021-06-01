using GISBlox.Services.SDK.Models;
using System;

namespace GISBlox.Services.CLI.Utils
{
   internal class PositionParser
   {
      /// <summary>
      /// Converts a coordinate string into a Coordinate type.
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
         return CoordinateFromPoints(pair[0], pair[1], coordinateOrder);         
      }

      /// <summary>
      /// Converts coordinate points into a single Coordinate type.
      /// </summary>
      /// <param name="coordinatePointA">A string containing the first coordinate point.</param>
      /// <param name="coordinatePointB">A string containing the second coordinate point.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates are in lat/lon or lon/lat order.</param>
      /// <returns>A Coordinate type.</returns>
      public static Coordinate CoordinateFromPoints(string coordinatePointA, string coordinatePointB, CoordinateOrderEnum coordinateOrder)
      {
         if (string.IsNullOrEmpty(coordinatePointA))
         {
            throw new ArgumentNullException(nameof(coordinatePointA));
         }
         if (string.IsNullOrEmpty(coordinatePointB))
         {
            throw new ArgumentNullException(nameof(coordinatePointB));
         }
         double a = ParseCoordinatePoint(coordinatePointA);
         double b = ParseCoordinatePoint(coordinatePointB);
         return (coordinateOrder == CoordinateOrderEnum.LatLon) ? new Coordinate(a, b) : new Coordinate(b, a);
      }

      /// <summary>
      /// Converts an RD location string into a RDPoint type.
      /// </summary>
      /// <param name="location">A string containing a location pair.</param>
      /// <param name="separator">The character used to separate the points in the location pair.</param>
      /// <param name="rdPointOrder">Specifies whether the location pair is in X/Y or Y/X order.</param>
      /// <returns>An RDPoint type.</returns>
      public static RDPoint RDPointFromString(string location, string separator, RDPointOrderEnum rdPointOrder)
      {
         if (string.IsNullOrEmpty(location))
         {
            throw new ArgumentNullException(nameof(location));
         }
         string[] pair = GetCoordinatePair(location, separator);
         return RDPointFromPoints(pair[0], pair[1], rdPointOrder);
      }

      /// <summary>
      /// Converts RD location points into a single RDPoint type.
      /// </summary>
      /// <param name="rdPointA">A string containing the first location point.</param>
      /// <param name="rdPointB">A string containing the second location point.</param>
      /// <param name="rdPointOrder">Specifies whether the location pair is in X/Y or Y/X order.</param>
      /// <returns>An RDPoint type.</returns>
      public static RDPoint RDPointFromPoints(string rdPointA, string rdPointB, RDPointOrderEnum rdPointOrder)
      {
         if (string.IsNullOrEmpty(rdPointA))
         {
            throw new ArgumentNullException(nameof(rdPointA));
         }
         if (string.IsNullOrEmpty(rdPointB))
         {
            throw new ArgumentNullException(nameof(rdPointB));
         }
         int a = ParseRDPoint(rdPointA);
         int b = ParseRDPoint(rdPointB);
         return (rdPointOrder == RDPointOrderEnum.XY) ? new RDPoint(a, b) : new RDPoint(b, a);
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
