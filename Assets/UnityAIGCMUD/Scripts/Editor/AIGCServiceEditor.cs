namespace AillieoUtils.AIGC.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AIGCService), editorForChildClasses: true)]
    public class AIGCServiceEditor : UnityEditor.Editor
    {
        private string validationErrorMessage;

        private bool validationStateUpdated;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (this.validationStateUpdated)
            {
                if (string.IsNullOrEmpty(this.validationErrorMessage))
                {
                    EditorGUILayout.HelpBox("OK", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox(this.validationErrorMessage, MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Unknown state", MessageType.Warning);
            }

            if (GUILayout.Button("Validate service"))
            {
                AIGCService service = this.target as AIGCService;
                if (service != null)
                {
                    if (service.Validate(out this.validationErrorMessage))
                    {
                        this.validationErrorMessage = null;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(this.validationErrorMessage))
                        {
                            this.validationErrorMessage = "Something wrong";
                        }
                    }
                }
            }
        }
    }
}
