using Dialog;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Scripts
{
    [CustomPropertyDrawer(typeof(DialogPart))]
    public class DialogMessageArrayPropertyDrawer : PropertyDrawer
    {
        private ReorderableList dialogPartReorderableList;
        private bool foldout = false;

        private const float FoldoutHeight = 18f;
        private const float ReorderableListRowHeight = 48f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var foldoutPosition = position;
            foldoutPosition.height = FoldoutHeight;
            foldout = EditorGUI.Foldout(foldoutPosition, foldout, label);
            if (foldout)
            {
                BuildDialogPartReorderableList(property, label);
                
                dialogPartReorderableList.DoList(position);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            BuildDialogPartReorderableList(property, label);
        
            return !foldout ? FoldoutHeight : dialogPartReorderableList.GetHeight();
        }

        private void BuildDialogPartReorderableList(SerializedProperty dialogPartProperty, GUIContent label)
        {
            if (dialogPartReorderableList != null) return;
            
            SerializedProperty dialogMessagesProperty = dialogPartProperty.FindPropertyRelative(DialogPart.NameOfDialogMessagesField);
            
            dialogPartReorderableList = new ReorderableList(
                dialogMessagesProperty.serializedObject, 
                dialogMessagesProperty, 
                true, 
                true, 
                true, 
                true
            );

            dialogPartReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var dialogMessageProperty = dialogMessagesProperty.GetArrayElementAtIndex(index);
                
                var messagePropertyRect = RenderMessageTextArea(dialogMessageProperty, rect);
                RenderEmotionField(dialogMessageProperty, messagePropertyRect, rect);
            };

            string listLabel = label.text;
            dialogPartReorderableList.drawHeaderCallback += (Rect rect) => { GUI.Label(rect, listLabel); };
            dialogPartReorderableList.elementHeight = ReorderableListRowHeight;
        }

        private static Rect RenderMessageTextArea(SerializedProperty dialogMessageProperty, Rect rect)
        {
            var messageProperty = dialogMessageProperty.FindPropertyRelative(DialogMessage.NameOfMessageField);

            GUIStyle myTextAreaStyle = new GUIStyle(EditorStyles.textArea);

            var messagePropertyRect = rect;
            messagePropertyRect.width *= 0.75f;

            messageProperty.stringValue = EditorGUI.TextArea(
                messagePropertyRect,
                messageProperty.stringValue,
                myTextAreaStyle
            );

            return messagePropertyRect;
        }

        private static void RenderEmotionField(SerializedProperty dialogMessageProperty, Rect messagePropertyRect, Rect rect)
        {
            var emotionProperty = dialogMessageProperty.FindPropertyRelative(DialogMessage.NameOfEmotionField);
            
            var emotionPropertyPosition = messagePropertyRect;
            emotionPropertyPosition.x += messagePropertyRect.width;
            emotionPropertyPosition.width = rect.width * 0.25f;

            EditorGUI.ObjectField(emotionPropertyPosition, emotionProperty, GUIContent.none);
        }
    }
}