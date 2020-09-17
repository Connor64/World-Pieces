using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGen {
    public int seed;
    public static PerlinNoiseGen instance;

    private PerlinNoiseGen() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Attempted to create a second instance of PerlinNoiseGen.cs");
            return;
        }
    }


    //public PerlinNoiseGen (int seed) {
    //    this.seed = seed;
    //}

    void Start() {

    }

    void Update() {

    }

    public float getHeightValue(float x, float z) {
        return 0;
    }

    public static float perlin3D(float x, float y, float z, float scalar, float multiplier) {
        float _x = x * scalar;
        float _y = y * scalar;
        float _z = z * scalar;

        float XY = Mathf.PerlinNoise(_x, _y);
        float YZ = Mathf.PerlinNoise(_y, _z);
        float XZ = Mathf.PerlinNoise(_x, z);

        float YX = Mathf.PerlinNoise(_y, _x);
        float ZY = Mathf.PerlinNoise(_z, _y);
        float ZX = Mathf.PerlinNoise(_z, _x);

        return (((XY + YZ + XZ + YX + ZY + ZX) / 6) + (y * scalar / 12) * multiplier);
    }

    public static float perlin2D(float x, float y, float scalar, bool multiply) {
        float _x = x * scalar;
        float _y = y * scalar;

        float XY = Mathf.PerlinNoise(_x, _y);
        float YX = Mathf.PerlinNoise(_y, _x);

        if (multiply) {
            return (XY * YX);
        } else {
            return XY;
        }
    }

    public static float[] marchingPerlin3D(float x, float y, float z, float scalar, float multiplier) {
        float[] perlins = new float[] {
            perlin3D(x, y, z, scalar, multiplier),
            perlin3D(x, y + 1, z, scalar, multiplier),
            perlin3D(x + 1, y, z, scalar, multiplier),
            perlin3D(x + 1, y + 1, z, scalar, multiplier),

            perlin3D(x + 1, y, z + 1, scalar, multiplier),
            perlin3D(x + 1, y + 1, z + 1, scalar, multiplier),
            perlin3D(x, y, z + 1, scalar, multiplier),
            perlin3D(x, y + 1, z + 1, scalar, multiplier),
        };
        return perlins;
    }

    public static float[] marchingPerlin2D(float x, float z, float scalar, bool multiply) {
        float _x = x * scalar;
        float _z = z * scalar;
        float _x2 = (x + 1) * scalar;
        float _z2 = (z + 1) * scalar;

        float XZ = Mathf.PerlinNoise(_x, _z);
        float XZ2 = Mathf.PerlinNoise(_x2, _z);
        float XZ3 = Mathf.PerlinNoise(_x, _z2);
        float XZ4 = Mathf.PerlinNoise(_x2, _z2);

        if (multiply) {
            float ZX = Mathf.PerlinNoise(_z, _x);
            float ZX2 = Mathf.PerlinNoise(_z2, _x);
            float ZX3 = Mathf.PerlinNoise(_z, _x2);
            float ZX4 = Mathf.PerlinNoise(_z2, _x2);

            return new float[] { XZ * ZX, XZ2 * ZX2, XZ3 * ZX3, XZ4 * ZX4 };
        } else {
            return new float[] { XZ, XZ2, XZ4, XZ3 };
        }
    }

    //public static float sine2D(float x, float z, float scalar) {
        
    //}

    //public static float[] marching3DSin
}