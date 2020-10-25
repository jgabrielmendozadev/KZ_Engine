public class DefaultKZValues : UnityEngine.MonoBehaviour {

    public static KZEditorSettings settings;

    public KZEditorSettings _settings;
    private void Awake() {
        settings = _settings;
    }

}