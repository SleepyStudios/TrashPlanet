using UnityEngine;
using UnityEngine.UIElements;

public class OverlayController : MonoBehaviour
{
    private VisualElement root;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        Invoke("Hide", 0.6f);
    }

    private void OnEnable()
    {
        LevelManager.OnPlayerDeath += Show;
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= Show;
    }

    void Hide()
    {
        root.Q<VisualElement>("overlay").style.opacity = 0f;
    }

    void Show()
    {
        root.Q<VisualElement>("overlay").style.opacity = 1f;
    }
}
