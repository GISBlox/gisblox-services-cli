// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using GISBlox.Services.SDK.Models;
using System;
using System.Globalization;

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
      /// Converts a Coordinate type into a string.
      /// </summary>
      /// <param name="c">A Coordinate type.</param>
      /// <param name="separator">The character to separate the coordinates.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinate is in lat/lon or lon/lat order.</param>
      /// <param name="formatProvider">Culture-specific formatting options.</param>
      /// <returns>A string</returns>
      public static string CoordinateToString(Coordinate c, string separator, CoordinateOrderEnum coordinateOrder, IFormatProvider formatProvider)
      {
         if (c == null)
         {
            throw new ArgumentNullException(nameof(c));
         }
         if (coordinateOrder == CoordinateOrderEnum.LatLon)
         {
            return $"{ c.Lat.ToString(formatProvider) }{ separator }{ c.Lon.ToString(formatProvider) }";
         }
         else
         {
            return $"{ c.Lon.ToString(formatProvider) }{ separator }{ c.Lat.ToString(formatProvider) }";
         }         
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
      /// Converts an RDPoint type into a string. 
      /// </summary>
      /// <param name="rdPoint">An RDPoint type.</param>
      /// <param name="separator">The character to separate the location pairs.</param>
      /// <param name="rdPointOrder">Specifies whether the location points are in X/Y or Y/X order.</param>
      /// <param name="formatProvider">Culture-specific formatting options.</param>
      /// <returns>A string</returns>
      public static string RDPointToString(RDPoint rdPoint, string separator, RDPointOrderEnum rdPointOrder, IFormatProvider formatProvider)
      {
         if (rdPoint == null)
         {
            throw new ArgumentNullException(nameof(rdPoint));
         }
         if (rdPointOrder == RDPointOrderEnum.XY)
         {
            return $"{ rdPoint.X.ToString(formatProvider) }{ separator }{ rdPoint.Y.ToString(formatProvider) }";
         }
         else
         {
            return $"{ rdPoint.Y.ToString(formatProvider) }{ separator }{ rdPoint.X.ToString(formatProvider) }";
         }
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
         if (!double.TryParse(point, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double value))
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
         if (!int.TryParse(point, NumberStyles.Number, CultureInfo.InvariantCulture, out int value))
         {
            throw new ArithmeticException($"Invalid RDPoint: point '{ point }' is not of type 'int'.");
         }
         return value;         
      }

      /// <summary>
      /// Converts a Location type into a string. 
      /// </summary>
      /// <param name="l">A Location type</param>
      /// <param name="separator">The character to separate the locations.</param>
      /// <param name="coordinateFirst">True to output the coordinate of the location first, False to output the RDPoint first.</param>
      /// <param name="coordinateOrder">Specifies whether the location is in lat/lon/X/Y or lon/lat/Y/X order.</param>
      /// <param name="formatProvider">Culture-specific formatting options.</param>
      /// <returns></returns>
      public static string LocationToString(Location l, string separator, bool coordinateFirst, CoordinateOrderEnum coordinateOrder, IFormatProvider formatProvider)
      {
         if (l == null)
         {
            throw new ArgumentNullException(nameof(l));
         }
         if (coordinateOrder == CoordinateOrderEnum.LatLon)
         {
            if (coordinateFirst)
            {
               return $"{ l.Lat.ToString(formatProvider) }{ separator }{ l.Lon.ToString(formatProvider) }{ separator }{ l.X.ToString(formatProvider) }{ separator }{ l.Y.ToString(formatProvider) }";
            }
            else
            {
               return $"{ l.X.ToString(formatProvider) }{ separator }{ l.Y.ToString(formatProvider) }{ separator }{ l.Lat.ToString(formatProvider) }{ separator }{ l.Lon.ToString(formatProvider) }";
            }
         }
         else
         {
            if (coordinateFirst)
            {
               return $"{ l.Lon.ToString(formatProvider) }{ separator }{ l.Lat.ToString(formatProvider) }{ separator }{ l.Y.ToString(formatProvider) }{ separator }{ l.X.ToString(formatProvider) }";
            }
            else
            {
               return $"{ l.Y.ToString(formatProvider) }{ separator }{ l.X.ToString(formatProvider) }{ separator }{ l.Lon.ToString(formatProvider) }{ separator }{ l.Lat.ToString(formatProvider) }";
            }
         }
      }
   }
}
