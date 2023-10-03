using UnityEngine.UIElements;

namespace UnityEnhanced.SerializeInterfaces.Editor
{
	internal class Tab : Toggle
	{
		public Tab(string text) : base()
		{
			base.text = text;
			RemoveFromClassList(ussClassName);
			AddToClassList(ussClassName);
		}
	}
}