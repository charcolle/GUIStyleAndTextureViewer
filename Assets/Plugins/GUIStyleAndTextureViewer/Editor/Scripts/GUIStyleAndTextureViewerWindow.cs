using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;

using GUIHelper = charcolle.Utility.GUIStyleAndTextureViewer.GUIHelper;

namespace charcolle.Utility.GUIStyleAndTextureViewer {

    internal class GUIStyleAndTextureViewerWindow : EditorWindow, IHasCustomMenu {

        #region window varies
        private GUIStyleAndTextureData Data;
        private IList<GUIStyleAndTextureElement> Elements;
        private GUIStyleAndTextureTreeView treeView;
        private TreeViewState treeViewState;
        private int windowTabIdx  = 0;
        private string searchText = "";
        private string addText    = "";
        private bool isFavorite   = false;

        #endregion

        #region GUI varies
        private const string WINDOW_TITLE           = "GUI Viewer";
        private static readonly Vector2 WINDOW_SIZE = new Vector2( 250, 350 );
        private static readonly string[] TAB_TOOLBAR = new string[] { "GUIStyle", "Texture" };
        #endregion

        public void AddItemsToMenu( GenericMenu menu ) {
            menu.AddItem( new GUIContent( "Reload" ), false, () => {
                Initialize();
            } );
        }

        [MenuItem("Window/GUIStyleAndTextureViewer")]
        private static void Open() {
            var win                   = GetWindow<GUIStyleAndTextureViewerWindow>();
            win.titleContent.text     = WINDOW_TITLE;
            win.minSize               = WINDOW_SIZE;
            win.Show();
        }

        private void OnEnable() {
            // Intialize(); this cause GUIStyleError;
            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        private void Initialize() {
            Data = GUIStyleAndTextureData.Instance;
            TreeViewInitialize( windowTabIdx );
        }

        private void TreeViewInitialize( int idx ) {
            if( Data == null ) {
                treeView = null;
                return;
            }

            Elements = windowTabIdx == 0 ? Data.GUIStyles.ConvertAll<GUIStyleAndTextureElement>( e => e )
                                         : Data.Textures.ConvertAll<GUIStyleAndTextureElement>( e => e );
            if( Elements == null )
                return;

            for( int i = 0; i < Elements.Count; i++ )
                Elements[ i ].Initialize();

            if( treeViewState == null )
                treeViewState = new TreeViewState();

            var treeModel = new TreeModel<GUIStyleAndTextureElement>( windowTabIdx == 0 ? Data.GUIStyles.ConvertAll<GUIStyleAndTextureElement>( e => e )
                                         : Data.Textures.ConvertAll<GUIStyleAndTextureElement>( e => e ) );
            treeView = new GUIStyleAndTextureTreeView( treeViewState, treeModel );
            treeView.treeModel.modelChanged += updateTreeElement;
            treeView.IsFavoriteMode = isFavorite;
            treeView.Reload();
        }

        private void updateTreeElement() {
            Undo.RecordObject( Data, "Move Element" );
            if( windowTabIdx == 0 ) {
                Data.GUIStyles = treeView.treeModel.data.ToList().ConvertAll<GUIStyleElement>( e => (GUIStyleElement)e );
            } else {
                Data.Textures = treeView.treeModel.data.ToList().ConvertAll<TextureElement>( e => ( TextureElement )e );
            }
            EditorUtility.SetDirty( Data );
        }
        
        private void OnGUI() {
            if( Data == null || treeView == null ) {
                Initialize();
                // return; // this cause OnGUI Layout error.
            }
            if( Data == null )
                return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            {
                // window tab
                var idx = GUILayout.Toolbar( windowTabIdx, TAB_TOOLBAR, EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) );
                if( idx != windowTabIdx ) {
                    EditorGUIUtility.keyboardControl = 0;
                    windowTabIdx = idx;
                    TreeViewInitialize( windowTabIdx );
                }

                // search
                EditorGUILayout.BeginHorizontal();
                {
                    searchText = EditorGUILayout.TextField( searchText, GUIHelper.Styles.SearchField );
                    if( GUILayout.Button( "", GUIHelper.Styles.SearchFieldCancel ) ) {
                        Undo.IncrementCurrentGroup();
                        Undo.RecordObject( this, "Delete SearchText" );
                        searchText = "";
                        EditorGUIUtility.keyboardControl = 0;
                    }
                    var fav = GUILayout.Toggle( isFavorite, isFavorite ? "★" : "☆", GUI.skin.button, GUILayout.Width( 20 ) );
                    if( fav != isFavorite ) {
                        isFavorite = fav;
                        treeView.IsFavoriteMode = isFavorite;
                        treeView.Reload();
                    }
                }

                EditorGUILayout.EndHorizontal();

                // treeview
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                treeView.searchString = searchText;
                treeView.OnGUI( GUILayoutUtility.GetLastRect() );

                EditorGUILayout.BeginHorizontal();
                {
                    Undo.IncrementCurrentGroup();
                    Undo.RecordObject( this, "Edit Text" );

                    addText = EditorGUILayout.TextField( addText );
                    GUI.backgroundColor = Color.cyan;
                    if( GUILayout.Button( "Add", GUILayout.Width( 100 ) ) ) {
                        if( !string.IsNullOrEmpty( addText ) ) {
                            AddElementProcess();
                            EditorGUIUtility.keyboardControl = 0;
                        }
                        Initialize();
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 3 );

            }
            EditorGUILayout.EndVertical();

            CheckIsContextClicked();

            if( EditorGUI.EndChangeCheck() ) {
                EditorUtility.SetDirty( Data );
            }
        }

        private void CheckIsContextClicked() {
            for( int i = 0; i < Elements.Count; i++ ) {
                if( Elements[ i ].IsContextClicked ) {
                    var menu = new GenericMenu();
                    menu.AddItem( new GUIContent( "Delete" ), false, () => {
                        Undo.RecordObject( Data, "Delete" );
                        if( windowTabIdx == 0 ) {
                            Data.GUIStyles.RemoveAt( i );
                        } else {
                            Data.Textures.RemoveAt( i );
                        }
                        TreeViewInitialize( windowTabIdx );
                    } );
                    menu.ShowAsContext();
                    Event.current.Use();
                    Elements[ i ].IsContextClicked = false;
                    break;
                }
            }
        }

        private void AddElementProcess() {
            Undo.RecordObject( Data, "Add" );
            if( windowTabIdx == 0 ) {
                var num = Data.GUIStyles.Count + 1;
                var element = new GUIStyleElement( addText, 0, num );
                Data.GUIStyles.Add( element );
            } else {
                var num = Data.Textures.Count + 1;
                var element = new TextureElement( addText, 0, num );
                Data.Textures.Add( element );
            }
            addText = "";
        }

        //private void DrawContent( List<GUIStyleAndTextureElement> GUIs, bool IsGUIStyle ) {
        //    if( GUIs == null )
        //        return;

        //    var isSearch = !string.IsNullOrEmpty( searchText );
        //    for( int i = 0; i < GUIs.Count; i++ ) {
        //        var element = GUIs[ i ];
        //        if( isSearch && !element.Name.Contains( searchText ) )
        //            continue;

        //        using( new GUIDrawerClass( element, IsGUIStyle ) ) {
        //            if( IsGUIStyle ) {
        //                var style = new GUIStyle( element.Name );
        //                GUILayout.Label( "", style, new GUILayoutOption[] { GUILayout.ExpandWidth( true ) } );
        //            } else {
        //                var texture = EditorGUIUtility.Load( element.Name ) as Texture2D;
        //                GUILayout.Label( new GUIContent( texture ), new GUILayoutOption[] { GUILayout.ExpandWidth( true ) } );
        //            }
        //        }

        //        if( Event.current.type == EventType.ContextClick ) {
        //            if( GUILayoutUtility.GetLastRect().Contains( Event.current.mousePosition ) ) {
        //                var menu = new GenericMenu();
        //                menu.AddItem( new GUIContent( "Delete" ), false, () => {
        //                    Undo.RecordObject( Data, "Delete" );
        //                    GUIs.RemoveAt( i );
        //                } );
        //                menu.ShowAsContext();
        //                Event.current.Use();
        //                break;
        //            }
        //        }

        //    }

        //}


    }

}
