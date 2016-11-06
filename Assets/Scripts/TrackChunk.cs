using UnityEngine;
using System.Collections;

public class TrackChunk : MonoBehaviour {

    public enum ChunkType {
        STRAIGHT,
        CURVE_LEFT,
        CURVE_RIGHT
    }

    public int heightDiff = 0; // 1: Ramp up, -1: Ramp down, 0: straight
    public ChunkType chunkType = ChunkType.STRAIGHT;
    public int chunkIndex = 0;
}
