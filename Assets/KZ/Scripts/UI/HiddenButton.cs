using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(RawImage))]
public class HiddenButton : MonoBehaviour, IPointerDownHandler {

    [SerializeField] int clicksToActivate = 3;
    [SerializeField] float timeLimit = 2;
    [SerializeField] UnityEvent OnActivate = new UnityEvent();

    float timeWhenReset = -1;
    int _clicks = 0;


    public void OnPointerDown(PointerEventData eventData) {
        float t = Time.realtimeSinceStartup;
        if (t > timeWhenReset) {
            _clicks = 0;
            timeWhenReset = t + timeLimit;
        }
        if (++_clicks >= clicksToActivate) {
            _clicks = 0;
            OnActivate.Invoke();
        }
    }
}