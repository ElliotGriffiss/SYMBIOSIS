
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class InvertedCircleCollider : MonoBehaviour
{
    [SerializeField] private float Radius;

    [Range(2, 100)][SerializeField] private int NumEdges;

    public float GetBoundryRadius()
    {
        return (transform.lossyScale.x * Radius)  - 2; // This will only work if the height and width of the object are the same, 
        //-2 to ensure nothing spawns ontop of a collider
    }

    private void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        Generate();
    }

    private void Generate()
    {
        EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[NumEdges + 1];

        for (int i = 0; i < NumEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / NumEdges;
            float x = Radius * Mathf.Cos(angle);
            float y = Radius * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }
        points[NumEdges] = points[0];

        edgeCollider2D.points = points;
    }
}

