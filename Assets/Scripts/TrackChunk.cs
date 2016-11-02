using UnityEngine;
using System.Collections;

public class TrackChunk : MonoBehaviour {

    public enum ChunkType {
        STRAIGHT,
        CURVE_LEFT,
        CURVE_RIGHT
    }

    public int heightDiff = 0;
    public ChunkType chunkType = ChunkType.STRAIGHT;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
