using UnityEngine;
using System.Collections;

public class TrackController : MonoBehaviour {

    [Header("Track Chunk Prefabs")]
    public TrackChunk[] trackChunks;

    [Header("Editor Track Generator Parameters")]
    public int length = 5;
    public float difficulty = 0.5f;
    public int seed = 42;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private TrackChunk GetRandomChunk(int heightDiff) {
        TrackChunk chunk;
        do {
            chunk = trackChunks[Random.Range(0, trackChunks.Length)];
        } while (chunk.heightDiff != heightDiff);

        return chunk;
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
                Destroy(trackBase);
            }
        }
    }

    public void GenerateTrack() {
        if (trackChunks.Length == 0) {
            Debug.LogError("No track chunks are avaiable.");
            return;
        }

        CreateTrack(seed, length, difficulty);
    }
}
