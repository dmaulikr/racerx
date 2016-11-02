using UnityEngine;
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

    public GameObject CreateTrack(int seed, int length, float difficulty, bool ingame) {
        System.Random rand = new System.Random(seed);

        GameObject trackBase = null;

        bool done = false;
        while (!done) {
            string name = "RacingTrack" + rand.Next(10000).ToString();
            trackBase = new GameObject(name);

            TrackGenerator generator = new TrackGenerator(trackBase, rand, trackChunks);
            generator.difficulty = difficulty;
            generator.length = length;

            try {
                generator.GenerateTrack();
                done = true;
            } catch (TrackGenerator.InvalidTrackException) {
                if (ingame) {
                    Destroy(trackBase);
                }
            }
        }

        return trackBase;
    }

    public void GenerateTrack() {
        CreateTrack(seed, length, difficulty, false);
    }

    public void GenerateTrackRand() {
        CreateTrack(UnityEngine.Random.Range(1, 100000), length, difficulty, false);
    }
}
