/* =========================================================================

    SpreadSheetUtil.cs
    Copyright(c) R-Koubou

   ======================================================================== */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace RK.I18n.Editor
{
    /// <summary>
    ///
    /// </summary>
    public static class SpreadSheetUtil
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colmnIndex"></param>
        /// <returns></returns>
        public static Cell GetCell( Worksheet sheet, int rowIndex, int colmnIndex )
        {
            Row row = sheet.GetFirstChild<SheetData>()
                .Elements<Row>()
                .FirstOrDefault( r => r.RowIndex == rowIndex );

            return GetCell( row, colmnIndex );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colmnIndex"></param>
        /// <returns></returns>
        public static Cell GetCell( Row row, int colmnIndex )
        {
            string cellReference = ToColmnIndexToCellReferenceName( row, colmnIndex );

            Cell cell = row?.Elements<Cell>()
                .FirstOrDefault( c => c.CellReference.Value == cellReference );

            return cell;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colmnIndex"></param>
        /// <returns></returns>
        public static string ToColmnIndexToCellReferenceName( Row row, int colmnIndex )
        {
            return ColmnIndexToCellReferenceName( (int)row.RowIndex.Value, colmnIndex );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colmnIndex"></param>
        /// <returns></returns>
        public static string ColmnIndexToCellReferenceName( int rowIndex, int colmnIndex )
        {
            string ret = "";
            const string COL_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int COL_CHARS_LEN = COL_CHARS.Length;
            int v = colmnIndex;

            do
            {
                ret += COL_CHARS[ v % COL_CHARS_LEN ];
                v -= COL_CHARS_LEN;
            }
            while( v >= 0 );

            return ret + rowIndex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="workbookPart"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetStringValue( WorkbookPart workbookPart, Cell cell )
        {
            string value = cell.InnerText;
            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            // Following code from Microsoft documents
            // https://docs.microsoft.com/ja-jp/office/open-xml/how-to-retrieve-the-values-of-cells-in-a-spreadsheet#sample-code

            if( cell.DataType != null )
            {
                switch( cell.DataType.Value )
                {
                    case CellValues.Boolean:
                    case CellValues.Date:
                    case CellValues.Error:
                    case CellValues.InlineString:
                    case CellValues.Number:
                    case CellValues.String:
                        value = cell.InnerText;
                        break;
                    case CellValues.SharedString:
                        if( stringTable != null )
                            value = stringTable.SharedStringTable.ElementAt( int.Parse( value ) ).InnerText;
                        break;
                    default:
                        break;
                }
            }
            return value;
        }
    }
}