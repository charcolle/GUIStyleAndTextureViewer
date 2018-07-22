using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using GUIHelper = charcolle.Utility.GUIStyleAndTextureViewer.GUIHelper;

// this code was modified from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip

namespace charcolle.Utility.GUIStyleAndTextureViewer {

    internal class GUIStyleAndTextureTreeView : TreeViewWithTreeModel<GUIStyleAndTextureElement> {

        public bool IsFavoriteMode;
        private const string COPY_GUISTYLE_TEXT = "new GUIStyle( \"{0}\" );";
        private const string COPY_TEXTURE_TEXT = " EditorGUIUtility.Load( \"{0}\" ) as Texture2D;";

        public GUIStyleAndTextureTreeView( TreeViewState state, TreeModel<GUIStyleAndTextureElement> model )
            : base( state, model ) {
            // Custom setup
            showBorder = false;
            customFoldoutYOffset = 3f;
            showAlternatingRowBackgrounds = true;

            Reload();
        }

        protected override float GetCustomRowHeight( int row, TreeViewItem item ) {
            var data = ( TreeViewItem<GUIStyleAndTextureElement> )item;
            if( data != null ) {
                var element = data.data;
                if( element is GUIStyleElement ) {
                    var guiStyleElement = element as GUIStyleElement;
                    var guiStyle = guiStyleElement.GetElement();
                    if( guiStyle != null ) {
                        if( guiStyle.fixedHeight > 0 )
                            return guiStyle.fixedHeight + 40f;
                    }
                } else {
                    var textureElement = element as TextureElement;
                    var texture = textureElement.GetElement();
                    if( texture != null )
                        return texture.height + 40f;
                }
            }

            return 60f;
        }

        protected override IList<TreeViewItem> BuildRows( TreeViewItem root ) {
            var rows = base.BuildRows( root );
            if( IsFavoriteMode )
                rows = rows.Select( r => ( TreeViewItem<GUIStyleAndTextureElement> )r ).Where( i => i.data.IsFavorite ).Select( s => ( TreeViewItem )s ).ToList();
            return rows;
        }

        protected override void ContextClickedItem( int id ) {
            var target = FindItem( id, rootItem );
            if( target != null ) {
                var item = ( TreeViewItem<GUIStyleAndTextureElement> )target;
                item.data.IsContextClicked = true;
            }
        }

        protected override void DoubleClickedItem( int id ) {
            var target = FindItem( id, rootItem );
            if( target != null ) {
                var item = ( TreeViewItem<GUIStyleAndTextureElement> )target;
                var copyText = "";
                if( item.data is GUIStyleElement ) {
                    copyText = string.Format( COPY_GUISTYLE_TEXT, item.data.Name );
                } else {
                    copyText = string.Format( COPY_TEXTURE_TEXT, item.data.Name ); ;
                }
                EditorGUIUtility.systemCopyBuffer = copyText;
                Debug.Log( "This gui content is copied! : " + copyText );
            }
        }

        //=======================================================
        // gui
        //=======================================================

        public override void OnGUI( Rect rect ) {
            base.OnGUI( rect );
        }

        protected override void RowGUI( RowGUIArgs args ) {
            var item = ( TreeViewItem<GUIStyleAndTextureElement> )args.item;
            var contentIndent = GetContentIndent( item );
            if( !item.data.IsValid )
                GUI.backgroundColor = Color.red;

            var bgRect = args.rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max( bgRect.width - contentIndent, 155f );
            bgRect.yMin += 3f;
            bgRect.yMax -= 2f;

            Draw( bgRect, item );
            GUI.backgroundColor = Color.white;
        }

        //=======================================================
        // drawer
        //=======================================================

        private void Draw( Rect rect, TreeViewItem<GUIStyleAndTextureElement> item ) {
            var header = headerRect( rect );

            DrawRect( rect );
            GUI.Label( labelRect( header ), item.data.Name );

            if( GUI.Button( favoriteButtonRect( header ), item.data.IsFavorite ? "★" : "☆", GUI.skin.label ) )
                item.data.IsFavorite = !item.data.IsFavorite;

            DrawItem( item, backgroundRect( rect ) );
        }

        private void DrawRect( Rect rect ) {
            if( Event.current.type == EventType.Repaint ) {
                GUIHelper.Styles.RLHeader.Draw( headerRect( rect ), false, false, false, false );
                GUIHelper.Styles.RLBackGround.Draw( backgroundRect( rect ), false, false, false, false );
            }
        }

        private void DrawItem( TreeViewItem<GUIStyleAndTextureElement> item, Rect backGroundRect ) {
            var rect = backGroundRect;
            var element = item.data;
            if( element is GUIStyleElement ) {
                if( Event.current.type == EventType.Repaint ) {
                    var guiStyleElement = element as GUIStyleElement;
                    var guiStyle = guiStyleElement.GetElement();
                    if( guiStyle != null ) {
                        rect.y += ( rect.height * 0.5f ) - ( guiStyle.fixedHeight > 0 ? guiStyle.fixedHeight * 0.5f : 15f );
                        rect.xMin += 5f;
                        rect.xMax -= 5f;
                        rect.height = guiStyle.fixedHeight > 0 ? guiStyle.fixedHeight : 25f;
                        guiStyle.Draw( rect, false, false, false, false );
                    }
                }
            } else {
                var textureElement = element as TextureElement;
                var texture = textureElement.GetElement();
                if( texture != null ) {
                    rect.y += ( rect.height * 0.5f ) - ( texture.height * 0.5f );
                    rect.x += ( rect.width * 0.5f ) - ( texture.width * 0.5f );
                    rect.height = texture.width > rect.width ? rect.width : texture.height;
                    rect.width = texture.width > rect.width ? rect.width : texture.width;
                    GUI.DrawTexture( rect, texture );
                }
            }
        }

        //=======================================================
        // gui property
        //=======================================================

        private Rect headerRect( Rect bgRect ) {
            var rect = bgRect;
            rect.xMin += 5f;
            rect.xMax -= 5f;
            rect.height = GUIHelper.Styles.RLHeader.fixedHeight;
            return rect;
        }

        private Rect backgroundRect( Rect bgRect ) {
            var rect = headerRect( bgRect );

            rect.y += rect.height;
            rect.height = bgRect.height - rect.height;
            return rect;
        }

        private Rect labelRect( Rect headerRect ) {
            var rect = headerRect;
            rect.y += 1f;
            rect.xMin += 7f;
            rect.xMax -= 17f;
            return rect;
        }

        private Rect favoriteButtonRect( Rect headerRect ) {
            var rect = headerRect;
            rect.y += 1f;
            rect.x = headerRect.xMax - 20f;
            rect.width = 20f;
            return rect;
        }

    }
}