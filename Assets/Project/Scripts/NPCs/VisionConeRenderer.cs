using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class VisionConeRenderer : MonoBehaviour
{
    public VisionCone logicalCone;
    public int rays = 30;

    private Mesh visualCone;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        visualCone = new Mesh();
        meshFilter.mesh = visualCone;

        Vector3[] vertices = new Vector3[1 + rays];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < vertices.Length; i++)
        {
            Quaternion angle = Quaternion.AngleAxis(-logicalCone.Amplitude / 2 + (i / (float)rays) * logicalCone.Amplitude, Vector3.up);
            vertices[i] = angle * Vector3.forward * logicalCone.Radius;
            Debug.DrawRay(transform.position, angle * transform.forward * logicalCone.Radius, Color.red, 1);
        }
        visualCone.vertices = vertices;

        int[] triangles = new int[3 * (rays - 1)];
        int currentVertex = 1;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
                triangles[i] = 0;
            else if (i % 3 == 1)
            {
                triangles[i] = currentVertex++;
            }
            else
            {
                triangles[i] = currentVertex;
            }
        }
        visualCone.triangles = triangles;
        
    }

    private void Update()
    {
        Vector3[] vertices = visualCone.vertices;
        for (int i = 1; i < vertices.Length; i++)
        {
            Quaternion angle = Quaternion.AngleAxis(-logicalCone.Amplitude / 2 + (i / (float)rays) * logicalCone.Amplitude, Vector3.up);
            Debug.DrawRay(transform.position, angle * transform.forward * logicalCone.Radius, Color.red, 0.1f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, angle * transform.forward * logicalCone.Radius,
                out hit, logicalCone.Radius, LayerMask.GetMask(logicalCone.OcclusionLayer)))
            {
                vertices[i] = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertices[i] = angle * Vector3.forward * logicalCone.Radius;
            }
        }
        visualCone.vertices = vertices;
    }

    private void OnBecameInvisible()
    {
        if (enabled)
            enabled = false;
    }

    private void OnBecameVisible()
    {
        if (!enabled)
            enabled = true;
    }
}
