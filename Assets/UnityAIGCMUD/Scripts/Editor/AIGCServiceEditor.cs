namespace AillieoUtils.AIGC.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AIGCService), editorForChildClasses: true)]
    public class AIGCServiceEditor : UnityEditor.Editor
    {
        private string validationErrorMessage;

        private bool validationStateUpdated;

        private bool disableButton;

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

            EditorGUI.BeginDisabledGroup(disableButton);

            if (GUILayout.Button("Validate service"))
            {
                ValidteAsync();
            }

            EditorGUI.EndDisabledGroup();
        }

        private async void ValidteAsync()
        {
            AIGCService service = this.target as AIGCService;

            if (service != null)
            {
                disableButton = true;

                try
                {
                    bool valid = await service.Validate();
                    if (valid)
                    {
                        this.validationErrorMessage = null;
                    }
                    else
                    {
                        this.validationErrorMessage = "service may not work";
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    this.validationErrorMessage = e.Message;
                }

                disableButton = false;
            }
        }
    }
}
