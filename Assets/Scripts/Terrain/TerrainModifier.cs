using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier {

    private TerrainGenerator terrainGen = TerrainGenerator.instance;

    public static MarchingBlock removeCornersOLD(int x, int y, int z, int corner, SmoothWP swp) {
        MarchingBlock block = swp.data[x, z, y];
        HashSet<int> newCorners = block.cornersExposed;
        newCorners.Remove(corner);
        return new MarchingBlock(block.landType, block.position, newCorners);
    }

    public static void removeCorners(int x, int y, int z, int corner, SmoothWP swp) {
        //List<int> corners
        switch (corner) {
            case 0:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y - i].cornersExposed.Add(0 + i);
                    swp.data[x - 1, z, y - i].cornersExposed.Add(2 + i);
                    swp.data[x, z - 1, y - i].cornersExposed.Add(6 + i);
                    swp.data[x - 1, z - 1, y - i].cornersExposed.Add(4 + i);
                }
                break;
            case 1:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y + i].cornersExposed.Add(1 - i);
                    swp.data[x - 1, z, y + i].cornersExposed.Add(3 - i);
                    swp.data[x, z - 1, y + i].cornersExposed.Add(5 - i);
                    swp.data[x - 1, z - 1, y + i].cornersExposed.Add(6 - i);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y - i].cornersExposed.Add(2 + i);
                    swp.data[x + 1, z, y - i].cornersExposed.Add(0 + i);
                    swp.data[x, z - 1, y - i].cornersExposed.Add(4 + i);
                    swp.data[x + 1, z - 1, y - i].cornersExposed.Add(6 + i);
                }
                break;
            case 3:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y + i].cornersExposed.Add(3 - i);
                    swp.data[x + 1, z, y + i].cornersExposed.Add(1 - i);
                    swp.data[x, z - 1, y + i].cornersExposed.Add(5 - i);
                    swp.data[x + 1, z - 1, y + i].cornersExposed.Add(7 - i);
                }
                break;
            case 4:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y - i].cornersExposed.Add(4 + i);
                    swp.data[x + 1, z, y - i].cornersExposed.Add(6 + i);
                    swp.data[x, z + 1, y - i].cornersExposed.Add(2 + i);
                    swp.data[x + 1, z + 1, y - i].cornersExposed.Add(0 + i);
                }
                break;
            case 5:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y + i].cornersExposed.Add(5 - i);
                    swp.data[x + 1, z, y + i].cornersExposed.Add(7 - i);
                    swp.data[x, z + 1, y + i].cornersExposed.Add(3 - i);
                    swp.data[x + 1, z + 1, y + i].cornersExposed.Add(1 - i);
                }
                break;
            case 6:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y - i].cornersExposed.Add(6 + i);
                    swp.data[x - 1, z, y - i].cornersExposed.Add(4 + i);
                    swp.data[x, z + 1, y - i].cornersExposed.Add(0 + i);
                    swp.data[x - 1, z + 1, y - i].cornersExposed.Add(2 + i);
                }
                break;
            case 7:
                for (int i = 0; i < 2; i++) {
                    swp.data[x, z, y + i].cornersExposed.Add(7 - i);
                    swp.data[x - 1, z, y + i].cornersExposed.Add(5 - i);
                    swp.data[x, z + 1, y + i].cornersExposed.Add(1 - i);
                    swp.data[x - 1, z + 1, y + i].cornersExposed.Add(3 - i);
                }
                break;
            default:
                Debug.LogError("Index out of range");
                break;
        }
    }

    public static void removeCorners2(int x, int y, int z, int corner, AngularWP awp) {
        //List<int> corners
        switch (corner) {
            case 0:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y - i].cornersExposed.Add(0 + i);
                    awp.voxelData[x - 1, z, y - i].cornersExposed.Add(2 + i);
                    awp.voxelData[x, z - 1, y - i].cornersExposed.Add(6 + i);
                    awp.voxelData[x - 1, z - 1, y - i].cornersExposed.Add(4 + i);
                }
                break;
            case 1:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y + i].cornersExposed.Add(1 - i);
                    awp.voxelData[x - 1, z, y + i].cornersExposed.Add(3 - i);
                    awp.voxelData[x, z - 1, y + i].cornersExposed.Add(5 - i);
                    awp.voxelData[x - 1, z - 1, y + i].cornersExposed.Add(6 - i);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y - i].cornersExposed.Add(2 + i);
                    awp.voxelData[x + 1, z, y - i].cornersExposed.Add(0 + i);
                    awp.voxelData[x, z - 1, y - i].cornersExposed.Add(4 + i);
                    awp.voxelData[x + 1, z - 1, y - i].cornersExposed.Add(6 + i);
                }
                break;
            case 3:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y + i].cornersExposed.Add(3 - i);
                    awp.voxelData[x + 1, z, y + i].cornersExposed.Add(1 - i);
                    awp.voxelData[x, z - 1, y + i].cornersExposed.Add(5 - i);
                    awp.voxelData[x + 1, z - 1, y + i].cornersExposed.Add(7 - i);
                }
                break;
            case 4:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y - i].cornersExposed.Add(4 + i);
                    awp.voxelData[x + 1, z, y - i].cornersExposed.Add(6 + i);
                    awp.voxelData[x, z + 1, y - i].cornersExposed.Add(2 + i);
                    awp.voxelData[x + 1, z + 1, y - i].cornersExposed.Add(0 + i);
                }
                break;
            case 5:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y + i].cornersExposed.Add(5 - i);
                    awp.voxelData[x + 1, z, y + i].cornersExposed.Add(7 - i);
                    awp.voxelData[x, z + 1, y + i].cornersExposed.Add(3 - i);
                    awp.voxelData[x + 1, z + 1, y + i].cornersExposed.Add(1 - i);
                }
                break;
            case 6:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y - i].cornersExposed.Add(6 + i);
                    awp.voxelData[x - 1, z, y - i].cornersExposed.Add(4 + i);
                    awp.voxelData[x, z + 1, y - i].cornersExposed.Add(0 + i);
                    awp.voxelData[x - 1, z + 1, y - i].cornersExposed.Add(2 + i);
                }
                break;
            case 7:
                for (int i = 0; i < 2; i++) {
                    awp.voxelData[x, z, y + i].cornersExposed.Add(7 - i);
                    awp.voxelData[x - 1, z, y + i].cornersExposed.Add(5 - i);
                    awp.voxelData[x, z + 1, y + i].cornersExposed.Add(1 - i);
                    awp.voxelData[x - 1, z + 1, y + i].cornersExposed.Add(3 - i);
                }
                break;
            default:
                Debug.LogError("Index out of range");
                break;
        }
    }

    //public static void destroyVoxel(float x, float y, float z, SmoothWP swp) {
    //    int _x = Mathf.FloorToInt(x);
    //    int _y = Mathf.FloorToInt(y);
    //    int _z = Mathf.FloorToInt(z);
    //    float scalar = swp.voxelScalar;
    //    Vector3[] voxelCorners = new Vector3[] {
    //        new Vector3(_x, _y, _z),
    //        new Vector3(_x + scalar, _y, _z),
    //        new Vector3(_x, _y + scalar, _z),
    //        new Vector3(_x + scalar, _y + scalar, _z),

    //        new Vector3(_x + scalar, _y, _z + scalar),
    //        new Vector3(_x + scalar, _y + scalar, _z + scalar),
    //        new Vector3(_x, _y, _z + scalar),
    //        new Vector3(_x, _y + scalar, _z + scalar)
    //    };
    //    HashSet<int> corners = swp.data[_x, _z, _y].cornersExposed;
    //    Vector3 cursorPosition = new Vector3(x, y, z);

    //    int cornerSelected = 10;
    //    float distance = 1;

    //    for (int i = 0; i < voxelCorners.Length; i++) {
    //        float newDistance = Vector3.Distance(voxelCorners[i], cursorPosition);
    //        if (newDistance < distance && !corners.Contains(i)) {
    //            cornerSelected = i;
    //            distance = newDistance;
    //        }
    //    }

    //    if (cornerSelected != 10) {
    //        removeCorners(_x, _y, _z, cornerSelected, swp);
    //    } else {
    //    }
    //    swp.renderMesh();
    //    swp.finalize();
    //    swp.updatePhysics();
    //}

    public static void destroyVoxel(float x, float y, float z, AngularWP awp) {
        int _x = Mathf.FloorToInt(x);
        int _y = Mathf.FloorToInt(y);
        int _z = Mathf.FloorToInt(z);
        float scalar = awp.voxelScalar;
        Vector3[] voxelCorners = new Vector3[] {
            new Vector3(_x, _y, _z),
            new Vector3(_x + scalar, _y, _z),
            new Vector3(_x, _y + scalar, _z),
            new Vector3(_x + scalar, _y + scalar, _z),

            new Vector3(_x + scalar, _y, _z + scalar),
            new Vector3(_x + scalar, _y + scalar, _z + scalar),
            new Vector3(_x, _y, _z + scalar),
            new Vector3(_x, _y + scalar, _z + scalar)
        };
        HashSet<int> corners = awp.voxelData[_x, _z, _y].cornersExposed;
        Vector3 cursorPosition = new Vector3(x, y, z);

        int cornerSelected = 10;
        float distance = 1;

        for (int i = 0; i < voxelCorners.Length; i++) {
            float newDistance = Vector3.Distance(voxelCorners[i], cursorPosition);
            if (newDistance < distance && !corners.Contains(i)) {
                cornerSelected = i;
                distance = newDistance;
            }
        }

        if (cornerSelected != 10) {
            removeCorners2(_x, _y, _z, cornerSelected, awp);
        } else {
        }
        awp.refreshMesh();
        awp.renderMesh();
        awp.finalize();
        awp.updatePhysics();
    }

    //public static void placeVoxel(float x, float y, float z, SmoothWP swp) {
    //    int _x = Mathf.FloorToInt(x);
    //    int _y = Mathf.FloorToInt(y);
    //    int _z = Mathf.FloorToInt(z);

    //    if (swp.data[_x, _z, _y].landType == LandType.Air) {
    //        swp.data[_x, _z, _y].landType = LandType.Dirt;
    //        swp.data[_x, _z, _y].position = new Vector3(x, y, z);
    //        swp.data[_x, _z, _y].cornersExposed = new HashSet<int> { 1, 3, 7, 5 };
    //        //swp.setup();
    //        swp.renderMesh();
    //        swp.finalize();
    //    } else {
    //        Debug.LogError("NOT AIR");
    //    }
    //}

    public static void placeVoxel(float x, float y, float z, AngularWP awp) {
        int _x = Mathf.FloorToInt(x);
        int _y = Mathf.FloorToInt(y);
        int _z = Mathf.FloorToInt(z);

        if (awp.voxelData[_x, _z, _y].voxelType == VoxelType.Air) {
            awp.voxelData[_x, _z, _y].voxelType = VoxelType.Dirt;
            awp.voxelData[_x, _z, _y].position = new Vector3(x, y, z);
            awp.voxelData[_x, _z, _y].cornersExposed = new HashSet<int> { 1, 3, 7, 5 };
            //swp.setup();
            awp.refreshMesh();
            awp.renderMesh();
            awp.finalize();
        } else {
            Debug.LogError("NOT AIR");
        }
    }

    public static void hideVoxel(float x, float y, float z, AngularWP awp) {
        int _x = Mathf.FloorToInt(x);
        int _y = Mathf.FloorToInt(y);
        int _z = Mathf.FloorToInt(z);

        if (awp.voxelData[_x, _z, _y].voxelType != VoxelType.Air) {
            awp.voxelData[_x, _z, _y].render = false;
            awp.refreshMesh();
            awp.renderMesh();
            awp.finalize();
        }
    }

    public static HashSet<int> remainingCorners(HashSet<int> corners) {
        int[] full = { 1, 3, 5, 7, 0, 2, 4, 6 };
        HashSet<int> other = new HashSet<int>();
        foreach (int i in full) {
            if (!corners.Contains(i)) {
                other.Add(i);
            }
        }
        return other;
    }

    // The main voxel is surrounded by 6 other voxels that have sides facing outward 
    public static void updateVoxel(float x, float y, float z, AngularWP awp, bool destroy) {
        int _x = Mathf.FloorToInt(x);
        int _y = Mathf.FloorToInt(y);
        int _z = Mathf.FloorToInt(z);

        // top, bottom, left, right, front, back, current
        Voxel[] neighbors = new Voxel[] {
            awp.voxelData[_x, _z, _y + 1],
            awp.voxelData[_x, _z, _y - 1],
            awp.voxelData[_x - 1, _z, _y],
            awp.voxelData[_x + 1, _z, _y],
            awp.voxelData[_x, _z + 1, _y],
            awp.voxelData[_x, _z - 1, _y],
            awp.voxelData[_x, _z, _y],
        };

        if (destroy) {
            for (int i = 0; i < neighbors.Length; i++) {
                if (neighbors[i].voxelType != VoxelType.Air) {
                    switch (i) {
                        case 0:
                            // top

                            neighbors[i].facesToRender.Add(1);
                            Debug.Log("butts");
                            // Make the voxel above the main voxel render its bottom face (index 1)

                            neighbors[i].cornersExposed.Add(0);
                            neighbors[i].cornersExposed.Add(2);
                            neighbors[i].cornersExposed.Add(4);
                            neighbors[i].cornersExposed.Add(6);
                            break;
                        case 1:
                            // bottom

                            neighbors[i].facesToRender.Add(0);
                            // Make the voxel below the main voxel render its top face (index 0)

                            //neighbors[i].cornersExposed.Add(1);
                            //neighbors[i].cornersExposed.Add(3);
                            //neighbors[i].cornersExposed.Add(5);
                            //neighbors[i].cornersExposed.Add(7);
                            break;
                        case 2:
                            // left

                            neighbors[i].facesToRender.Add(3);
                            // Make the voxel to the left of the main voxel render its right face (index 3)

                            //neighbors[i].cornersExposed.Add(2);
                            //neighbors[i].cornersExposed.Add(3);
                            //neighbors[i].cornersExposed.Add(4);
                            //neighbors[i].cornersExposed.Add(5);
                            break;
                        case 3:
                            // right

                            neighbors[i].facesToRender.Add(2);
                            // Make the voxel to the right of the main voxel render its left face (index 2)

                            //neighbors[i].cornersExposed.Add(0);
                            //neighbors[i].cornersExposed.Add(1);
                            //neighbors[i].cornersExposed.Add(6);
                            //neighbors[i].cornersExposed.Add(7);
                            break;
                        case 4:
                            // front

                            neighbors[i].facesToRender.Add(5);
                            // Make the voxel in the front of the main voxel render its back face (index 5)

                            //neighbors[i].cornersExposed.Add(0);
                            //neighbors[i].cornersExposed.Add(1);
                            //neighbors[i].cornersExposed.Add(2);
                            //neighbors[i].cornersExposed.Add(3);
                            break;
                        case 5:
                            // back

                            neighbors[i].facesToRender.Add(4);
                            // Make the voxel behind the main voxel render its front face (index 4)

                            //neighbors[i].cornersExposed.Add(4);
                            //neighbors[i].cornersExposed.Add(5);
                            //neighbors[i].cornersExposed.Add(6);
                            //neighbors[i].cornersExposed.Add(7);
                            break;
                        case 6:
                            neighbors[i].addRemaining();
                            neighbors[i].facesToRender.Clear();
                                break;
                        default:
                            Debug.LogError("Too many cubes in destruction array?? What? Why? Idk what's happening...");
                            break;
                    }
                    neighbors[i].updateCorners();
                }
            }
        }
        awp.refreshMesh();
        awp.renderMesh();
        awp.finalize();
        awp.updatePhysics();
    }
}