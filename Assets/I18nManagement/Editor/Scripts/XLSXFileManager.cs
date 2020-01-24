/* =========================================================================

    XLSXImporter.cs
    Copyright(c) R-Koubou

   ======================================================================== */

// Suppress compiler warnings
#pragma warning disable 0219
#pragma warning disable 0414

using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace RK.I18n.Editor
{
    /// <summary>
    /// 所定フォーマットのXLSXファイルを取り扱う
    /// </summary>
    static public class XLSXFileManager
    {

        /// <summary>
        /// 雛形から新規XLSXファイルを生成する
        /// </summary>
        [MenuItem( "Assets/I18n Management/Create new xlsx file", false, 1 )]
        static void CreateNewFileFromTemplate()
        {
            string src  = "Assets/I18nManagement/Template/Template.xlsx";
            string path = "Assets";
            string dir  = "Assets";

            Object selected = UnityEditor.Selection.activeObject;
            if( selected != null )
            {
                dir = Path.GetDirectoryName( AssetDatabase.GetAssetPath( selected ) );
            }
            path = EditorUtility.SaveFilePanel( "Save as", dir, "NewI18n", "xlsx" );

            if( string.IsNullOrEmpty( path ) )
            {
                Debug.LogWarning( "XLSX NOT SELECTED" );
                return;
            }
            FileUtil.CopyFileOrDirectory( src, path );
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 指定したxlsxファイルをソースにコンバート行う
        /// </summary>
        [MenuItem("Assets/I18n Management/Convert from ...", false, 3)]
        static void ImportSelectedXlsxFile()
        {
            string path = EditorUtility.OpenFilePanel( "Open", "Assets", "xlsx" );
            if( string.IsNullOrEmpty( path ) )
            {
                Debug.LogWarning( "XLSX NOT SELECTED" );
                return;
            }
            ImportFromXlsx( path );
        }

        /// <summary>
        /// 選択した xlsx をソースにコンバート行う
        /// </summary>
        [MenuItem("Assets/I18n Management/Convert from selected XLSX", false, 4)]
        static void ImportActiveSelectedFile()
        {
            Object selected = UnityEditor.Selection.activeObject;
            if( selected != null )
            {
                ImportFromXlsx( AssetDatabase.GetAssetPath( selected ) );
            }
            else
            {
                Debug.LogWarning( "XLSX NOT SELECTED" );
            }
        }

        /// <summary>
        /// 指定されたパスをソースにコンバート行う
        /// </summary>
        /// <param name="xlsxPath"></param>
        /// <param name="outputDir"></param>
        static void ImportFromXlsx( string xlsxPath, string outputDir = null )
        {
            string PATH              = xlsxPath;
            string OUTPUT_FORMAT     = LanguageType.ASSET_NAME_FORMAT + ".asset";
            string OUTPUT_DIR        = outputDir;

            if( string.IsNullOrEmpty( outputDir ) )
            {
                OUTPUT_DIR = EditorUtility.SaveFolderPanel( "Choose Output Folder", "Assets/", "Resources" );
            }

            // OUTPUT_DIR: To relative path
            {
                System.Uri projectUri  = new System.Uri( Path.GetFullPath( "./" ) );
                System.Uri outputUri   = new System.Uri( OUTPUT_DIR );
                OUTPUT_DIR = projectUri.MakeRelativeUri( outputUri ).ToString();
                Debug.Log( OUTPUT_DIR );
            }
            if( string.IsNullOrEmpty( OUTPUT_DIR ) )
            {
                return;
            }

            if( string.IsNullOrEmpty( xlsxPath ) || xlsxPath.EndsWith( ".xlsx" ) == false )
            {
                EditorUtility.DisplayDialog( "Error", Path.GetFileName( xlsxPath ) + " is not xlsx file", "OK" );
                return;
            }

            const int COL_ID_POSITION         = 0;                              // ID値格納位置（列）※Excel上での管理用途。インポートでは使わない。
            const int COL_SYMBOL_POSITION     = 1;                              // シンボル名格納位置（列）
            const int COL_LANG_START_POSITION = 2;                              // 単語格納開始位置（列）
            const int COL_INDEX_NUM           = COL_LANG_START_POSITION - 1;    // 単語データが格納開始されるまでの列数

            const int ROW_START_DATA_POSITION = 1;                              // 単語データ格納開始行

            List<string> enumSymbols = new List<string>( 128 );
            List<string> defaultLang = new List<string>( 128 );

            System.Func<Worksheet, Row,int,Cell> ValidateCell = ( sheet, row, col ) =>
            {
                Cell cell = SpreadSheetUtil.GetCell( row, col );
                if( cell == null || cell.IsNullOrEmpty() )
                {
                    return null;
                }
                return cell;
            };

            using( SpreadsheetDocument xlsx = SpreadsheetDocument.Open( PATH, false, new OpenSettings{ AutoSave = false} ) )
            {
                WorkbookPart workbookpart = xlsx.WorkbookPart;
                Sheet sheet = workbookpart
                     .Workbook
                     .GetFirstChild<Sheets>()
                     .Elements<Sheet>()
                     .FirstOrDefault( s => s.Name == "List" );

                WorksheetPart worksheetPart = (WorksheetPart)workbookpart.GetPartById( sheet.Id.Value );
                Worksheet worksheet = worksheetPart.Worksheet;

                foreach( var r in worksheet.GetFirstChild<SheetData>().Elements<Row>().Select( ( Value, Index ) => new { Value, Index } ) )
                {
                    if( r.Index < ROW_START_DATA_POSITION )
                    {
                        continue;
                    }
                    Cell idCell     = ValidateCell( worksheet, r.Value, COL_ID_POSITION );
                    Cell symbolCell = ValidateCell( worksheet, r.Value, COL_SYMBOL_POSITION );

                    if( idCell == null || symbolCell == null )
                    {
                        continue;
                    }
                    enumSymbols.Add( SpreadSheetUtil.GetStringValue( workbookpart, symbolCell ) );
                }

                for( int langIndex = 0; langIndex < LanguageType.LANG_NUMS; langIndex++ )
                {
                    Language lang = ScriptableObject.CreateInstance<Language>();
                    bool added    = false;

                    foreach( var r in worksheet.GetFirstChild<SheetData>().Elements<Row>().Select( ( Value, Index ) => new { Value, Index } ) )
                    {
                        if( r.Index < ROW_START_DATA_POSITION )
                        {
                            continue;
                        }
                        Cell idCell     = ValidateCell( worksheet, r.Value, COL_ID_POSITION );
                        Cell symbolCell = ValidateCell( worksheet, r.Value, COL_SYMBOL_POSITION );

                        if( idCell == null || symbolCell == null )
                        {
                            continue;
                        }

                        Cell cell  = SpreadSheetUtil.GetCell( r.Value, COL_LANG_START_POSITION + langIndex );
                        string word = "";

                        if( cell != null && !cell.IsNullOrEmpty() )
                        {
                            word = SpreadSheetUtil.GetStringValue( workbookpart, cell );
                        }

                        lang.wordList.Add( word );

                        if( langIndex == (int)LanguageType.Language.Ja )
                        {
                            defaultLang.Add( word );
                        }

                        added = true;
                    }
                    AssetDatabase.CreateAsset( lang, Path.Combine( OUTPUT_DIR, string.Format( OUTPUT_FORMAT, LanguageType.LANG_LIST[ langIndex ] ) ) );
                }
            }

            GenerateIndexEnum( enumSymbols, defaultLang );
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog( "Import", "Done.", "OK" );
        }

        /// <summary>
        /// 添字用列挙定数の生成
        /// </summary>
        /// <param name="symbols"></param>
        static void GenerateIndexEnum( List<string> symbols, List<string>defaultLang )
        {
            string generated = File.ReadAllText( "Assets/I18nManagement/Template/WordIndex.cs.txt" );
            string DEST_PATH = "Assets/I18nManagement/Runtime/Scripts/Generated/WordIndex.cs";

            string FORMAT        = @"        /// <summary>
        /// {0}
        /// </summary>
        {1} = {2},
";

#if false
            // 記号類は _ に置き換える正規表現
            string REGEX_PETTERN = string.Format( "[{0}]+", "\t|,|.|\"|\\s|\\?|#|\\$|%|&|'|\\(|\\)|-|=|\\^|~|\\|\\||\\+|;|\\*|\\:|\\[|\\]|\\{|\\}|<>" );
#endif

            int index = 0;

            using( StringWriter sw = new StringWriter() )
            {
                string date = System.DateTime.Now.ToString( "yyyy/MM/dd hh:mm:ss" );

                foreach( string sym in symbols )
                {
                    string comment = defaultLang[ index ];
                    comment = comment.Replace( "\n", "" );
                    sw.WriteLine( string.Format( FORMAT, comment, sym, index ) );
                    index++;
#if false
                    string symbol = Regex.Replace( sym, REGEX_PETTERN, "_" );
                    sw.WriteLine( string.Format( FORMAT, defaultLang[ index ], symbol, index ) );
                    index++;
#endif
                }
                generated = generated.Replace( "##DATETIME##", date );
                generated = generated.Replace( "##BODY##", sw.ToString() );
            }
            File.WriteAllText( DEST_PATH, generated );
        }
    }
}
