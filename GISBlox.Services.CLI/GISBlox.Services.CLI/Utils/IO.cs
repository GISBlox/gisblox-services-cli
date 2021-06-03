// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using BartelsOnline.Office.IO.Excel;
using BartelsOnline.Office.IO.Excel.Models;
using GISBlox.Services.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Utils
{
   internal class IO
   {
      #region Load 

      /// <summary>
      /// Loads coordinates from a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file. Ignored if the input file is an Excel workbook.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates in the file are stored in lat/lon or lon/lat order.</param>      
      /// <param name="inputRange">The Excel range that contains the coordinates to project (if applicable).</param>
      /// <returns>A List with Coordinate types.</returns>
      public static async Task<List<Coordinate>> LoadCoordinatesFromFile(string fileName, string separator, bool firstLineContainsHeaders, CoordinateOrderEnum coordinateOrder, string inputRange)
      {
         FileFormatEnum format = ParseFileFormat(fileName);
         switch (format)
         {
            case FileFormatEnum.CSV:
               return await LoadCoordinatesFromCSVFile(fileName, separator, firstLineContainsHeaders, coordinateOrder);               
            case FileFormatEnum.XLS:
               return await LoadCoordinatesFromXLSFile(fileName, inputRange, coordinateOrder);
            default:
               return null;               
         }
      }

      /// <summary>
      /// Loads coordinates from a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates in the file are stored in lat/lon or lon/lat order.</param>
      /// <returns>A List with Coordinate types.</returns>
      private static async Task<List<Coordinate>> LoadCoordinatesFromCSVFile(string fileName, string separator, bool firstLineContainsHeaders, CoordinateOrderEnum coordinateOrder)
      {
         if (string.IsNullOrEmpty(separator))
         {
            throw new ArgumentNullException(nameof(separator));
         }
         
         List<Coordinate> coordinates = new();
         using (StreamReader sr = new(fileName))
         {
            if (firstLineContainsHeaders && !sr.EndOfStream)
            {
               await sr.ReadLineAsync();
            }
            while (!sr.EndOfStream)
            {
               string rowData = await sr.ReadLineAsync();
               if (!string.IsNullOrEmpty(rowData))
               {
                  coordinates.Add(PositionParser.CoordinateFromString(rowData, separator, coordinateOrder));
               }
            }
         }
         return coordinates;
      }

      /// <summary>
      /// Loads coordinates from an Excel file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="inputRange">The Excel range that contains the coordinates to project.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates in the file are stored in lat/lon or lon/lat order.</param>
      /// <returns>A List with Coordinate types.</returns>
      private static async Task<List<Coordinate>> LoadCoordinatesFromXLSFile(string fileName, string inputRange, CoordinateOrderEnum coordinateOrder)
      {
         List<Coordinate> coordinates = new();
         using (ExcelReader xlsReader = new(fileName))
         {
            var rows = Task.Run(() => xlsReader.ReadRange(1, inputRange));            
            foreach (var row in await rows)
            {
               coordinates.Add(PositionParser.CoordinateFromPoints(row[0].Value, row[1].Value, coordinateOrder));
            }
         }
         return coordinates;
      }

      /// <summary>
      /// Loads RDNew locations from a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <param name="rdPointOrder">Specifies whether the locations in the file are stored in X-Y or Y-X order.</param>
      /// <param name="inputRange">The Excel range that contains the coordinates to project (if applicable).</param>
      /// <returns>A List with RDPoint types.</returns>
      public static async Task<List<RDPoint>> LoadRDPointsFromFile(string fileName, string separator, bool firstLineContainsHeaders, RDPointOrderEnum rdPointOrder, string inputRange)
      {
         FileFormatEnum format = ParseFileFormat(fileName);
         switch (format)
         {
            case FileFormatEnum.CSV:
               return await LoadRDPointsFromCSVFile(fileName, separator, firstLineContainsHeaders, rdPointOrder);
            case FileFormatEnum.XLS:
               return await LoadRDPointsFromXLSFile(fileName, inputRange, rdPointOrder);               
            default:
               return null;
         }
      }

      /// <summary>
      /// Loads RDNew locations from a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <param name="rdPointOrder">Specifies whether the locations in the file are stored in X-Y or Y-X order.</param>
      /// <returns>A List with RDPoint types.</returns>
      private static async Task<List<RDPoint>> LoadRDPointsFromCSVFile(string fileName, string separator, bool firstLineContainsHeaders, RDPointOrderEnum rdPointOrder)
      {
         if (string.IsNullOrEmpty(separator))
         {
            throw new ArgumentNullException(nameof(separator));
         }

         List<RDPoint> points = new();
         using (StreamReader sr = new(fileName))
         {
            if (firstLineContainsHeaders && !sr.EndOfStream)
            {
               await sr.ReadLineAsync();
            }
            while (!sr.EndOfStream)
            {
               string rowData = await sr.ReadLineAsync();
               if (!string.IsNullOrEmpty(rowData))
               {
                  points.Add(PositionParser.RDPointFromString(rowData, separator, rdPointOrder));
               }
            }
         }
         return points;
      }

      /// <summary>
      /// Loads RDNew locations from an Excel file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="inputRange">The Excel range that contains the locations to project.</param>
      /// <param name="rdPointOrder">Specifies whether the locations in the file are stored in X-Y or Y-X order.</param>
      /// <returns>A List with RDPoint types.</returns>
      private static async Task<List<RDPoint>> LoadRDPointsFromXLSFile(string fileName, string inputRange, RDPointOrderEnum rdPointOrder)
      {
         List<RDPoint> points = new();
         using (ExcelReader xlsReader = new(fileName))
         {
            var rows = Task.Run(() => xlsReader.ReadRange(1, inputRange));
            foreach (var row in await rows)
            {
               points.Add(PositionParser.RDPointFromPoints(row[0].Value, row[1].Value, rdPointOrder));
            } 
         }
         return points;
      }

      #endregion

      #region Save

      /// <summary>
      /// Saves coordinates to a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that separates the coordinates in the file.</param>
      /// <param name="coordinates">A List with Coordinate types.</param>
      /// <param name="coordinateOrder">The order in which to save the coordinate pairs.</param>
      /// <param name="formatProvider">Culture-specific formatting options</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string separator, List<Coordinate> coordinates, CoordinateOrderEnum coordinateOrder, IFormatProvider formatProvider)
      {
         if (string.IsNullOrEmpty(separator)) separator = ";";

         using (StreamWriter sw = new(CorrectOutputFileType(fileName)))
         {
            foreach (var coordinate in coordinates)
            {
               await sw.WriteLineAsync(PositionParser.CoordinateToString(coordinate, separator, coordinateOrder, formatProvider));
            }
         }
      }

      /// <summary>
      /// Saves RDNew locations to a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that separates the locations in the file.</param>
      /// <param name="points">A List with RDPoint types.</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string separator, List<RDPoint> points, RDPointOrderEnum rdPointOrder, IFormatProvider formatProvider)
      {
         if (string.IsNullOrEmpty(separator)) separator = ";";

         using (StreamWriter sw = new(CorrectOutputFileType(fileName)))
         {
            foreach (var point in points)
            {
               await sw.WriteLineAsync(PositionParser.RDPointToString(point, separator, rdPointOrder, formatProvider));
            }
         }
      }

      /// <summary>
      /// Save Location types to a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="separator">The character that separates the coordinates in the file.</param>
      /// <param name="locations">A List with Location types.</param>
      /// <param name="coordinateFirst">True to output the coordinate of the location first, False to output the RDPoint first.</param>
      /// <param name="coordinateOrder">The order in which to save the coordinate pairs.</param>
      /// <param name="formatProvider">Culture-specific formatting options</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string separator, List<Location> locations, bool coordinateFirst, CoordinateOrderEnum coordinateOrder, IFormatProvider formatProvider)
      {
         if (string.IsNullOrEmpty(separator)) separator = ";";

         using (StreamWriter sw = new(CorrectOutputFileType(fileName)))
         {
            foreach (var location in locations)
            {             
               await sw.WriteLineAsync(PositionParser.LocationToString(location, separator, coordinateFirst, coordinateOrder, formatProvider));               
            }
         }
      }

      #endregion

      #region File utils

      protected static FileFormatEnum ParseFileFormat(string fileName)
      {
         FileInfo fi = new (fileName);
         switch (fi.Extension)
         {
            case ".csv":
            case ".txt":
               return FileFormatEnum.CSV;
            case ".xls":
            case ".xlsx":
               return FileFormatEnum.XLS;
            default:
               throw new ArgumentException($"Unsupported file format '{ fi.Extension }'.");
         }               
      }

      protected static string CorrectOutputFileType(string fileName)
      {
         FileFormatEnum format = ParseFileFormat(fileName);
         if (format == FileFormatEnum.XLS)
         {
            // Currently, saving to Excel is not supported, so save as CSV which opens with Excel 
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".csv");
         }
         else
         {
            return fileName;
         }
      }

      #endregion
   }
}
