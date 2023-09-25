using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(SpinningSlot))]
    public class UIButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SpinningSlot t = (SpinningSlot)target;
        }
    }
}