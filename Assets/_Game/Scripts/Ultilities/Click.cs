using UnityEngine.EventSystems;
using UnityEngine;

public class Click : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.GetUI<CanvasSkinShop>().OnChosingItem(transform.GetComponent<ItemShop>());
    }
}
