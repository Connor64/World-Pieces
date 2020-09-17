using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType {
    Dirt, Air
}

public class Block {
    public BlockType blockType;
    public Vector3 position;

    public Block(BlockType blockType, Vector3 position) {
        this.blockType = blockType;
        this.position = position;
    }


    void Start() {
        
    }

    void Update() {
        
    }
}
