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
    public Checkpoint checkpoint;

    [Header("Checkpoint Parameters")]
    public float checkpointFrequency = 1 / 10f;
    public int lastCheckpoint = 0;

    [Header("Editor Track Generator Parameters")]
    public int length = 5;
    public float difficulty = 0.5f;
    public int seed = 42;

    public RacingTrack CreateTrack(int seed, int length, float difficulty) {
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
                if (Application.isPlaying) {
                    Destroy(trackBase);
                }
            }
        }

        return trackBase.GetComponent<RacingTrack>();
    }

    public void GenerateTrack() {
        RacingTrack racingTrack = CreateTrack(seed, length, difficulty);
        GenerateCheckPointsAndStart(racingTrack);
    }

    public void GenerateTrackRand() {
        RacingTrack racingTrack = CreateTrack(UnityEngine.Random.Range(1, 100000), length, difficulty);
        GenerateCheckPointsAndStart(racingTrack);
    }

    public void GenerateCheckPointsAndStart(RacingTrack racingTrack) {
        int checkpointIndex = 0;
        //We don't want checkpoints in the last chunks
        for(int i = 1; i < racingTrack.trackChunks.Count - 1/checkpointFrequency; i++) {
            TrackChunk chunk = racingTrack.trackChunks[i];
            if((i * checkpointFrequency) % 1 <= 0.01) {
                Checkpoint clone = Instantiate(checkpoint, chunk.transform) as Checkpoint;
                
                switch(chunk.chunkType) {
                    case TrackChunk.ChunkType.CURVE_LEFT:
                        clone.transform.localPosition = new Vector3(-6, 0, -5);
                        break;
                    case TrackChunk.ChunkType.CURVE_RIGHT:
                        clone.transform.localPosition = new Vector3(6, 0, -5);
                        break;
                    case TrackChunk.ChunkType.STRAIGHT:
                        if (chunk.heightDiff == 1) {
                            clone.transform.localPosition = new Vector3(0, 5, 0);
                        } else if (chunk.heightDiff == -1) {
                            clone.transform.localPosition = new Vector3(0, -5, 0);
                        } else {
                            clone.transform.localPosition = Vector3.zero;
                        }
                        break;
                }
                clone.checkpointIndex = checkpointIndex;
                checkpointIndex++;
            }
        }
        lastCheckpoint = checkpointIndex;
        //TODO Generate Start/End Flag
    }
}
