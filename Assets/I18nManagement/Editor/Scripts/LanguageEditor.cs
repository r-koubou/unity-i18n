/* =========================================================================

    LanguageEditor.cs
    Copyright(c) R-Koubou

   ======================================================================== */

// Suppress compiler warnings
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEditor;
using UnityEngine;

namespace RK.I18n.Editor
{
    /// <summary>
    /// Language インスペクタ拡張
    /// </summary>
    [CustomEditor( typeof( Language ) )]
    [CanEditMultipleObjects]
    public class LanguageEditor : UnityEditor.Editor
    {
        /// <summary>
        /// OnEnable メッセージ
        /// </summary>
        void OnEnable()
        {
        }

        /// <summary>
        /// GUI のカスタム描画
        /// </summary>
        public override void OnInspectorGUI()
        {
            Language self    = target as Language;
            string[] symbols = System.Enum.GetNames( typeof( WordIndex ) );

            if( self.wordList.Count != symbols.Length )
            {
                using( EditorGUILayout.VerticalScope v = new EditorGUILayout.VerticalScope() )
                {
                    EditorGUILayout.LabelField( "Error: WordIndex or Language#wordList is broken" );
                    EditorGUILayout.LabelField( "Number of WordIndex         : " + symbols.Length );
                    EditorGUILayout.LabelField( "Number of Language#wordList : " + self.wordList.Count );
                }
                return;
            }

            using( EditorGUILayout.VerticalScope v = new EditorGUILayout.VerticalScope() )
            {
                for( int index = 0; index < symbols.Length; index++ )
                {
                    using( EditorGUILayout.HorizontalScope h = new EditorGUILayout.HorizontalScope() )
                    {
                        EditorGUILayout.LabelField( symbols[ index ], GUILayout.ExpandWidth( true ) , GUILayout.MaxWidth( 160 ) );
                        self.wordList[ index ] = EditorGUILayout.TextArea( self.wordList[ index ], GUILayout.ExpandWidth( true ) );
                    }
                }
            }
        }
    }
}
