using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace charcolle.Utility.GUIStyleAndTextureViewer {

    internal class GUIHelper {

        internal static class Styles {

            static Styles() {
                RLHeader                 = new GUIStyle( "RL Header" );
                RLBackGround             = new GUIStyle( "RL Background" );

                SearchField              = new GUIStyle( "SearchTextField" );
                SearchFieldCancel        = new GUIStyle( "SearchCancelButton" );
                SearchFieldToolBar       = new GUIStyle( "ToolbarSeachTextField" );
                SearchFieldCancelToolBar = new GUIStyle( "ToolbarSeachCancelButton" );
            }

            public static GUIStyle RLHeader {
                get;
                private set;
            }

            public static GUIStyle RLBackGround {
                get;
                private set;
            }

            public static GUIStyle SearchField {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancel {
                get;
                private set;
            }

            public static GUIStyle SearchFieldToolBar {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancelToolBar {
                get;
                private set;
            }

        }

    }

}