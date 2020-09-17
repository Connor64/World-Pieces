using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LandType {
    Dirt, Air
}

public class MarchingBlock {

    public LandType landType;
    public Vector3 position;
    public List<int> cornersEncapsulated;

    public MarchingBlock(LandType landType, Vector3 position, List<int> cornersEncapsulated) {
        this.landType = landType;
        this.position = position;
        this.cornersEncapsulated = cornersEncapsulated;
    }

    public MarchingBlock(LandType landType, Vector3 position) {
        this.landType = landType;
        this.position = position;
        cornersEncapsulated = null;
    }

}