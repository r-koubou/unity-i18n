/* =========================================================================

    SpreadSheetCellExtension.cs
    Copyright(c) R-Koubou

   ======================================================================== */

using DocumentFormat.OpenXml.Spreadsheet;

namespace RK.I18n.Editor
{
    /// <summary>
    ///
    /// </summary>
    public static class SpreadSheetCellExtension
    {
        public static bool IsNullOrEmpty( this Cell cell )
        {
            return string.IsNullOrEmpty( cell.CellValue.Text );
        }
    }
}
