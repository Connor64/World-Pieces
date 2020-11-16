using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public class WorldPiece : MonoBehaviour {
    public Material[] materials;
    public GameObject cube;
    public Block[,,] data;
    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    public Vector3 position;

    public Vector3 size;

    void Start() {
        //buildChunk();
        //new CubeGenerator(new Vector3(0, 0, 0), material);
    }

    void Update() {

    }

    //void buildChunk() {
    //    for (int x = 0; x < 16; x++) {
    //        for (int z = 0; z < 16; z++) {
    //            //cubeGen.generateCube(new Vector3(x, 0, z));
    //            var cubeObject = GameObject.Instantiate(cube).GetComponent<Cube>();
    //            //data[x][0][z] = 
    //            cubeObject.generateCube(new Vector3(x, 0, z), material);
    //            cubeObject.setParentChunk(gameObject);
    //        }
    //    }
    //}

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
        gameObject.AddComponent<MeshCollider>();

        gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void renderMesh() {
        mesh.Clear();
        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                for (int y = 0; y < size.y; y++) {
                    if (data[x, z, y].blockType != BlockType.Air) {
                        if (x != 0) {
                            if (data[x - 1, z, y].blockType == BlockType.Air) {
                                // LEFT
                                verts.Add(new Vector3(x, y, z + 1));
                                verts.Add(new Vector3(x, y + 1, z + 1));
                                verts.Add(new Vector3(x, y, z));
                                verts.Add(new Vector3(x, y + 1, z));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        if (x != size.x - 1) {
                            if (data[x + 1, z, y].blockType == BlockType.Air) {
                                // RIGHT
                                verts.Add(new Vector3(x + 1, y, z));
                                verts.Add(new Vector3(x + 1, y + 1, z));
                                verts.Add(new Vector3(x + 1, y, z + 1));
                                verts.Add(new Vector3(x + 1, y + 1, z + 1));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        if (z != 0) {
                            if (data[x, z - 1, y].blockType == BlockType.Air) {
                                // FRONT
                                verts.Add(new Vector3(x, y, z));
                                verts.Add(new Vector3(x, y + 1, z));
                                verts.Add(new Vector3(x + 1, y, z));
                                verts.Add(new Vector3(x + 1, y + 1, z));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        if (z != size.z - 1) {
                            if (data[x, z + 1, y].blockType == BlockType.Air) {
                                // BEHIND
                                verts.Add(new Vector3(x + 1, y, z + 1));
                                verts.Add(new Vector3(x + 1, y + 1, z + 1));
                                verts.Add(new Vector3(x, y, z + 1));
                                verts.Add(new Vector3(x, y + 1, z + 1));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        if (y != 0) {
                            if (data[x, z, y - 1].blockType == BlockType.Air) {
                                // UNDERNEATH
                                verts.Add(new Vector3(x, y, z + 1));
                                verts.Add(new Vector3(x, y, z));
                                verts.Add(new Vector3(x + 1, y, z + 1));
                                verts.Add(new Vector3(x + 1, y, z));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        if (y != size.y - 1) {
                            if (data[x, z, y + 1].blockType == BlockType.Air) {
                                // ABOVE
                                verts.Add(new Vector3(x, y + 1, z));
                                verts.Add(new Vector3(x, y + 1, z + 1));
                                verts.Add(new Vector3(x + 1, y + 1, z));
                                verts.Add(new Vector3(x + 1, y + 1, z + 1));

                                tris.Add(verts.Count - 4);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 2);
                                tris.Add(verts.Count - 3);
                                tris.Add(verts.Count - 1);
                            }
                        }
                        // remaining rendering is for chunk borders for testing
                        if (x == 0) {
                            verts.Add(new Vector3(x, y, z + 1));
                            verts.Add(new Vector3(x, y + 1, z + 1));
                            verts.Add(new Vector3(x, y, z));
                            verts.Add(new Vector3(x, y + 1, z));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                        if (x == size.x - 1) {
                            verts.Add(new Vector3(x + 1, y, z));
                            verts.Add(new Vector3(x + 1, y + 1, z));
                            verts.Add(new Vector3(x + 1, y, z + 1));
                            verts.Add(new Vector3(x + 1, y + 1, z + 1));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                        if (z == 0) {
                            verts.Add(new Vector3(x, y, z));
                            verts.Add(new Vector3(x, y + 1, z));
                            verts.Add(new Vector3(x + 1, y, z));
                            verts.Add(new Vector3(x + 1, y + 1, z));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                        if (z == size.z - 1) {
                            verts.Add(new Vector3(x + 1, y, z + 1));
                            verts.Add(new Vector3(x + 1, y + 1, z + 1));
                            verts.Add(new Vector3(x, y, z + 1));
                            verts.Add(new Vector3(x, y + 1, z + 1));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                        if (y == 0) {
                            verts.Add(new Vector3(x, y, z + 1));
                            verts.Add(new Vector3(x, y, z));
                            verts.Add(new Vector3(x + 1, y, z + 1));
                            verts.Add(new Vector3(x + 1, y, z));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                        if (y == size.y - 1) {
                            verts.Add(new Vector3(x, y + 1, z));
                            verts.Add(new Vector3(x, y + 1, z + 1));
                            verts.Add(new Vector3(x + 1, y + 1, z));
                            verts.Add(new Vector3(x + 1, y + 1, z + 1));

                            tris.Add(verts.Count - 4);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 2);
                            tris.Add(verts.Count - 3);
                            tris.Add(verts.Count - 1);
                        }
                    }
                }
            }
        }
    }

    float remap(float value, float low1, float high1, float low2, float high2) {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    void setBlockData(int x, int y, int z) {
        
    }

    public void setSize(Vector3 size) {
        this.size = size;
        data = new Block[(int)size.x, (int)size.z, (int)size.y];
    }
}