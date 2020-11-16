using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelType {
    Dirt, Air
}

public class Voxel {

    public bool render;
    public VoxelType voxelType;
    public Vector3 position;
    public HashSet<int> cornersExposed;
    public HashSet<int> facesToRender = new HashSet<int>();

    private int[] full = { 1, 3, 7, 5, 0, 2, 4, 6 };

    public Voxel(VoxelType voxelType, Vector3 position, HashSet<int> cornersExposed) {
        render = true;
        this.voxelType = voxelType;
        this.position = position;
        this.cornersExposed = cornersExposed;
    }

    public void updateCorners() {
        if (cornersExposed.Count >= 8) {
            voxelType = VoxelType.Air;
        }
    }

    public void addRemaining() {
        foreach (int i in full) {
            if (!cornersExposed.Contains(i)) {
                cornersExposed.Add(i);
            }
        }
    }
}