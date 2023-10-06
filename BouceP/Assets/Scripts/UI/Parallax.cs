using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float speed       = 1.0f;     // Oscillation speed
    [SerializeField] private float amplitude   = 100.0f;   // Amplitude of the oscillation.
    
    private RectTransform  rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Example usage in Unity Update() method
    void Update()
    {
        float distanceToMove = Time.time * speed;

        float newY = rt.offsetMax.y + amplitude * Mathf.Sin(distanceToMove);

        SetParallax(rt, newY, newY);
    }

    public void SetParallax(RectTransform rt, float top, float bot)
    {
        SetTop(rt, rt.offsetMax.y - top);
        SetBottom(rt, rt.offsetMax.y - bot);
    }

    public static void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, top);
    }

    public static void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
