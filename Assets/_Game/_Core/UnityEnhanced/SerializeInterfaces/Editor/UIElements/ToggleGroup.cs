﻿using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEnhanced.SerializeInterfaces.Editor
{
	internal class ToggleGroup
	{
		List<Toggle> _toggles = new();

		public event EventHandler<Toggle> OnToggleChanged;

		public ToggleGroup()
		{
		}

		public void RegisterToggle(Toggle toggle)
		{
			if (toggle == null || _toggles.Contains(toggle)) return;

			_toggles.Add(toggle);
			toggle.RegisterValueChangedCallback(ToggleValueChanged);
		}

		public void UnregisterToggle(Toggle toggle)
		{
			if (!_toggles.Remove(toggle)) return;

			toggle.UnregisterValueChangedCallback(ToggleValueChanged);
		}

		public void Validate()
		{
			if (_toggles.Count == 0) return;

			var activeToggle = GetFirstActiveToggle();
			if (activeToggle == null)
			{
				activeToggle = _toggles[0];
				activeToggle.value = true;
			}
			foreach (var toggle in _toggles)
			{
				if (toggle.value)
					toggle.SetValueWithoutNotify(false);
			}
		}

		public Toggle GetFirstActiveToggle()
		{
			return _toggles.Find(x => x.value == true);
		}

		public bool IsAnyOn()
		{
			return GetFirstActiveToggle() != null;
		}

		void ToggleValueChanged(ChangeEvent<bool> evt)
		{
			HandleToggleChanged(evt.target as Toggle);
		}

		void HandleToggleChanged(Toggle targetToggle)
		{
			ValidateToggleIsInGroup(targetToggle);

			foreach (var toggle in _toggles)
			{
				if (toggle == targetToggle)
					continue;
				toggle.SetValueWithoutNotify(false);
			}

			if (targetToggle.value == true)
				OnToggleChanged?.Invoke(this, targetToggle);
			else
				targetToggle.value = true;
		}

		void ValidateToggleIsInGroup(Toggle toggle)
		{
			if (toggle == null || !_toggles.Contains(toggle))
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
		}
	}
}
