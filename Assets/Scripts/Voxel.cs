using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelType {
    Dirt, Air
}

public class Voxel {

    public VoxelType voxelType;
    public Vector3 position;
    public HashSet<int> cornersExposed;

    public Voxel(VoxelType voxelType, Vector3 position, HashSet<int> cornersExposed) {
        this.voxelType = voxelType;
        this.position = position;
        this.cornersExposed = cornersExposed;
    }
}