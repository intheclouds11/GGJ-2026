using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace intheclouds
{
	public class DisableOnAwake : MonoBehaviour
	{
		[SerializeField]
		private bool _disableGameObject = true;
		[SerializeField]
		private List<GameObject> _gameObjectsToDisable;
		[SerializeField]
		private bool _disableComponents;
		[SerializeField, ShowIf(nameof(_disableComponents))]
		private List<Component> _componentsToDisable;
	
	
		private void Awake()
		{
			if (_disableGameObject)
			{
				gameObject.SetActive(false);
			}

			foreach (var go in _gameObjectsToDisable)
			{
				go.SetActive(false);
			}

			if (_disableComponents)
			{
				foreach (var component in _componentsToDisable.Where(component => component))
				{
					component.DisableComponent();
				}
			}
		}
	}
}