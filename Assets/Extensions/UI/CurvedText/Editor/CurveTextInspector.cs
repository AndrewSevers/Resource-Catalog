using UnityEditor;

namespace Extensions.UI {

    [CustomEditor(typeof(CurvedText))]
    public class CurvedTextInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
        }
    }

}
