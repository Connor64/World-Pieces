using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier {

    private TerrainGenerator terrainGen = TerrainGenerator.instance;

    public static MarchingBlock removeCorners(int x, int y, int z, int corner, SmoothWP swp) {
        MarchingBlock block = swp.data[x, z, y];
        HashSet<int> newCorners = block.cornersExposed;
        newCorners.Remove(corner);
        return new MarchingBlock(block.landType, block.position, newCorners);
    }

    public static void removeMoreCorners(int x, int y, int z, int corner, SmoothWP swp) {
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
}