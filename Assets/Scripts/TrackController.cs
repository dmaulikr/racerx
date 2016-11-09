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
    public Finish finish;

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

    public RacingTrack GenerateTrack() {
        RacingTrack racingTrack = CreateTrack(seed, length, difficulty);
        generateCheckPointsAndStart(racingTrack);
        return racingTrack;
    }

    public RacingTrack GenerateTrackRand() {
        RacingTrack racingTrack = CreateTrack(UnityEngine.Random.Range(1, 100000), length, difficulty);
        generateCheckPointsAndStart(racingTrack);
        return racingTrack;
    }

    private void generateCheckPointsAndStart(RacingTrack racingTrack) {
        int checkpointIndex = 0;
        for(int i = 1; i < racingTrack.trackChunks.Count - 1/checkpointFrequency; i++) {
            TrackChunk chunk = racingTrack.trackChunks[i];
            if((i * checkpointFrequency) % 1 <= 0.01) {
                Checkpoint clone = Instantiate(checkpoint, chunk.transform) as Checkpoint;
                setFlagPosition(chunk, clone.transform);
                clone.checkpointIndex = checkpointIndex;
                checkpointIndex++;
            }
        }
        lastCheckpoint = checkpointIndex-1;
        TrackChunk finishChunk = racingTrack.trackChunks[0];
        Finish finishFlag = Instantiate(finish, finishChunk.transform) as Finish;
        setFlagPosition(finishChunk, finishFlag.transform);
    }

    private void setFlagPosition(TrackChunk trackChunk, Transform transform) {
        switch(trackChunk.chunkType) {
            case TrackChunk.ChunkType.CURVE_LEFT:
                transform.localPosition = new Vector3(-6, 0, -5);
                break;
            case TrackChunk.ChunkType.CURVE_RIGHT:
                transform.localPosition = new Vector3(6, 0, -5);
                break;
            case TrackChunk.ChunkType.STRAIGHT:
                if(trackChunk.heightDiff == 1) {
                    transform.localPosition = new Vector3(0, 5, 0);
                } else if(trackChunk.heightDiff == -1) {
                    transform.localPosition = new Vector3(0, -5, 0);
                } else {
                    transform.localPosition = Vector3.zero;
                }
                break;
        }
    }
}
