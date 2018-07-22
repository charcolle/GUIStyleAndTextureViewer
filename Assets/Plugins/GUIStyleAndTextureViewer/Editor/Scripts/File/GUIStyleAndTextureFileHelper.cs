using UnityEngine;
using UnityEditor;
using System.IO;

namespace charcolle.Utility.GUIStyleAndTextureViewer {

    internal static class FileHelper {

        private const string SEARCH_ROOT = "GUIStyleAndTextureFileHelper";
        private const string SEARCH_DATA = "GUIStyleAndTextureData";

        private const string RELATIVEPATH_SAVEDATA = "Editor/Data/";
        private const string NAME_DATA = "GUIStyleAndTexture.asset";

        //=============================================================================
        // path
        //=============================================================================

        internal static string RootPath {
            get {
                var guid = getAssetGUID( SEARCH_ROOT );
                if( string.IsNullOrEmpty( guid ) ) {
                    Debug.LogError( "fatal error." );
                    return null;
                }
                var filePath   = Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guid ) );
                var scriptPath = Path.GetDirectoryName( filePath );
                var editorPath = Path.GetDirectoryName( scriptPath );
                var rootPath   = Path.GetDirectoryName( editorPath );

                return pathSlashFix( rootPath );
            }
        }

        internal static string DataPath {
            get {
                return pathSlashFix( Path.Combine( RootPath, RELATIVEPATH_SAVEDATA ) );
            }
        }

        //=============================================================================
        // data load
        //=============================================================================

        internal static GUIStyleAndTextureData LoadGUIStyleAndTextureData() {
            var asset = FindAssetByType<GUIStyleAndTextureData>( SEARCH_DATA );
            if( asset == null )
                asset = CreateGUIStyleAndTextureData();
            return asset;
        }

        internal static GUIStyleAndTextureData CreateGUIStyleAndTextureData() {
            if( !Directory.Exists( DataPath ) )
                Directory.CreateDirectory( DataPath );
            var asset = ScriptableObject.CreateInstance<GUIStyleAndTextureData>();
            asset.GUIStyles.Add( GUIStyleElement.Root );
            asset.Textures.Add( TextureElement.Root );

            var savePath = Path.Combine( DataPath, NAME_DATA );
            AssetDatabase.CreateAsset( asset, savePath );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return asset;
        }

        //=============================================================================
        // utility
        //=============================================================================

        private static T FindAssetByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guid = getAssetGUID( searchFilter );
            if( string.IsNullOrEmpty( guid ) )
                return null;
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            return AssetDatabase.LoadAssetAtPath<T>( assetPath );
        }

        private static string getAssetGUID( string searchFilter ) {
            var guids = AssetDatabase.FindAssets( searchFilter );
            if( guids == null || guids.Length == 0 ) {
                return null;
            }
            return guids[ 0 ];
        }
        internal static string AssetPathToSystemPath( string path ) {
            return pathSlashFix( Path.Combine( dataPathWithoutAssets, path ) );
        }

        private const string forwardSlash = "/";
        private const string backSlash = "\\";
        private static string pathSlashFix( string path ) {
            return path.Replace( backSlash, forwardSlash );
        }

        private static string dataPathWithoutAssets {
            get {
                return pathSlashFix( Application.dataPath.Replace( "Assets", "" ) );
            }
        }
    }
}