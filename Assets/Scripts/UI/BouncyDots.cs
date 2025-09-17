using UnityEngine;
public class BouncyDots : MonoBehaviour
{
    public RectTransform[] dots; // assign 3 dots
    public float bounceHeight = 10f;
    public float speed = 4f; // oscillation speed
    public float offset = 0.3f; // time delay between dots

    private Vector2[] _basePositions;

    private void Start()
    {
        _basePositions = new Vector2[dots.Length];
        for (var i = 0; i < dots.Length; i++)
        {
            _basePositions[i] = dots[i].anchoredPosition;
        }
    }

    private void Update()
    {
        var time = Time.time * speed;

        for (var i = 0; i < dots.Length; i++)
        {
            var y = Mathf.Sin(time - i * offset) * bounceHeight;
            dots[i].anchoredPosition = _basePositions[i] + new Vector2(0, y);
        }
    }
}