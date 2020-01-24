/* =========================================================================

    Word.cs
    Copyright(c) R-Koubou

   ======================================================================== */

// Suppress compiler warnings
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RK.I18n
{
    /// <summary>
    /// 1言語分のテキストを表現するデータ構造
    /// </summary>
    [CreateAssetMenu( fileName = "NewLang", menuName = "I18nManagement/Create New Lang", order = 1 )]
    public class Language : ScriptableObject
    {
        /// <summary>
        /// 単語を格納するリスト
        /// </summary>
        public List<string> wordList = new List<string>();

        /// <summary>
        /// 自動生成された WordIndex を用いて該当の単語へのアクセスを行う。
        /// 単語リストが空、または null の場合は空文字を返す。
        /// </summary>
        public string this[ int index ]
        {
            get
            {
                if( wordList == null || wordList.Count == 0 )
                {
                    return "";
                }
                return wordList[ index ];
            }
        }

    }
}
