using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularWP : MonoBehaviour {

    public Material[] materials;
    public GameObject cube;
    public Voxel[,,] voxelData;
    public Vector3 voxelPos;
    public bool readCorners = false;
    public float voxelScalar;
    public Vector3 size;

    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private Rigidbody rb;
    private MeshCollider mc;

    private Vector3[] points = new Vector3[] {
        new Vector3(0.5f, 0, 1),
        new Vector3(1, 0, 0.5f),
        new Vector3(0.5f, 0, 0),
        new Vector3(0, 0, 0.5f),

        new Vector3(0.5f, 1, 1),
        new Vector3(1, 1, 0.5f),
        new Vector3(0.5f, 1, 0),
        new Vector3(0, 1, 0.5f),

        new Vector3(0, 0.5f, 1),
        new Vector3(1, 0.5f, 1f),
        new Vector3(1, 0.5f, 0),
        new Vector3(0, 0.5f, 0),
    };

    void Start() {
        
    }
    public void setup() {
        GetComponent<MeshRenderer>().material = materials[0];

        mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void finalize() {
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void initialPhysicsSetup() {
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void updatePhysics() {
        mc.sharedMesh = mesh;
    }

    public void renderMesh() {
        mesh.Clear(false);
        verts.Clear();
        tris.Clear();
        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                for (int y = 0; y < size.y; y++) {

                }
            }
        }
    }

    public void setSize(Vector3 size) {
        this.size = size;
        voxelData = new Voxel[(int)size.x, (int)size.z, (int)size.y];
    }

    int[][] triangulationTable = new int[][] {
        new int[] {}, // none
        new int[] {0, 4, 7}, // 6
        new int[] {6, 2, 5}, // 4
        new int[] {2, 5, 0, 0, 5, 7}, // 4, 6
        new int[] {4, 0, 3}, // 2
        new int[] {4, 0, 3, 0, 4, 7}, // 2, 6
        new int[] {6, 0, 5, 5, 3, 0}, // 2, 4
        new int[] {7, 0, 5, 5, 0, 3}, // 2, 4, 6
        new int[] {2, 6, 1}, // 0
        new int[] {2, 4, 1, 1, 4, 7}, // 0, 6
        new int[] {2, 5, 1, 6, 1, 5}, // 0, 4
        new int[] {2, 5, 1, 7, 1, 5}, // 0, 4, 6
        new int[] {6, 1, 4, 4, 1, 3}, // 0, 2
        new int[] {7, 1, 4, 4, 1, 3}, // 0, 2, 6
        new int[] {1, 3, 6, 6, 3, 5}, // 0, 2, 4
        new int[] {3, 5, 1, 1, 5, 7}, // 0, 2, 4, 6
        new int[] {6, 5, 1}, // 7
        new int[] {4, 5, 0, 0, 5, 1}, // 7, 6
        new int[] {6, 5, 1, 5, 6, 2}, // 7, 4
        new int[] {0, 1, 5, 0, 5, 2}, // 7, 4, 6
        new int[] {6, 5, 1, 0, 3, 4}, // 7, 2
        new int[] {4, 5, 0, 0, 1, 4, 0, 3, 4}, // 7, 2, 6
        new int[] {6, 5, 1, 0, 3, 6, 6, 3, 5}, // 7, 2, 4
        new int[] {0, 3, 5, 5, 1, 0}, // 7, 2, 4, 6
        new int[] {6, 5, 1, 2, 6, 1}, // 7, 0
        new int[] {1, 4, 5, 1, 2, 4}, // 7, 0, 6
        new int[] {6, 5, 1, 1, 2, 6, 6, 2, 5}, // 7, 0, 4
        new int[] {1, 2, 5}, // 7, 0, 4, 6
        new int[] {}, // 7, 0, 2
        new int[] {}, // 7, 0, 2, 6
        new int[] {}, // 7, 0, 2, 4
        new int[] {}, // 7, 0, 2, 4, 6
        new int[] {}, // 5
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
        new int[] {},
    };
}