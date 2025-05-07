using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class GetVertices : MonoBehaviour
{
    public Mesh mesh;
    public GameObject prefab;
    void Start()
    {
        if (mesh == null)
        {
            // Get the mesh filter attached to the GameObject
            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                // Retrieve tangents from the mesh
                Vector4[] tangents = meshFilter.sharedMesh.tangents;

                // Retrieve vertices from the mesh
                Vector3[] vertices = meshFilter.sharedMesh.vertices;

                int index = 0;
                foreach (var vertex in vertices)
                {
                    // Get the world position of the vertex
                    var worldPosition = transform.TransformPoint(vertex);

                    // Get the tangent for this vertex (as Vector3, ignoring the w component)
                    Vector3 tangent = new Vector3(tangents[index].x, tangents[index].y, tangents[index].z);

                    // Normalize the tangent (optional, to control the line length)
                    tangent.Normalize();

                    // Draw a line representing the tangent from the vertex in world space
                    Debug.DrawLine(worldPosition, worldPosition + tangent, Color.green, 100000);

                    // Optionally instantiate a prefab at the vertex position
                    GameObject pre = Instantiate(prefab, worldPosition, Quaternion.identity, transform);

                    // Log the vertex position (just for debugging purposes)
                    Debug.Log("AAA:" + index + " " + pre.transform.localPosition);

                    index++;
                }

                // Now you can do something with the vertices, for example, log them:
                foreach (var vertex in vertices)
                {
                    Debug.Log(vertex);
                }
            }
        }
    }
}