/*
 * This is a way for a script somewhere else to know whether the player is actually on the right menu or not
 */
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusLayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool inFocus = false;

    public void OnPointerEnter(PointerEventData eventData) {
        inFocus = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        inFocus = false;
    }
}
