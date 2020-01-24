/* =========================================================================

    Sample.cs

   ======================================================================== */

// Suppress compiler warnings
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;
using UnityEngine.UI;

using RK.I18n;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RK.I18n.Sample
{
    /**
     * 実行中に言語切替が可能なサンプル
     */
    public class Sample : MonoBehaviour
    {
        /** 画面に表示するテキストUI */
        public Text textUI;

        /** 使用する言語 */
        public  LanguageType.Language lang = LanguageType.Language.Ja;

        /** 画面に表示するテキストID */
        //public  WordIndex word = WordIndex.WORD_0001;
        public int word = 0;

        ////////////////////////////////////////////////////////////////////////////
        /**
         * Use this for initialization.
         */
        void Start()
        {
            textUI.text = LanguageManeger.Instance.LoadCurrentLanguage( lang )[ word ];
          //textUI.text = LanguageManeger.Instance.LoadCurrentLanguage()[ word ]; システムの言語設定（Application.systemLanguage）を参照する場合は省略可能
        }

        ////////////////////////////////////////////////////////////////////////////
        /**
         * Update is called once per frame
         */
        void Update()
        {
            textUI.text = LanguageManeger.Instance.LoadCurrentLanguage( lang )[ word ];
        }
    }
}
