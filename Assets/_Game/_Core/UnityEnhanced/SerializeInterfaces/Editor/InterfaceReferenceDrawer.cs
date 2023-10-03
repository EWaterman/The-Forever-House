using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEnhanced.SerializeInterfaces.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    [CustomPropertyDrawer(typeof(InterfaceReference<,>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        const string FIELD_NAME = "_underlyingValue";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative(FIELD_NAME);
            InterfaceReferenceUtility.OnGUI(position, prop, label, GetArguments(fieldInfo));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative(FIELD_NAME);
            return InterfaceReferenceUtility.GetPropertyHeight(prop, label, GetArguments(fieldInfo));
        }

        static void GetObjectAndInterfaceType(Type fieldType, out Type objectType, out Type interfaceType)
        {
            if (TryGetTypesFromInterfaceReference(fieldType, out objectType, out interfaceType))
                return;

            TryGetTypesFromList(fieldType, out objectType, out interfaceType);
        }

        static bool TryGetTypesFromInterfaceReference(Type fieldType, out Type objectType, out Type interfaceType)
        {
            var fieldBaseType = fieldType;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(InterfaceReference<>))
                fieldBaseType = fieldType.BaseType;

            if (fieldBaseType.IsGenericType && fieldBaseType.GetGenericTypeDefinition() == typeof(InterfaceReference<,>))
            {
                var types = fieldBaseType.GetGenericArguments();
                interfaceType = types[0];
                objectType = types[1];
                return true;
            }

            objectType = null;
            interfaceType = null;
            return false;
        }

        static bool TryGetTypesFromList(Type fieldType, out Type objectType, out Type interfaceType)
        {
            Type listType = fieldType.GetInterfaces().FirstOrDefault(x =>
              x.IsGenericType &&
              x.GetGenericTypeDefinition() == typeof(IList<>));

            return TryGetTypesFromInterfaceReference(listType.GetGenericArguments()[0], out objectType, out interfaceType);
        }

        static InterfaceObjectArguments GetArguments(FieldInfo fieldInfo)
        {
            GetObjectAndInterfaceType(fieldInfo.FieldType, out var objectType, out var interfaceType);
            return new InterfaceObjectArguments(objectType, interfaceType);
        }
    }
}
