using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LandType {
    Dirt, Air
}

public class MarchingBlock {

    public LandType landType;
    public Vector3 position;
    public HashSet<int> cornersExposed;
    private float[] cornerValues;

    public MarchingBlock[] surroundingVoxels;

    public MarchingBlock(LandType landType, Vector3 position, HashSet<int> cornersExposed) {
        this.landType = landType;
        this.position = position;
        this.cornersExposed = cornersExposed;
    }

    public MarchingBlock(LandType landType, Vector3 position) {
        this.landType = landType;
        this.position = position;
        cornersExposed = null;
    }

    void setCornerValues(float[] values) {
        cornerValues = values;
    }

    float[] getCornerValues() {
        return cornerValues;
    }

    void reEvaluateCorners(float threshold) {
        cornersExposed.Clear();
        for (int i = 0; i < 8; i++) {
            if (cornerValues[i] < threshold) {
                cornersExposed.Add(i);
            }
        }
    }

    void exposeCorner(int corner) {
        if (landType != LandType.Air && cornersExposed.Count < 8) {
            cornersExposed.Add(corner);
        }
        if (cornersExposed.Count >= 8) {
            landType = LandType.Air;
        }
    }
}