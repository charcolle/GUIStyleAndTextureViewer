using System;
using UnityEngine;
using UnityEditor;

namespace charcolle.Utility.GUIStyleAndTextureViewer {

    [Serializable]
    internal class GUIStyleAndTextureElement : TreeElement {

        public string Name;
        public bool IsFavorite;
        public bool IsValid;
        [NonSerialized]
        public bool IsContextClicked;

        public GUIStyleAndTextureElement( string name, int depth, int id ) : base( name, depth, id ) { }

        public virtual void Initialize() { }

    }

    [Serializable]
    internal class GUIStyleAndTextureBase<T> : GUIStyleAndTextureElement {

        [NonSerialized]
        protected T element;

        public GUIStyleAndTextureBase( string name, int depth, int id ) : base( name, depth, id ) { }

        public T GetElement() {
            return element;
        }
    }

    [Serializable]
    internal class GUIStyleElement : GUIStyleAndTextureBase<GUIStyle> {

        public GUIStyleElement( string name, int depth, int id ) : base( name, depth, id ) {
            Name = name;
        }

        public override void Initialize() {
            if( !string.IsNullOrEmpty( Name ) ) {
                element = new GUIStyle( Name );
                IsValid = element != null;
            }
        }

        public static GUIStyleElement Root {
            get {
                return new GUIStyleElement( "", -1, 0 );
            }
        }
    }

    [Serializable]
    internal class TextureElement : GUIStyleAndTextureBase<Texture2D> {

        public TextureElement( string name, int depth, int id ) : base( name, depth, id ) {
            Name = name;
        }

        public override void Initialize() {
            if( !string.IsNullOrEmpty( Name ) ) {
                element = EditorGUIUtility.Load( Name ) as Texture2D;
                IsValid = element != null;
            }
        }

        public static TextureElement Root {
            get {
                return new TextureElement( "", -1, 0 );
            }
        }

    }


}