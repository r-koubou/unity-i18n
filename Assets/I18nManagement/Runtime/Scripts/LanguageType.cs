/* =========================================================================

    LanguageType.cs
    Copyright(c) R-Koubou

   ======================================================================== */

namespace RK.I18n
{
    /**
     * 言語定義
     */
    static public class LanguageType
    {
        /** 言語の文字列表記 */
        static public readonly string[] LANG_LIST = {
            "Ja",
            "En",
            "Fr",
            "It",
            "De",
            "Es",
            "Pr",
            "Zh",
            "Tw",
            "Kr",
        };

        /** LANG_LIST要素数 */
        static public readonly int LANG_NUMS = LANG_LIST.Length;

        /** 単語定義ファイルフォーマット 0:言語名 */
        static public string ASSET_NAME_FORMAT = "lang_{0}";

        /**
         * 言語識別子
         */
        public enum Language
        {
            /** 日本語 */
            Ja = 0,

            /** 英語 */
            En = 1,

            /** フランス語 */
            Fr = 2,

            /** イタリア語 */
            It = 3,

            /** ドイツ語 */
            De = 4,

            /** スペイン語 */
            Es = 5,

            /** ポルトガル語 */
            Pr = 6,

            /** 中国語 */
            Zh = 7,

            /** 中国語 */
            Tw = 8,

            /** 韓国語 */
            Kr = 9,
        }
    }
}
