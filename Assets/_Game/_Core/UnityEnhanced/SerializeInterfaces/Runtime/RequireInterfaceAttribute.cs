using System;
using UnityEngine;

namespace UnityEnhanced.SerializeInterfaces
{
    /// <summary>
    /// Use on any serializable field of type UnityEngine.Object or of any type inheriting
	/// UnityEngine.Object, as well as List<> and Arrays that contain said types, or any
	/// other case where you can't simply use a InterfaceReference field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RequireInterfaceAttribute : PropertyAttribute
	{
		public readonly Type InterfaceType;

		public RequireInterfaceAttribute(Type interfaceType)
		{
			Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} needs to be an interface.");
			InterfaceType = interfaceType;
		}
	}
}