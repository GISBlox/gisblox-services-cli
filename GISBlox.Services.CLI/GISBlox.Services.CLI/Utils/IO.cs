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
      /// <param name="columnSeparator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file. Ignored if the input file is an Excel workbook.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates in the file are stored in lat/lon or lon/lat order.</param>      
      /// <param name="inputRange">The Excel range that contains the coordinates to project (if applicable).</param>
      /// <returns>A List with Coordinate types.</returns>
      public static async Task<List<Coordinate>> LoadCoordinatesFromFile(string fileName, string columnSeparator, bool firstLineContainsHeaders, CoordinateOrderEnum coordinateOrder, string inputRange)
      {
         FileFormatEnum format = ParseFileFormat(fileName);
         switch (format)
         {
            case FileFormatEnum.CSV:
               return await LoadCoordinatesFromCSVFile(fileName, columnSeparator, firstLineContainsHeaders, coordinateOrder);               
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
      /// <param name="columnSeparator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <param name="coordinateOrder">Specifies whether the coordinates in the file are stored in lat/lon or lon/lat order.</param>
      /// <returns>A List with Coordinate types.</returns>
      private static async Task<List<Coordinate>> LoadCoordinatesFromCSVFile(string fileName, string columnSeparator, bool firstLineContainsHeaders, CoordinateOrderEnum coordinateOrder)
      {         
         if (string.IsNullOrEmpty(columnSeparator))         
            throw new ArgumentNullException(nameof(columnSeparator));
         
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
                  coordinates.Add(PositionParser.CoordinateFromString(rowData, columnSeparator, coordinateOrder));
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
      /// <returns></returns>
      private static async Task<List<Coordinate>> LoadCoordinatesFromXLSFile(string fileName, string inputRange, CoordinateOrderEnum coordinateOrder)
      {
         List<Coordinate> coordinates = new();
         using (ExcelReader xlsReader = new(fileName))
         {
            List<List<XlsRange>> rows = xlsReader.ReadRange(1, inputRange);            
            foreach (var row in rows)
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
      /// <param name="columnSeparator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <param name="fileFormat">The input file format.</param>
      /// <returns>A List with RDPoint types.</returns>
      public static async Task<List<RDPoint>> LoadRDPointsFromFile(string fileName, string columnSeparator, bool firstLineContainsHeaders, RDPointOrderEnum rdPointOrder)
      {
         FileFormatEnum format = ParseFileFormat(fileName);
         switch (format)
         {
            case FileFormatEnum.CSV:
               return await LoadRDPointsFromCSVFile(fileName, columnSeparator, firstLineContainsHeaders, rdPointOrder);
            case FileFormatEnum.XLS:
               return await LoadRDPointsFromXLSFile(fileName, firstLineContainsHeaders, rdPointOrder);               
            default:
               return null;
         }
      }

      /// <summary>
      /// Loads RDNew locations from a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="columnSeparator">The character that was used to separate the coordinates in the file.</param>
      /// <param name="firstLineContainsHeaders">Determines whether to skip the first line in the file.</param>
      /// <returns>A List with RDPoint types.</returns>
      private static async Task<List<RDPoint>> LoadRDPointsFromCSVFile(string fileName, string columnSeparator, bool firstLineContainsHeaders, RDPointOrderEnum rdPointOrder)
      {
         if (string.IsNullOrEmpty(columnSeparator))
            throw new ArgumentNullException(nameof(columnSeparator));

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
                  points.Add(PositionParser.RDPointFromString(rowData, columnSeparator, rdPointOrder));
               }
            }
         }
         return points;
      }

      private static async Task<List<RDPoint>> LoadRDPointsFromXLSFile(string fileName, bool firstLineContainsHeaders, RDPointOrderEnum rdPointOrder)
      {
         List<RDPoint> points = new();
         using (ExcelReader xlsReader = new(fileName))
         {
            var range = xlsReader.ReadRange(1, "A1:B4");
            foreach (var row in range)
            {
               XlsRange colA = row[0];
               XlsRange colB = row[1];
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
      /// <param name="columnSeparator">The character that separates the coordinates in the file.</param>
      /// <param name="coordinates">A List with Coordinate types.</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string columnSeparator, List<Coordinate> coordinates)
      {
         if (string.IsNullOrEmpty(columnSeparator)) columnSeparator = ";";

         using (StreamWriter sw = new(fileName))
         {
            foreach (var coordinate in coordinates)
            {
               await sw.WriteLineAsync($"{coordinate.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}{columnSeparator}{coordinate.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
         }
      }

      /// <summary>
      /// Saves RDNew locations to a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="columnSeparator">The character that separates the coordinates in the file.</param>
      /// <param name="points">A List with RDPoint types.</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string columnSeparator, List<RDPoint> points)
      {
         if (string.IsNullOrEmpty(columnSeparator)) columnSeparator = ";";

         using (StreamWriter sw = new(fileName))
         {
            foreach (var point in points)
            {
               await sw.WriteLineAsync($"{point.X}{columnSeparator}{point.Y}");
            }
         }
      }

      /// <summary>
      /// Save Location types to a file.
      /// </summary>
      /// <param name="fileName">The fully-qualified file name.</param>
      /// <param name="columnSeparator">The character that separates the coordinates in the file.</param>
      /// <param name="locations">A List with Location types.</param>
      /// <returns></returns>
      public static async Task SaveToCSVFile(string fileName, string columnSeparator, List<Location> locations)
      {
         if (string.IsNullOrEmpty(columnSeparator)) columnSeparator = ";";

         using (StreamWriter sw = new(fileName))
         {
            foreach (var location in locations)
            {
               await sw.WriteLineAsync($"{location.X}{columnSeparator}{location.Y}{columnSeparator}{location.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}{columnSeparator}{location.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
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

      #endregion
   }
}
