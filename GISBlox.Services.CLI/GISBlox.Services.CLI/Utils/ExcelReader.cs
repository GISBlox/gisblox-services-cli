using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GISBlox.Services.CLI.Utils
{
   internal class ExcelReader : IDisposable
   {
      private readonly SpreadsheetDocument _document;
      private readonly WorkbookPart _workbookPart;

      private bool disposedValue;

      #region Constructor / Destructor

      public ExcelReader(string fileName)
      {
         _document = SpreadsheetDocument.Open(fileName, false);
         _workbookPart = _document.WorkbookPart;
      }

      ~ExcelReader()
      {         
         Dispose(disposing: false);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               if (_document != null)
               {                  
                  _document.Close();
                  _document.Dispose();
               }
            }
            disposedValue = true;
         }
      }

      public void Dispose()
      {
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }

      #endregion

      /// <summary>
      /// Returns a cell value from a sheet.
      /// </summary>
      /// <param name="sheetName">The sheet name.</param>
      /// <param name="cellAddress">The cell address.</param>
      /// <returns>The cell value as a string type.</returns>
      public string ReadCellValue(string sheetName, string cellAddress)
      {
         string cellValue = string.Empty;
         Sheet theSheet = GetSheet(sheetName);
         if (theSheet != null)
         {
            cellValue = GetCellValue(theSheet, cellAddress);
         }
         return cellValue;
      }

      /// <summary>
      /// Returns a cell value from a sheet.
      /// </summary>
      /// <param name="sheetNumber">The sheet number.</param>
      /// <param name="cellAddress">The cell address.</param>
      /// <returns>The cell value as a string type.</returns>
      public string ReadCellValue(int sheetNumber, string cellAddress)
      {
         string cellValue = string.Empty;
         Sheet theSheet = GetSheet(sheetNumber);
         if (theSheet != null)
         {
            cellValue = GetCellValue(theSheet, cellAddress);
         }
         return cellValue;
      }

      public void ReadRangeValues(string sheetName, string rangeAddress)
      {
         Sheet theSheet = GetSheet(sheetName);
         if (theSheet != null)
         {
            int sepPos = rangeAddress.IndexOf(":");
            if (sepPos > 0)
            {
               CellAddress upperLeft = GetCellAddress(rangeAddress.Substring(0, sepPos));
               CellAddress bottomRight = GetCellAddress(rangeAddress.Substring(sepPos + 1));
               for (int row = upperLeft.RowIndex; row <= bottomRight.RowIndex; row++)
               {
                  for (int col = upperLeft.ColumnIndex; col <= bottomRight.ColumnIndex; col++)
                  {
                     Cell rangeCell = GetCell(theSheet, row, col);
                     if (rangeCell != null)
                     {
                        // range reader: init with range, then each read goes through cells in first row, then second row, etc. Take<cell>
                        string rangeCellValue = $"{ rangeCell.CellReference.Value }: { GetCellValue(rangeCell)}";
                     }
                  }
               }
            }
            else
            {
               throw new ArgumentException($"Invalid range address.");
            }
         }

      }

      #region Private methods

      /// <summary>
      /// Get a sheet reference.
      /// </summary>
      /// <param name="id">The sheet number (first sheet is 1).</param>
      /// <returns>A Sheet type.</returns>
      private Sheet GetSheet(int id)
      {
         return _workbookPart.Workbook.Descendants<Sheet>().ElementAt(id - 1);
      }

      /// <summary>
      /// Get a sheet reference.
      /// </summary>
      /// <param name="name">The sheet name.</param>
      /// <returns>A Sheet type.</returns>
      private Sheet GetSheet(string name)
      {
         return _workbookPart.Workbook.Descendants<Sheet>()
                .Where(s => s.Name == name).FirstOrDefault();
      }

      /// <summary>
      /// Get a row reference.
      /// </summary>
      /// <param name="sheet">A sheet type.</param>
      /// <param name="rowIndex">The row index.</param>
      /// <returns>A Row type.</returns>
      private Row GetRow(Sheet sheet, int rowIndex)
      {
         return sheet.GetFirstChild<SheetData>().Elements<Row>()
                .Where(r => r.RowIndex == rowIndex).FirstOrDefault();
      }

      /// <summary>
      /// Gets a cell reference.
      /// </summary>
      /// <param name="sheet">A sheet type.</param>
      /// <param name="address">The cell address.</param>
      /// <returns>A Cell type.</returns>
      private Cell GetCell(Sheet sheet, string address)
      {
         WorksheetPart wsPart = (WorksheetPart)(_workbookPart.GetPartById(sheet.Id));
         return wsPart.Worksheet.Descendants<Cell>()
                .Where(c => c.CellReference == address).FirstOrDefault();
      }

      /// <summary>
      /// Gets a cell reference.
      /// </summary>
      /// <param name="sheet">A sheet type.</param>
      /// <param name="rowIndex">The row index (first row is 1).</param>
      /// <param name="columnIndex">The column index (first column is 1).</param>
      /// <returns>A Cell type.</returns>
      private Cell GetCell(Sheet sheet, int rowIndex, int columnIndex)
      {
         WorksheetPart wsPart = (WorksheetPart)(_workbookPart.GetPartById(sheet.Id));
         return wsPart.Worksheet.Descendants<Cell>()
                .Where(c => c.CellReference == GetColumnName(columnIndex) + rowIndex).FirstOrDefault();
      }

      /// <summary>
      /// Returns a cell value.
      /// </summary>
      /// <param name="sheet">A sheet.</param>
      /// <param name="address">The cell address.</param>
      /// <returns>The cell value as a string type.</returns>
      private string GetCellValue(Sheet sheet, string address)
      {         
         Cell cell = GetCell(sheet, address);
         return GetCellValue(cell);         
      }

      /// <summary>
      /// Returns a cell value.
      /// </summary>      
      /// <param name="cell">A cell.</param>
      /// <returns>The cell value as a string type.</returns>
      private string GetCellValue(Cell cell)
      {
         string cellValue = string.Empty;
         if (cell != null && cell.InnerText.Length > 0)
         {
            // integer number
            cellValue = cell.InnerText;

            // not an integet
            if (cell.DataType != null)
            {
               switch (cell.DataType.Value)
               {
                  case CellValues.Boolean:
                     switch (cellValue)
                     {
                        case "0":
                           cellValue = "FALSE";
                           break;
                        default:
                           cellValue = "TRUE";
                           break;
                     }
                     break;
                  case CellValues.SharedString:
                     var stringTable = _workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                     if (stringTable != null)
                     {
                        cellValue = stringTable.SharedStringTable.ElementAt(int.Parse(cellValue)).InnerText;
                     }
                     break;
               }
            }
         }
         return cellValue;
      }

      /// <summary>
      /// Returns the column name.
      /// </summary>
      /// <param name="columnIndex">The column index.</param>
      /// <returns>The column name.</returns>
      private static string GetColumnName(int columnIndex)
      {
         int modifier;
         int dividend = columnIndex;
         string columnName = string.Empty;
         
         while (dividend > 0)
         {
            modifier = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
            dividend = (dividend - modifier) / 26;
         }

         return columnName;
      }

      /// <summary>
      /// Returns the column index.
      /// </summary>
      /// <param name="columnName">The column name.</param>
      /// <returns>The column index.</returns>
      private static int GetColumnIndex(string columnName)
      {
         int sum = 0;
         columnName = columnName.ToUpperInvariant();

         for (int i = 0; i < columnName.Length; i++)
         {
            sum *= 26;
            sum += (columnName[i] - 'A' + 1);
         }

         return sum;
      }

      /// <summary>
      /// Converts a cell address like 'B12' into a CellAddress type.
      /// </summary>
      /// <param name="address">The cell address.</param>
      /// <returns>A CellAdress type.</returns>
      private static CellAddress GetCellAddress(string address)
      {
         int startIndex = address.IndexOfAny("0123456789".ToCharArray());
         return new CellAddress()
         {
            Column = address.Substring(0, startIndex),
            ColumnIndex = GetColumnIndex(address.Substring(0, startIndex)),
            RowIndex = int.Parse(address.Substring(startIndex))
         };
      }

      #endregion
   }

   internal struct CellAddress
   {
      public string Column { get; set; }

      public int ColumnIndex { get; set; }

      public int RowIndex { get; set; }
   }
}
