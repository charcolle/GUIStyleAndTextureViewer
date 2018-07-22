using System.Collections.Generic;
using UnityEngine;
using charcolle.Utility.GUIStyleAndTextureViewer;

[CreateAssetMenu()]
internal class GUIStyleAndTextureData : ScriptableObject {

    [SerializeField]
    internal List<GUIStyleElement> GUIStyles = new List<GUIStyleElement>();
    [SerializeField]
    internal List<TextureElement> Textures   = new List<TextureElement>();

    private static GUIStyleAndTextureData instance;

    internal static GUIStyleAndTextureData Instance {
        get {
            if( instance == null )
                instance = FileHelper.LoadGUIStyleAndTextureData();
            return instance;
        }
    }

}