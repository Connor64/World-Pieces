using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour {
    private Dictionary<WorldPiece, GameObject> worldPieces = new Dictionary<WorldPiece, GameObject>();
    private Dictionary<SmoothWP, GameObject> smoothWPs = new Dictionary<SmoothWP, GameObject>();
    public GameObject wpPref;
    public GameObject smoothWPPref;
    public Vector3 worldPieceSize;
    public Vector2 wpAmount;
    public float perlin2DMultiplier = 30f;
    public float perlin3DMultiplier = 0.05f;
    public float perlin3DThreshold = 0.38f;
    public float perlin2DScalar = 0.025f;
    public float perlin3DScalar = 0.079f;
    public float heightBuffer = 64f;
    public float voxelScalar = 1f;

    public static TerrainGenerator instance;

    public bool regenerate = false;
    // used during runtime to regenerate the chunk for testing. Automatically reset once enabled

    public HashSet<int> testCorners;

    void Start() {
        if (instance == null) {
            instance = gameObject.GetComponent<TerrainGenerator>();
        } else {
            Destroy(gameObject);
        }
        if (worldPieceSize == Vector3.one) {
            GameObject swpObject = Instantiate(smoothWPPref);
            //swpObject.transform.position = position;
            swpObject.layer = 8;
            SmoothWP swp = swpObject.GetComponent<SmoothWP>();
            swp.setSize(worldPieceSize, voxelScalar);

            swp.data[0, 0, 0] = new MarchingBlock(LandType.Dirt, Vector3.zero, testCorners);
            swp.setup();
            swp.renderMesh();
            swp.finalize();
            swp.initialPhysicsSetup();
            smoothWPs.Add(swp, swpObject);
        } else {
            for (int x = 0; x < wpAmount.x; x++) {
                for (int z = 0; z < wpAmount.y; z++) {
                    //buildVoxelWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
                    buildSmoothWP(new Vector3(x * worldPieceSize.x * voxelScalar, 0, z * worldPieceSize.z * voxelScalar));
                }
            }
        }
    }

    void Update() {
        if (regenerate) {
            foreach (KeyValuePair<SmoothWP, GameObject> swp in smoothWPs) {
                Destroy(swp.Value);
            }
            smoothWPs.Clear();
            //Destroy(thingy);
            for (int x = 0; x < wpAmount.x; x++) {
                for (int z = 0; z < wpAmount.y; z++) {

                    //buildVoxelWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
                    buildSmoothWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
                }
            }
            regenerate = false;
        }
    }

    void buildVoxelWP(Vector3 position) {
        GameObject wpObject = Instantiate(wpPref);
        wpObject.transform.position = position;
        wpObject.layer = 8;
        WorldPiece wp = wpObject.GetComponent<WorldPiece>();
        wp.position = wpObject.transform.position;
        wp.setSize(worldPieceSize);
        for (int x = 0; x < worldPieceSize.x; x++) {
            for (int z = 0; z < worldPieceSize.z; z++) {
                for (int y = 0; y < worldPieceSize.y; y++) {
                    // uses heightmap from 2D perlin noise to create hills and stuff
                    if (y > PerlinNoiseGen.perlin2D(x + position.x, z + position.z, perlin2DScalar, false) * PerlinNoiseGen.perlin2D(x + position.x, z + position.z, perlin2DScalar, false) * perlin2DMultiplier + heightBuffer) {
                        wp.data[x, z, y] = new Block(BlockType.Dirt, new Vector3(x + position.x, y, z + position.z));
                    } else {
                        wp.data[x, z, y] = new Block(BlockType.Air, new Vector3(x + position.x, y, z + position.z));
                    }

                    //// cut out caves
                    //if (PerlinNoiseGen.perlin3D(x + position.x, y, z + position.z, perlin3DScalar) > perlin3DThreshold) {
                    //    wp.data[x, z, y] = new Block(BlockType.Air, new Vector3(x + position.x, y, z + position.z));
                    //}
                }
            }
        }
        wp.setup();
        wp.renderMesh();
        wp.finalize();
        wp.initialPhysicsSetup();
        worldPieces.Add(wp, wpObject);
    }

    void buildSmoothWP(Vector3 position) {
        GameObject swpObject = Instantiate(smoothWPPref);
        swpObject.transform.position = position;
        swpObject.layer = 8;
        SmoothWP swp = swpObject.GetComponent<SmoothWP>();
        swp.setSize(worldPieceSize, voxelScalar);
        for (int x = 0; x < worldPieceSize.x; x++) {
            float scaledX = x * voxelScalar;
            for (int z = 0; z < worldPieceSize.z; z++) {
                float scaledZ = z * voxelScalar;
                for (int y = 0; y < worldPieceSize.y; y++) {
                    float scaledY = y * voxelScalar;
                    HashSet<int> corners = new HashSet<int>();

                    //float[] perlin3DStuff = PerlinNoiseGen.marchingPerlin3D(scaledX + position.x, scaledY, scaledZ + position.z, perlin3DScalar, perlin3DMultiplier);
                    float[] perlin3DStuff = PerlinNoiseGen.marchingPerlin3D(x + position.x, y, z + position.z, perlin3DScalar, perlin3DMultiplier);
                    //print(perlin3DStuff[0]);
                    for (int i = 0; i < perlin3DStuff.Length; i++) {
                        if (perlin3DStuff[i] < perlin3DThreshold) {
                            //swp.cornerValues[x, z, y, i] = perlin3DStuff[i];
                            corners.Add(i);
                        } else {
                            swp.data[x, z, y] = new MarchingBlock(LandType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z));
                        }
                    }
                    if (corners.Count >= 8) {
                        swp.data[x, z, y] = new MarchingBlock(LandType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                    } else {
                        swp.data[x, z, y] = new MarchingBlock(LandType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                    }

                    if (swp.data[x, z, y].landType != LandType.Air) {
                        //float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(scaledX + position.x, scaledZ + position.z, perlin2DScalar, false);
                        float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(x + position.x, z + position.z, perlin2DScalar, false);
                        for (int i = 0; i < perlinStuff.Length; i++) {
                            perlinStuff[i] *= perlin2DMultiplier;
                            perlinStuff[i] += heightBuffer;
                        }
                        for (int i = 0; i < perlinStuff.Length; i++) {
                            if (y >= perlinStuff[i]) {
                                int val = i * 2;
                                if (!corners.Contains(val)) {
                                    corners.Add(val);
                                }
                            }
                            if (y + 1 >= perlinStuff[i]) {
                                int val = (i * 2) + 1;
                                if (!corners.Contains(val)) {
                                    corners.Add(val);
                                }
                            }
                        }
                        //for (int i = 0; i < 8; i++) {
                        //    if (!corners.Contains(i)) {
                        //        swp.cornerValues[x, z, y, i] = 0;
                        //    } else {
                        //        swp.cornerValues[x, z, y, i] = 1;
                        //    }
                        //}
                        if (corners.Count >= 8) {
                            // if all vertices are greater than the perlin noise value (being above the "curve")
                            swp.data[x, z, y] = new MarchingBlock(LandType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        } else {
                            swp.data[x, z, y] = new MarchingBlock(LandType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        }
                    }
                }
            }
        }
        swp.setup();
        swp.renderMesh();
        swp.finalize();
        swp.initialPhysicsSetup();
        smoothWPs.Add(swp, swpObject);
    }
}