/* =========================================================================

    LanguageManeger.cs
    Copyright(c) R-Koubou

   ======================================================================== */

// Suppress compiler warnings
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RK.I18n
{
    /// <summary>
    /// 言語リソースデータの管理を行う
    /// </summary>
    public class LanguageManeger : MonoBehaviour
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        static LanguageManeger instance;

        /// <summary>
        /// ロードされている言語データ
        /// </summary>
        Language currentLanguage;

        /// <summary>
        /// 最後にロードされた言語
        /// </summary>
        LanguageType.Language lastSelectedLang;

        /// <summary>
        /// インスタンスの取得
        /// </summary>
        /// <value></value>
        static public LanguageManeger Instance
        {
            get
            {
                if( instance == null )
                {
                    const string gameObjectName = "LangageManager";
                    System.Type type = typeof( LanguageManeger );

                    if( ( instance = (LanguageManeger)FindObjectOfType( type ) ) == null )
                    {
                        instance = new GameObject( gameObjectName ).AddComponent<LanguageManeger>();
                        instance.LoadCurrentLanguage( LanguageType.Language.En );
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// このマネージャーが管理する、ユーザーの言語設定を元に言語データをResourcesからロードし、GetCurrentLanguageから参照可能なインスタンス生成を行う
        /// </summary>
        /// <param name="resourcesDir"></param>
        /// <returns></returns>
        public Language LoadCurrentLanguage( string resourcesDir = "" )
        {
            return LoadCurrentLanguage( GetSystemLanguage(), resourcesDir );
        }

        /// <summary>
        /// このマネージャーが管理する、指定された言語データをResourcesからロードし、GetCurrentLanguageから参照可能なインスタンス生成を行う
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="resourcesDir"></param>
        /// <returns></returns>
        public Language LoadCurrentLanguage( LanguageType.Language lang, string resourcesDir = "" )
        {
            string langAsset = string.Format( LanguageType.ASSET_NAME_FORMAT, lang.ToString() );
            if( !string.IsNullOrEmpty( resourcesDir ) )
            {
                langAsset = Path.Combine( resourcesDir, langAsset );
            }

            if( currentLanguage == null )
            {
                currentLanguage = Resources.Load<Language>( langAsset );
            }
            else
            {
                if( lastSelectedLang != lang )
                {
                    currentLanguage = Resources.Load<Language>( langAsset );
                }
            }
            lastSelectedLang = lang;
            return currentLanguage;
        }

        /// <summary>
        /// SetCurrentLanguage 関数により設定されている言語データを取得する
        /// </summary>
        /// <returns></returns>
        public Language GetCurrentLanguage()
        {
            return currentLanguage;
        }

        /// <summary>
        /// 実行中のユーザーの言語設定を元に LanguageType.Language を取得する
        /// </summary>
        /// <returns></returns>
        static public LanguageType.Language GetSystemLanguage()
        {
            switch( Application.systemLanguage )
            {
                case SystemLanguage.Japanese:               return LanguageType.Language.Ja;
                case SystemLanguage.English:                return LanguageType.Language.En;
                case SystemLanguage.French:                 return LanguageType.Language.Fr;
                case SystemLanguage.Italian:                return LanguageType.Language.It;
                case SystemLanguage.German:                 return LanguageType.Language.De;
                case SystemLanguage.Spanish:                return LanguageType.Language.Es;
                case SystemLanguage.Portuguese:             return LanguageType.Language.Pr;
                case SystemLanguage.ChineseSimplified:      return LanguageType.Language.Zh;
                case SystemLanguage.ChineseTraditional:     return LanguageType.Language.Tw;
                case SystemLanguage.Korean:                 return LanguageType.Language.Kr;
                default:
                    return LanguageType.Language.En;
            }
        }
    }
}
