using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public class Cube : MonoBehaviour {

    public GameObject parentChunk;
    public Vector3 position;
    public void renderSides(Mesh mesh, float pos, bool north, bool east, bool south, bool west) {
        if (north) {

        }
        if (east) {

        }
        if (south) {
            Vector3[] meshStuff = new Vector3[4] {
                new Vector3(pos - 1.0f, pos - 1.0f, pos - 1.0f), // bottom-left
                new Vector3(pos, pos - 1.0f, pos - 1.0f), // bottom-right
                new Vector3(pos - 1.0f, pos, pos - 1.0f), // top-left
                new Vector3(pos, pos, pos - 1.0f)  // top-right
            };
            mesh.vertices = meshStuff;
            //print("line 50");

            mesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

            GetComponent<MeshFilter>().mesh = mesh;
            transform.position = new Vector3(0, 0, 0);
            mesh.RecalculateNormals();
        }
        if (west) {

        }
    }

    public void generateCube(Vector3 position, Material material) {
        position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
        transform.position = position;
        this.position = position;
        GetComponent<MeshRenderer>().material = material;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.Clear();

        Vector3[] vertices = {
            new Vector3(0, 0, 0), // bottom-left front
            new Vector3(0, 1, 0), // top-left front
            new Vector3(1, 0, 0), // bottom-right front
            new Vector3(1, 1, 0), // top-right front

            new Vector3(0, 0, 1), // bottom-left back
            new Vector3(0, 1, 1), // top-left back
            new Vector3(1, 0, 1), // bottom-right back
            new Vector3(1, 1, 1),  // top-right back
        };
        mesh.vertices = vertices;

        int[] faceTriangles = {
            0, 1, 2, 2, 1, 3, // front
            1, 5, 3, 3, 5, 7, // top
            2, 3, 6, 6, 3, 7, // right
            4, 5, 0, 0, 5, 1, // left
            6, 7, 4, 4, 7, 5, // back
            2, 6, 0, 0, 6, 4, // bottom
        };
        mesh.triangles = faceTriangles;

        mesh.Optimize();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        gameObject.AddComponent<MeshCollider>();

        gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void destoryCube() {
        Destroy(gameObject);
    }

    public void printThing(string bean) {
        print(bean);
    }

    public GameObject getParentChunk() {
        return parentChunk;
    }

    public void setParentChunk(GameObject chunk) {
        parentChunk = chunk;
    }
}