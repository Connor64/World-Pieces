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
    public Vector3 position;

    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private Rigidbody rb;
    private MeshCollider mc;

    private Vector3[] points = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),

        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 1),
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

    public void refreshMesh() {
        mesh.Clear(false);
    }

    public void renderMesh() {
        verts.Clear();
        tris.Clear();
        for (int x = 0; x < size.x; x++) {
            float scaledX = x * voxelScalar;
            for (int z = 0; z < size.z; z++) {
                float scaledZ = z * voxelScalar;
                for (int y = 0; y < size.y; y++) {
                    float scaledY = y * voxelScalar;
                    if (voxelData[x, z, y].voxelType != VoxelType.Air && voxelData[x, z, y].cornersExposed != null) {
                        HashSet<int> corners = voxelData[x, z, y].cornersExposed;
                        int triIndex = 0;
                        foreach (int i in corners) {
                            switch (i) {
                                case 1:
                                    triIndex += 128;
                                    break;
                                case 3:
                                    triIndex += 64;
                                    break;
                                case 5:
                                    triIndex += 32;
                                    break;
                                case 7:
                                    triIndex += 16;
                                    break;
                                case 0:
                                    triIndex += 8;
                                    break;
                                case 2:
                                    triIndex += 4;
                                    break;
                                case 4:
                                    triIndex += 2;
                                    break;
                                case 6:
                                    triIndex += 1;
                                    break;
                                default:
                                    Debug.LogError("INVALID CORNER VALUE: " + i);
                                    break;
                            }
                        }
                        if (triIndex > 255 || triIndex < 0) {
                            print(triIndex);
                        }
                        addVertices(new Vector3(scaledX, scaledY, scaledZ), triangulationTable[triIndex], corners);
                    }
                }
            }
        }
    }

    void addVertices(Vector3 position, int[] triStuff, HashSet<int> corners) {
        foreach (int i in triStuff) {
            verts.Add(new Vector3(points[i].x + position.x, points[i].y + position.y, points[i].z + position.z));
            tris.Add(verts.Count - 1);
        }
    }

    public void setSize(Vector3 size, float voxelScalar) {
        this.size = size;
        this.voxelScalar = voxelScalar;
        voxelData = new Voxel[(int)size.x, (int)size.z, (int)size.y];
        // Sets the size of the voxel data array for the entire world piece
        for (int i = 0; i < points.Length; i++) {
            points[i] = Vector3.Scale(points[i], new Vector3(voxelScalar, voxelScalar, voxelScalar));
            // Changes the sizes of the edges based on the voxel scalar
        }
    }

    public void setPosition(Vector3 position) {
        this.position = position;
        transform.position = new Vector3(position.x, 0, position.z);
    }

    int[][] triangulationTable = new int[][] {
        new int[] {}, // none
        new int[] {0, 4, 7}, // 6
        new int[] {6, 2, 5}, // 4
        new int[] {2, 5, 0, 0, 5, 7}, // 4, 6
        new int[] {4, 0, 3}, // 2
        new int[] {4, 0, 3, 0, 4, 7}, // 2, 6
        new int[] {0, 3, 6, 6, 3, 5}, // 2, 4
        //new int[] {7, 0, 5, 5, 0, 3}, // 2, 4, 6
        new int[] {7, 0, 3, 7, 3, 5}, // 2, 4, 6
        new int[] {2, 6, 1}, // 0
        new int[] {2, 4, 1, 1, 4, 7}, // 0, 6
        new int[] {2, 5, 1, 6, 1, 5}, // 0, 4
        //new int[] {2, 5, 1, 7, 1, 5}, // 0, 4, 6
        new int[] {1, 2, 5, 1, 5, 7}, // 0, 4, 6
        new int[] {6, 1, 4, 4, 1, 3}, // 0, 2
        //new int[] {7, 1, 4, 4, 1, 3}, // 0, 2, 6
        new int[] {7, 3, 4, 3, 7, 1}, // 0, 2, 6
        //new int[] {1, 3, 6, 6, 3, 5}, // 0, 2, 4
        new int[] {5, 6, 1, 5, 1, 3}, // 0, 2, 4
        new int[] {3, 5, 1, 1, 5, 7}, // 0, 2, 4, 6
        new int[] {6, 5, 1}, // 7
        new int[] {4, 5, 0, 0, 5, 1}, // 7, 6
        new int[] {6, 5, 1, 5, 6, 2}, // 7, 4
        //new int[] {0, 5, 1, 0, 2, 5}, // 7, 4, 6
        new int[] {2, 5, 1, 2, 1, 0}, // 7, 4, 6
        new int[] {6, 5, 1, 0, 3, 4}, // 7, 2
        new int[] {4, 5, 0, 0, 1, 4, 0, 3, 4}, // 7, 2, 6
        new int[] {6, 5, 1, 0, 3, 6, 6, 3, 5}, // 7, 2, 4
        new int[] {0, 3, 5, 5, 1, 0}, // 7, 2, 4, 6
        new int[] {6, 5, 1, 2, 6, 1}, // 7, 0
        //new int[] {1, 4, 5, 1, 2, 4}, // 7, 0, 6
        new int[] {5, 1, 2, 5, 2, 4}, // 7, 0, 6
        new int[] {6, 5, 1, 1, 2, 6, 6, 2, 5}, // 7, 0, 4
        new int[] {1, 2, 5}, // 7, 0, 4, 6
        new int[] {6, 5, 1, 6, 1, 4, 4, 1, 3}, // 7, 0, 2
        new int[] {5, 1, 4, 4, 1, 3}, // 7, 0, 2, 6
        new int[] {6, 5, 1, 1, 3, 5}, // 7, 0, 2, 4
        new int[] {1, 3, 5}, // 7, 0, 2, 4, 6 -> may be problematic (?)
        new int[] {4, 3, 7}, // 5
        new int[] {0, 4, 7, 4, 3, 7}, // 5, 6
        new int[] {6, 2, 3, 3, 7, 6}, // 5, 4
        //new int[] {0, 3, 7, 0, 2, 3}, // 5, 4, 6
        new int[] {3, 7, 0, 3, 0, 2}, // 5, 4, 6
        new int[] {4, 3, 7, 4, 0, 3}, // 5, 2
        new int[] {0, 4, 7, 4, 3, 7, 4, 0, 3}, // 5, 2, 6
        new int[] {6, 0, 3, 6, 3, 7}, // 5, 2, 4
        new int[] {0, 3, 7}, // 5, 2, 4, 6
        new int[] {6, 1, 2, 4, 3, 7}, // 5, 0
        new int[] {4, 7, 2, 2, 7, 1, 4, 3, 7}, // 5, 0, 6
        new int[] {6, 1, 2, 2, 3, 6, 6, 3, 7}, // 5, 0, 4
        new int[] {2, 7, 1, 2, 3, 7}, // 5, 0, 4, 6
        new int[] {6, 1, 4, 4, 1, 3, 4, 3, 7}, // 5, 0, 2
        new int[] {4, 7, 3, 4, 3, 7}, // 5, 0, 2, 6
        new int[] {6, 1, 3, 3, 6, 7}, // 5, 0, 2, 4
        new int[] {1, 3, 7}, // 5, 0, 2, 4, 6 - > -> may be problematic (?)
        new int[] {4, 3, 6, 6, 3, 1}, // 5, 7
        //new int[] {0, 4, 1, 4, 3, 1}, // 5, 7, 6
        new int[] {0, 4, 3, 0, 3, 1}, // 5, 7, 6
        //new int[] {6, 3, 1, 2, 3, 6}, // 5, 7, 4
        new int[] {1, 6, 2, 1, 2, 3}, // 5, 7, 4
        new int[] {2, 3, 0, 0, 3, 1}, // 5, 7, 4, 6
        new int[] {0, 3, 4, 4, 3, 6, 6, 3, 1}, // 5, 7, 2
        new int[] {0, 3, 4, 4, 3, 1, 1, 4, 0}, // 5, 7, 2, 6
        new int[] {3, 1, 6, 0, 3, 6}, // 5, 7, 2, 4
        new int[] {0, 3, 1}, // 5, 7, 2, 4, 6 -> may be problematic (?)
        new int[] {6, 1, 2, 4, 3, 6, 6, 3, 1}, // 5, 7, 0
        new int[] {4, 3, 1, 1, 2, 4}, // 5, 7, 0, 6
        new int[] {2, 1, 6, 6, 3, 1, 2, 3, 6}, // 5, 7, 0, 4
        new int[] {2, 3, 1}, // 5, 7, 0, 4, 6
        new int[] {6, 1, 4, 4, 1, 3, 4, 3, 6, 6, 3, 1}, // 5, 7, 0, 2
        new int[] {4, 3, 1, 4, 1, 3}, // 5, 7, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 3, 1, 6, 1, 3}, // 5, 7, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 5, 7, 0, 2, 4, 6 (nothing)
        new int[] {2, 1, 5}, // 3
        new int[] {2, 1, 5, 0, 4, 7}, // 3, 6
        new int[] {2, 1, 5, 2, 5, 6}, // 3, 4
        new int[] {2, 1, 5, 2, 5, 0, 0, 5, 7}, // 3, 4, 6
        new int[] {0, 1, 4, 4, 1, 5}, // 3, 2
        new int[] {0, 1, 4, 4, 1, 5, 4, 0, 7}, // 3, 2, 6
        //new int[] {0, 1, 6, 6, 1, 5}, // 3, 2, 4
        new int[] {1, 5, 6, 1, 6, 0}, // 3, 2, 4
        new int[] {0, 1, 5, 0, 5, 7}, // 3, 2, 4, 6
        new int[] {2, 1, 5, 6, 1, 2}, // 3, 0
        new int[] {2, 1, 5, 4, 7, 2, 2, 7, 1}, // 3, 0, 6
        new int[] {2, 1, 5, 6, 1, 2, 2, 5, 6}, // 3, 0, 4
        new int[] {2, 1, 5, 2, 7, 1, 2, 7, 5}, // 3, 0, 4, 6
        //new int[] {6, 1, 4, 4, 1, 5}, // 3, 0, 2
        new int[] {6, 1, 5, 6, 5, 4}, // 3, 0, 2
        new int[] {1, 5, 4, 4, 7, 1}, // 3, 0, 2, 6
        new int[] {6, 1, 5}, // 3, 0, 2, 4
        new int[] {1, 5, 7}, // 3, 0, 2, 4, 6 -> may be problematic (?)
        new int[] {2, 1, 5, 6, 5, 1}, // 3, 7
        new int[] {2, 1, 5, 4, 5, 0, 0, 5, 1}, // 3, 7, 6
        new int[] {2, 1, 5, 2, 5, 6, 6, 5, 1}, // 3, 7, 4
        new int[] {2, 1, 5, 5, 1, 0, 2, 5, 0}, // 3, 7, 4, 6
        new int[] {0, 1, 4, 4, 1, 5, 6, 5, 1}, // 3, 7, 2
        new int[] {0, 1, 4, 4, 1, 5, 4, 5, 0, 0, 5, 1}, // 3, 7, 2, 6
        new int[] {6, 5, 1, 0, 5, 6, 0, 1, 5}, // 3, 7, 2, 4
        new int[] {0, 1, 5, 0, 5, 1}, // 3, 7, 2, 4, 6
        new int[] {2, 1, 5, 2, 6, 1, 6, 5, 1}, // 3, 7, 0
        new int[] {2, 1, 5, 2, 4, 1, 4, 5, 1}, // 3, 7, 0, 6
        new int[] {2, 1, 5, 6, 2, 5, 2, 6, 1, 6, 5, 1}, // 3, 7, 0, 4
        new int[] {2, 1, 5, 2, 5, 1}, // 3, 7, 0, 4, 6 -> may be problematic (?)
        new int[] {6, 1, 4, 4, 1, 5, 6, 5, 1}, // 3, 7, 0, 2
        new int[] {4, 1, 5, 4, 5, 1}, // 3, 7, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 5, 1, 6, 1, 5}, // 3, 7, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 3, 7, 0, 2, 4, 6 (nothing)
        new int[] {2, 1, 4, 4, 1, 7}, // 3, 5
        new int[] {0, 4, 7, 2, 1, 4, 4, 1, 5}, // 3, 5, 6
        //new int[] {6, 2, 7, 2, 1, 7}, // 3, 5, 4
        new int[] {6, 2, 1, 6, 1, 7}, // 3, 5, 4
        new int[] {0, 2, 7, 2, 3, 7}, // 3, 5, 4, 6
        //new int[] {0, 1, 4, 4, 1, 7}, // 3, 5, 2
        new int[] {7, 4, 0, 7, 0, 1}, // 3, 5, 2
        new int[] {0, 1, 4, 6, 4, 1}, // 3, 5, 2, 6
        new int[] {0, 1, 6, 6, 1, 7}, // 3, 5, 2, 4
        new int[] {0, 1, 7}, // 3, 5, 2, 4, 6 -> may be problematic (?)
        new int[] {2, 6, 1, 2, 1, 4, 4, 1, 7}, // 3, 5, 0
        new int[] {2, 1, 4, 4, 1, 7, 4, 7, 2, 2, 7, 1}, // 3, 5, 0, 6
        new int[] {2, 6, 1, 2, 1, 7, 2, 7, 6}, // 3, 5, 0, 4
        new int[] {2, 1, 7, 2, 7, 1}, // 3, 5, 0, 4, 6 -> may be problematic (?)
        new int[] {6, 1, 4, 4, 1, 7}, // 3, 5, 0, 2
        new int[] {4, 1, 7, 4, 7, 1}, // 3, 5, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 1, 7}, // 3, 5, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 3, 5, 0, 2, 4, 6 (nothing)
        //new int[] {2, 1, 4, 4, 1, 6}, // 3, 5, 7
        new int[] {2, 1, 6, 2, 6, 4}, // 3, 5, 7
        //new int[] {2, 1, 5, 6, 5, 1}, // 3, 5, 7
        new int[] {4, 1, 0, 2, 1, 4}, // 3, 5, 7, 6
        new int[] {2, 1, 6}, // 3, 5, 7, 4
        new int[] {0, 2, 1}, // 3, 5, 7, 4, 6 -> may be problematic (?)
        new int[] {0, 1, 4, 4, 1, 6}, // 3, 5, 7, 2
        new int[] {0, 1, 4, 4, 1, 0}, // 3, 5, 7, 2, 6 -> may be problematic (?)
        new int[] {0, 1, 6}, // 3, 5, 7, 2, 4 -> may be problematic (?)
        new int[] {}, // 3, 5, 7, 2, 4, 6 (nothing)
        new int[] {2, 6, 1, 2, 1, 4, 1, 6, 4}, // 3, 5, 7, 0
        new int[] {2, 1, 4}, // 3, 5, 7, 0, 6 -> may be problematic (?)
        new int[] {2, 6, 1, 2, 1, 6}, // 3, 5, 7, 0, 4 -> may be problematic (?)
        new int[] {}, // 3, 5, 7, 0, 4, 6 (nothing)
        new int[] {4, 6, 1, 4, 1, 6}, // 3, 5, 7, 0, 2 -> may be problematic (?)
        new int[] {}, // 3, 5, 7, 0, 2, 6 (nothing)
        new int[] {}, // 3, 5, 7, 0, 2, 4 (nothing)
        new int[] {}, // 3, 5, 7, 0, 2, 4, 6 (nothing)
        new int[] {0, 7, 3}, // 1
        new int[] {0, 7, 3, 0, 4, 7}, // 1, 6
        new int[] {0, 7, 3, 2, 5, 6}, // 1, 4
        new int[] {0, 7, 3, 2, 5, 0, 0, 5, 7}, // 1, 4, 6
        new int[] {0, 7, 3, 0, 3, 4}, // 1, 2
        new int[] {0, 7, 3, 0, 3, 4, 0, 4, 7}, // 1, 2, 6
        new int[] {0, 7, 3, 0, 3, 6, 6, 3, 5}, // 1, 2, 4
        new int[] {0, 7, 3, 0, 3, 5, 5, 7, 0}, // 1, 2, 4, 6
        new int[] {6, 7, 2, 2, 7, 3}, // 1, 0
        //new int[] {2, 7, 3, 2, 4, 7}, // 1, 0, 6
        new int[] {7, 3, 4, 4, 3, 2}, // 1, 0, 6
        new int[] {6, 7, 2, 2, 7, 3, 2, 5, 6}, // 1, 0, 4
        new int[] {2, 5, 7, 2, 7, 3}, // 1, 0, 4, 6
        //new int[] {6, 7, 4, 4, 7, 3}, // 1, 0, 2
        new int[] {7, 3, 4, 6, 7, 4}, // 1, 0, 2
        new int[] {4, 7, 3}, // 1, 0, 2, 6
        new int[] {6, 7, 3, 6, 3, 5}, // 1, 0, 2, 4
        new int[] {3, 5, 7}, // 1, 0, 2, 4, 6 -> may be problematic (?)
        new int[] {6, 5, 0, 0, 5, 3}, // 1, 7
        //new int[] {4, 5, 0, 0, 5, 3}, // 1, 7, 6
        new int[] {3, 0, 4, 3, 4, 5}, // 1, 7, 6
        new int[] {6, 5, 0, 0, 5, 3, 6, 2, 5}, // 1, 7, 4
        new int[] {4, 5, 0, 0, 5, 3}, // 1, 7, 4, 6
        new int[] {6, 5, 0, 0, 5, 3, 0, 3, 4}, // 1, 7, 2
        new int[] {4, 5, 0, 0, 5, 3, 0, 3, 4}, // 1, 7, 2, 6
        new int[] {6, 5, 0, 0, 5, 3, 0, 3, 6, 6, 3, 5}, // 1, 7, 2, 4
        new int[] {0, 3, 5, 0, 5, 3}, // 1, 7, 2, 4, 6 -> may be problematic (?)
        //new int[] {6, 5, 3, 3, 2, 6}, // 1, 7, 0
        new int[] {2, 6, 5, 2, 5, 3}, // 1, 7, 0
        new int[] {4, 5, 2, 2, 5, 3}, // 1, 7, 0, 6
        new int[] {2, 6, 3, 2, 5, 6, 3, 6, 5}, // 1, 7, 0, 4
        new int[] {2, 5, 3}, // 1, 7, 0, 4, 6 -> may be problematic (?)
        new int[] {4, 6, 3, 3, 6, 5}, // 1, 7, 0, 2
        new int[] {4, 5, 3}, // 1, 7, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 5, 3, 3, 5, 6}, // 1, 7, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 1, 7, 0, 2, 4, 6 (nothing)
        new int[] {0, 7, 3, 4, 3, 7}, // 1, 5
        new int[] {0, 7, 3, 0, 4, 7, 4, 3, 7}, // 1, 5, 6
        new int[] {0, 7, 3, 2, 3, 6, 6, 3, 7}, // 1, 5, 4
        new int[] {0, 7, 3, 2, 3, 7, 7, 0, 2}, // 1, 5, 4, 6
        new int[] {0, 7, 3, 0, 3, 4, 4, 3, 7}, // 1, 5, 2
        new int[] {0, 7, 3, 0, 3, 4, 4, 3, 7, 0, 4, 7}, // 1, 5, 2, 6
        new int[] {0, 7, 3, 0, 3, 6, 6, 3, 7}, // 1, 5, 2, 4
        new int[] {0, 7, 3, 0, 3, 7}, // 1, 5, 2, 4, 6 -> may be problematic (?)
        new int[] {6, 7, 2, 2, 7, 3, 4, 3, 7}, // 1, 5, 0
        new int[] {2, 4, 7, 2, 7, 3, 4, 3, 7}, // 1, 5, 0, 6
        new int[] {6, 7, 2, 2, 7, 3, 2, 3, 6, 6, 3, 7}, // 1, 5, 0, 4
        new int[] {2, 7, 3, 2, 3, 7}, // 1, 5, 0, 4, 6 -> may be problematic (?)
        new int[] {4, 3, 7, 6, 7, 3, 4, 6, 3}, // 1, 5, 0, 2
        new int[] {4, 3, 7, 4, 7, 3}, // 1, 5, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 3, 7, 6, 7, 3}, // 1, 5, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 1, 5, 0, 2, 4, 6 (nothing)
        //new int[] {0, 6, 3, 3, 6, 4}, // 1, 5, 7
        new int[] {4, 3, 0, 4, 0, 6}, // 1, 5, 7
        new int[] {4, 3, 0}, // 1, 5, 7, 6
        new int[] {0, 6, 3, 6, 2, 3}, // 1, 5, 7, 4
        new int[] {0, 2, 3}, // 1, 5, 7, 4, 6 -> may be problematic (?)
        new int[] {0, 3, 4, 0, 6, 3, 3, 6, 4}, // 1, 5, 7, 2
        new int[] {0, 3, 4, 0, 4, 3}, // 1, 5, 7, 2, 6 -> may be problematic (?)
        new int[] {0, 6, 3, 0, 3, 6}, // 1, 5, 7, 2, 4 -> may be problematic (?)
        new int[] {}, // 1, 5, 7, 2, 4, 6 (nothing)
        new int[] {6, 3, 2, 4, 3, 6}, // 1, 5, 7, 0
        new int[] {4, 3, 2}, // 1, 5, 7, 0, 6 -> may be problematic (?)
        new int[] {6, 3, 2, 6, 2, 3}, // 1, 5, 7, 0, 4 -> may be problematic (?)
        new int[] {}, // 1, 5, 7, 0, 4, 6 (nothing)
        new int[] {6, 3, 4, 6, 4, 3}, // 1, 5, 7, 0, 2 -> may be problematic (?)
        new int[] {}, // 1, 5, 7, 0, 2, 6 (nothing)
        new int[] {}, // 1, 5, 7, 0, 2, 4 (nothing) 
        new int[] {}, // 1, 5, 7, 0, 2, 4, 6 (nothing)
        new int[] {0, 7, 2, 2, 7, 5}, // 1, 3
        new int[] {0, 4, 7, 0, 7, 2, 2, 7, 5}, // 1, 3, 6
        new int[] {6, 2, 5, 0, 7, 2, 2, 7, 5}, // 1, 3, 4
        new int[] {0, 7, 2, 2, 7, 5, 2, 5, 0, 0, 5, 7}, // 1, 3, 4, 6
        //new int[] {0, 7, 5, 0, 5, 4}, // 1, 3, 2
        new int[] {4, 0, 7, 4, 7, 5}, // 1, 3, 2
        new int[] {0, 4, 7, 0, 7, 5, 0, 5, 4}, // 1, 3, 2, 6
        new int[] {1, 5, 0, 0, 5, 6}, // 1, 3, 2, 4
        new int[] {0, 7, 5, 0, 5, 7}, // 1, 3, 2, 4, 6 -> may be problematic (?)
        //new int[] {2, 6, 7, 2, 7, 5}, // 1, 3, 0
        new int[] {5, 2, 6, 5, 6, 7}, // 1, 3, 0
        new int[] {2, 4, 7, 2, 7, 5}, // 1, 3, 0, 6
        new int[] {2, 4, 7, 2, 7, 5, 2, 5, 6}, // 1, 3, 0, 4
        new int[] {2, 7, 5, 2, 5, 7}, // 1, 3, 0, 4, 6 -> may be problematic (?)
        new int[] {6, 7, 4, 4, 7, 5}, // 1, 3, 0, 2
        new int[] {4, 7, 5}, // 1, 3, 0, 2, 6 -> may be problematic (?)
        new int[] {6, 7, 5}, // 1, 3, 0, 2, 4 -> may be problematic (?)
        new int[] {}, // 1, 3, 0, 2, 4, 6 (nothing)
        //new int[] {2, 0, 5, 0, 6, 5}, // 1, 3, 7
        new int[] {6, 5, 2, 6, 2, 0}, // 1, 3, 7
        new int[] {0, 4, 5, 2, 0, 5}, // 1, 3, 7, 6
        new int[] {2, 5, 6, 2, 0, 5, 0, 6, 5}, // 1, 3, 7, 4
        new int[] {0, 5, 2, 2, 5, 0}, // 1, 3, 7, 4, 6 -> may be problematic (?)
        new int[] {0, 6, 5, 0, 5, 4}, // 1, 3, 7, 2
        new int[] {0, 5, 4, 0, 4, 5}, // 1, 3, 7, 2, 6 -> may be problematic (?)
        new int[] {0, 6, 5, 0, 5, 6}, // 1, 3, 7, 2, 4 -> may be problematic (?)
        new int[] {}, // 1, 3, 7, 2, 4, 6 (nothing)
        new int[] {6, 5, 2}, // 1, 3, 7, 0 ???????
        new int[] {2, 4, 5}, // 1, 3, 7, 0, 6 -> may be problematic (?)
        new int[] {2, 6, 5, 5, 6, 2}, // 1, 3, 7, 0, 4 -> may be problematic (?)
        new int[] {}, // 1, 3, 7, 0, 4, 6 (nothing)
        new int[] {6, 5, 4}, // 1, 3, 7, 0, 2 -> may be problematic (?)
        new int[] {}, // 1, 3, 7, 0, 2, 6 (nothing)
        new int[] {}, // 1, 3, 7, 0, 2, 4 (nothing)
        new int[] {}, // 1, 3, 7, 0, 2, 4, 6 (nothing)
        //new int[] {2, 0, 7, 2, 7, 4}, // 1, 3, 5
        new int[] {0, 7, 4, 0, 4, 2}, // 1, 3, 5
        new int[] {2, 0, 7, 2, 7, 4, 0, 4, 7}, // 1, 3, 5, 6
        new int[] {2, 0, 7, 2, 6, 7}, // 1, 3, 5, 4
        new int[] {2, 0, 7, 2, 7, 0}, // 1, 3, 5, 4, 6
        new int[] {0, 7, 4}, // 1, 3, 5, 2
        new int[] {0, 7, 4, 4, 7, 0}, // 1, 3, 5, 2, 6
        new int[] {0, 7, 6}, // 1, 3, 5, 2, 4
        new int[] {}, // 1, 3, 5, 2, 4, 6 (nothing)
        new int[] {2, 6, 7, 2, 7, 4}, // 1, 3, 5, 0
        new int[] {2, 7, 4, 2, 4, 7}, // 1, 3, 5, 0, 6
        new int[] {2, 6, 7, 2, 7, 6}, // 1, 3, 5, 0, 4
        new int[] {}, // 1, 3, 5, 0, 4, 6 (nothing)
        new int[] {6, 7, 4}, // 1, 3, 5, 0, 2
        new int[] {}, // 1, 3, 5, 0, 2, 6 (nothing)
        new int[] {}, // 1, 3, 5, 0, 2, 4 (nothing)
        new int[] {}, // 1, 3, 5, 0, 2, 4, 6 (nothing)
        new int[] {0, 6, 2, 2, 6, 4}, // 1, 3, 5, 7
        new int[] {0, 4, 2}, // 1, 3, 5, 7, 6
        new int[] {0, 6, 2}, // 1, 3, 5, 7, 4
        new int[] {}, // 1, 3, 5, 7, 4, 6 (nothing)
        new int[] {0, 6, 4}, // 1, 3, 5, 7, 2
        new int[] {}, // 1, 3, 5, 7, 2, 6 (nothing)
        new int[] {}, // 1, 3, 5, 7, 2, 4 (nothing)
        new int[] {}, // 1, 3, 5, 7, 2, 4, 6 (nothing)
        new int[] {6, 4, 2}, // 1, 3, 5, 7, 0
        new int[] {}, // 1, 3, 5, 7, 0, 6 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 4 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 4, 6 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 2 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 2, 6 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 2, 4 (nothing)
        new int[] {}, // 1, 3, 5, 7, 0, 2, 4, 6 (nothing)
    }; 
}