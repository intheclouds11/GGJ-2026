using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHelper : MonoBehaviour
{
    public void SetAsSelectedGO()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}