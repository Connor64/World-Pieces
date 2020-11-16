using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour {
    private Dictionary<WorldPiece, GameObject> worldPieces = new Dictionary<WorldPiece, GameObject>();
    private Dictionary<SmoothWP, GameObject> smoothWPs = new Dictionary<SmoothWP, GameObject>();
    private Dictionary<AngularWP, GameObject> angularWPs = new Dictionary<AngularWP, GameObject>();
    //private Dictionary<GameObject, Vector3> gameObjects = new Dictionary<GameObject, Vector3>();
    private List<GameObject> gameObjects = new List<GameObject>();
    private List<AngularWP> awps = new List<AngularWP>();
    public GameObject wpPref;
    public GameObject smoothWPPref;
    public GameObject angularWPPref;
    public Vector3 worldPieceSize;
    public Vector2 wpAmount;
    public float perlin2DMultiplier = 30f;
    public float perlin3DMultiplier = 0.05f;
    public float perlin3DThreshold = 0.38f;
    public float perlin2DScalar = 0.025f;
    public float perlin3DScalar = 0.079f;
    public float heightBuffer = 64f;
    public float voxelScalar = 1f;

    static readonly System.Object _obj = new System.Object();

    private float timeSinceRendered = 0;

    public static TerrainGenerator instance;
    public static List<AngularWP> renderList;
    private bool threadRunning = false;
    private Thread myThread;

    public bool regenerate = false;
    // Used during runtime to regenerate the chunk for debugging/testing. Automatically reset once enabled

    public HashSet<int> testCorners;
    private Vector2 perlinOffset;

    void Start() {
        renderList = new List<AngularWP>();

        perlinOffset = new Vector2(UnityEngine.Random.Range(0, 20000), UnityEngine.Random.Range(0, 20000));

        if (instance == null) {
            instance = gameObject.GetComponent<TerrainGenerator>();
        } else {
            Destroy(gameObject);
        }

        //for (int x = 0; x < wpAmount.x; x++) {
        //    for (int z = 0; z < wpAmount.y; z++) {
        //        //buildVoxelWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
        //        //buildSmoothWP(new Vector3(x * worldPieceSize.x * voxelScalar, 0, z * worldPieceSize.z * voxelScalar));
        //        buildWP(new Vector3(x * worldPieceSize.x * voxelScalar, 0, z * worldPieceSize.z * voxelScalar));
        //    }
        //}

        for (int x = 0; x < wpAmount.x; x++) {
            for (int z = 0; z < wpAmount.y; z++) {
                GameObject worldPiece = Instantiate(angularWPPref);
                worldPiece.GetComponent<AngularWP>().setPosition(new Vector3(x * worldPieceSize.x * voxelScalar, 0, z * worldPieceSize.z * voxelScalar));
                awps.Add(worldPiece.GetComponent<AngularWP>());
                //gameObjects.Add(Instantiate(angularWPPref), new Vector3(x * worldPieceSize.x * voxelScalar, 0, z * worldPieceSize.z * voxelScalar));
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
                    //buildSmoothWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
                    buildWP(new Vector3(x * worldPieceSize.x, 0, z * worldPieceSize.z));
                }
            }
            regenerate = false;
        }

        if (awps.Count > 0 && !threadRunning) {
            ThreadStart newThread = delegate {
                generateVoxels(awps[0].position, awps[0]);
            };
            myThread = new Thread(newThread);
            myThread.Start();
            threadRunning = true;
        }

        if (renderList.Count > 0) {
            renderList[0].setup();
            renderList[0].refreshMesh();
            renderList[0].finalize();
            renderList[0].initialPhysicsSetup();
            renderList[0].setPosition(new Vector3(renderList[0].position.x, 0, renderList[0].position.z));
            renderList.RemoveAt(0);
            timeSinceRendered = 0;
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
                    float[] perlin3DStuff = PerlinNoiseGen.marchingPerlin3D(x + position.x, y, z + position.z, perlin3DScalar, perlin3DMultiplier, perlinOffset);
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
                        float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(x + position.x, z + position.z, perlin2DScalar, false, perlinOffset);
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

    void buildWP(Vector3 position) {
        GameObject awpObject = Instantiate(angularWPPref);
        awpObject.transform.position = position;
        awpObject.layer = 8;
        AngularWP awp = awpObject.GetComponent<AngularWP>();
        awp.setSize(worldPieceSize, voxelScalar);
        // Instantiate a new world piece and set its position, layer, and size

        for (int x = 0; x < worldPieceSize.x; x++) {
            float scaledX = x * voxelScalar;
            for (int z = 0; z < worldPieceSize.z; z++) {
                float scaledZ = z * voxelScalar;
                for (int y = 0; y < worldPieceSize.y; y++) {
                    float scaledY = y * voxelScalar;
                    HashSet<int> corners = new HashSet<int>();
                    // Create an empty HashSet for the exposed corners of the current voxel
                    // Using a HashSet as it doesn't allow for duplicates

                    /* --------------------------------------------CAVE GENERATION--------------------------------------------
                     * 3D perlin noise is generated for each voxel (not necessarily random perlin noise PER voxel, but rather an array of values are created that represent
                     * the values in the 3D perlin noise at the voxel's corner's positions.
                     * Based on whether or not these corner values are within the threshold, the voxel becomes either air or dirt.
                     */
                    float[] perlin3DStuff = PerlinNoiseGen.marchingPerlin3D(x + position.x, y, z + position.z, perlin3DScalar, perlin3DMultiplier, new Vector3(perlinOffset.x, 0, perlinOffset.y));
                    // Generate a list of 3D perlin noise values at the corners of the current voxel
                    // Position values are based on the position of the world piece plus the current voxel's position within that world piece

                    for (int i = 0; i < perlin3DStuff.Length; i++) {
                        if (perlin3DStuff[i] < perlin3DThreshold) {
                            corners.Add(i);
                            // Adds the corners that are within the threshold values to the voxel's list of exposed corners.
                            // The corners are represented by the index of the loop as it iterates through the array of 3D perlin noise values for each corner of the voxel.
                        } else {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // (Possible spot for optimization as it may run this multiple times for no reason)
                        }
                    }
                    if (corners.Count >= 6) {
                        awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        // If there are more than 6 corners exposed then the voxel will be set as an air block as you cannot draw a triangle between the two remaining vertices.
                    } else {
                        awp.voxelData[x, z, y] = new Voxel(VoxelType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        // Otherwise, set the voxel as a dirt block and store its corners
                    }

                    /* --------------------------------------------SURFACE GENERATION--------------------------------------------
                     * During the Cave Generation segment above, all voxels are set as either air or dirt blocks, meaning none are null.
                     * Based on 2D perlin noise, a surface is created
                     */
                    if (awp.voxelData[x, z, y].voxelType != VoxelType.Air) {
                        //float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(scaledX + position.x, scaledZ + position.z, perlin2DScalar, false);
                        float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(x + position.x, z + position.z, perlin2DScalar, false, perlinOffset);
                        // Create an array of 2D perlin noise values at the top 4 corners of a voxel

                        for (int i = 0; i < perlinStuff.Length; i++) {
                            perlinStuff[i] *= perlin2DMultiplier;
                            perlinStuff[i] += heightBuffer;
                            // Alter the values based on the buffers and multipliers added

                            if (y >= perlinStuff[i]) {
                                // If the bottom of the current voxel is higher than the 2D perlin noise value at its corners' coordinates...

                                int bottomCorner = i * 2;
                                // Because "i" only iterates through 0-3, the only possible values that bottomCorner can be is 0, 2, 4, and 6, which are the values
                                // of the bottom corners of any given voxel.

                                if (!corners.Contains(bottomCorner)) {
                                    corners.Add(bottomCorner);
                                    // If the voxel does not already contain the exposed corner in its HashSet, then add it.
                                }
                            }
                            if (y + 1 >= perlinStuff[i]) {
                                // If the top of the current voxel is higher than the 2D perlin noise value at its corners' coordinates...

                                int topCorner = (i * 2) + 1;
                                // Because "i" only iterates through 0-3, the only possible values that bottomCorner can be is 1, 3, 5, and 7, which are the values
                                // of the top corners of any given voxel.

                                if (!corners.Contains(topCorner)) {
                                    corners.Add(topCorner);
                                    // If the voxel does not already contain the exposed corner in its HashSet, then add it.
                                }
                            }
                        }

                        if (corners.Count >= 6) {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // If all vertices are greater than the perlin noise value (being above the "curve"), set it as an air block
                        } else {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // Otherwise, set it as a dirt block
                        }
                    }
                }
            }
        }
        awp.setup();
        awp.renderMesh();
        awp.finalize();
        awp.initialPhysicsSetup();
        angularWPs.Add(awp, awpObject);
    }

    public void generateVoxels(Vector3 position, AngularWP awp) {
        awps.RemoveAt(0);
        Thread.Sleep(1);
        awp.setSize(worldPieceSize, voxelScalar);
        // Instantiate a new world piece and set its position, layer, and size

        for (int x = 0; x < worldPieceSize.x; x++) {
            float scaledX = x * voxelScalar;
            for (int z = 0; z < worldPieceSize.z; z++) {
                float scaledZ = z * voxelScalar;
                for (int y = 0; y < worldPieceSize.y; y++) {
                    float scaledY = y * voxelScalar;
                    HashSet<int> corners = new HashSet<int>();
                    // Create an empty HashSet for the exposed corners of the current voxel
                    // Using a HashSet as it doesn't allow for duplicates

                    /* --------------------------------------------CAVE GENERATION--------------------------------------------
                     * 3D perlin noise is generated for each voxel (not necessarily random perlin noise PER voxel, but rather an array of values are created that represent
                     * the values in the 3D perlin noise at the voxel's corner's positions.
                     * Based on whether or not these corner values are within the threshold, the voxel becomes either air or dirt.
                     */
                    float[] perlin3DStuff = PerlinNoiseGen.marchingPerlin3D(x + position.x, y, z + position.z, perlin3DScalar, perlin3DMultiplier, new Vector3(perlinOffset.x, 0, perlinOffset.y));
                    // Generate a list of 3D perlin noise values at the corners of the current voxel
                    // Position values are based on the position of the world piece plus the current voxel's position within that world piece

                    for (int i = 0; i < perlin3DStuff.Length; i++) {
                        if (perlin3DStuff[i] < perlin3DThreshold) {
                            corners.Add(i);
                            // Adds the corners that are within the threshold values to the voxel's list of exposed corners.
                            // The corners are represented by the index of the loop as it iterates through the array of 3D perlin noise values for each corner of the voxel.
                        } else {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // (Possible spot for optimization as it may run this multiple times for no reason)
                        }
                    }
                    if (corners.Count >= 6) {
                        awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        // If there are more than 6 corners exposed then the voxel will be set as an air block as you cannot draw a triangle between the two remaining vertices.
                    } else {
                        awp.voxelData[x, z, y] = new Voxel(VoxelType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                        // Otherwise, set the voxel as a dirt block and store its corners
                    }

                    /* --------------------------------------------SURFACE GENERATION--------------------------------------------
                     * During the Cave Generation segment above, all voxels are set as either air or dirt blocks, meaning none are null.
                     * Based on 2D perlin noise, a surface is created
                     */
                    if (awp.voxelData[x, z, y].voxelType != VoxelType.Air) {
                        //float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(scaledX + position.x, scaledZ + position.z, perlin2DScalar, false);
                        float[] perlinStuff = PerlinNoiseGen.marchingPerlin2D(x + position.x, z + position.z, perlin2DScalar, false, perlinOffset);
                        // Create an array of 2D perlin noise values at the top 4 corners of a voxel

                        for (int i = 0; i < perlinStuff.Length; i++) {
                            perlinStuff[i] *= perlin2DMultiplier;
                            perlinStuff[i] += heightBuffer;
                            // Alter the values based on the buffers and multipliers added

                            if (y >= perlinStuff[i]) {
                                // If the bottom of the current voxel is higher than the 2D perlin noise value at its corners' coordinates...

                                int bottomCorner = i * 2;
                                // Because "i" only iterates through 0-3, the only possible values that bottomCorner can be is 0, 2, 4, and 6, which are the values
                                // of the bottom corners of any given voxel.

                                if (!corners.Contains(bottomCorner)) {
                                    corners.Add(bottomCorner);
                                    // If the voxel does not already contain the exposed corner in its HashSet, then add it.
                                }
                            }
                            if (y + 1 >= perlinStuff[i]) {
                                // If the top of the current voxel is higher than the 2D perlin noise value at its corners' coordinates...

                                int topCorner = (i * 2) + 1;
                                // Because "i" only iterates through 0-3, the only possible values that bottomCorner can be is 1, 3, 5, and 7, which are the values
                                // of the top corners of any given voxel.

                                if (!corners.Contains(topCorner)) {
                                    corners.Add(topCorner);
                                    // If the voxel does not already contain the exposed corner in its HashSet, then add it.
                                }
                            }
                        }

                        if (corners.Count >= 6) {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Air, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // If all vertices are greater than the perlin noise value (being above the "curve"), set it as an air block
                        } else {
                            awp.voxelData[x, z, y] = new Voxel(VoxelType.Dirt, new Vector3(scaledX + position.x, scaledY, scaledZ + position.z), corners);
                            // Otherwise, set it as a dirt block
                        }
                    }
                }
            }
        }

        //awp.setup();
        awp.renderMesh();
        renderList.Add(awp);
        //awp.finalize();
        //awp.initialPhysicsSetup();
        threadRunning = false;
    }
}