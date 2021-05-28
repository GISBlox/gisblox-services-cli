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
         string cellValue = string.Empty;
         Cell theCell = GetCell(sheet, address);

         if (theCell != null && theCell.InnerText.Length > 0)
         {            
            // integer number
            cellValue = theCell.InnerText;

            // not an integet
            if (theCell.DataType != null)
            {
               switch (theCell.DataType.Value)
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

      #endregion
   }
}
