using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(RawImage))]
public class HiddenButton : MonoBehaviour,IPointerDownHandler {

    [SerializeField] int clicksToActivate;
    [SerializeField] float timeLimit;
    [SerializeField] UnityEngine.Events.UnityEvent OnActivate;

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