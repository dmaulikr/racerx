using UnityEngine;
using System.Collections;
using System;

public class TrackController : MonoBehaviour {

    [Serializable]
    public class TrackChunkCollection {
        public GameObject chunkStraight;
        public GameObject chunkDown;
        public GameObject chunkUp;
        public GameObject chunkTurnLeft;
        public GameObject chunkTurnRight;
    }

    [Header("Track Chunk Prefabs")]
    public TrackChunkCollection trackChunks;

    [Header("Editor Track Generator Parameters")]
    public int length = 5;
    public float difficulty = 0.5f;
    public int seed = 42;

    // Use this for initialization
    void Start () {
        GenerateTrack(); //remove
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CreateTrack(int seed, int length, float difficulty) {
        System.Random rand = new System.Random(seed);

        bool done = false;
        while (!done) {
            string name = "RacingTrack" + rand.Next(10000).ToString();
            GameObject trackBase = new GameObject(name);

            TrackGenerator generator = new TrackGenerator(trackBase, rand, trackChunks);
            generator.difficulty = difficulty;
            generator.length = length;

            try {
                generator.GenerateTrack();
                done = true;
            } catch (TrackGenerator.InvalidTrackException) {
               // Destroy(trackBase);
                Debug.Log("failed");
                return;
            }
        }
    }

    public void GenerateTrack() {
        CreateTrack(seed, length, difficulty);
    }
}
