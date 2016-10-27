using System.Collections.Generic;
using UnityEngine;

class TrackGenerator {

    public class InvalidTrackException : System.Exception {
        public InvalidTrackException(string msg) : base(msg) {
        }
    }

    private struct IntVector3 {
        public int x, y, z;
    }

    private class TrackNode {
        public int rotation;
        public TrackChunk trackChunk;
        public int x, y, z;

        public TrackNode(IntVector3 pos) {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public TrackNode(IntVector3 pos, int rotation, TrackChunk trackChunk) : this(pos) {
            this.rotation = rotation;
            this.trackChunk = trackChunk;
        }

        public override int GetHashCode() {
            unchecked
            {
                int result = 17;

                result = result * 31 + x.GetHashCode();
                result = result * 31 + y.GetHashCode();
                result = result * 31 + z.GetHashCode();

                return result;
            }
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
               
            TrackNode trackNode = obj as TrackNode;
            return trackNode.x == x && trackNode.y == y && trackNode.z == z;
        }
    }

    private enum GenerationStatus {
        OPENING,
        GENERATING,
        RISING,
        CLOSING
    }

    public const int CHUNK_SIZE = 40;
    public const int CHUNK_HEIGHT = 10;

    public const int MIN_LENGTH = 30;
    public const int MAX_LENGTH = 1000;
    public const int MAX_HEIGHT = 10;

    public const float HEIGHT_CHANGE_PROB = 0.2f;
    public const float DIRECTION_TURN_PROB = 0.9f;

    private GameObject trackBase;
    private System.Random rand;
    private TrackChunk[] chunkPrefabs;
    private TrackChunk startChunk;

    public float difficulty;
    public int length;

    public TrackGenerator(GameObject trackBase, System.Random rand, TrackChunk[] chunkPrefabs) {
        this.trackBase = trackBase;
        this.rand = rand;
        this.chunkPrefabs = chunkPrefabs;
        startChunk = null;
    }

    private TrackChunk GetRandomChunk(TrackChunk.ChunkType direction, int heightDiff) {
        List<TrackChunk> chunks = new List<TrackChunk>();

        foreach (TrackChunk chunk in chunkPrefabs) {
            if (chunk.chunkType == direction && chunk.heightDiff == heightDiff) {
                chunks.Add(chunk);
            }
        }

        if (chunks.Count == 0) {
            throw new InvalidTrackException("Unable to find chunk with specified properties.");
        }

        return chunks[rand.Next(0, chunks.Count)];
    }

    public void GenerateTrack() {
        difficulty = Mathf.Clamp01(difficulty);
        length = Mathf.Clamp(length, MIN_LENGTH, MAX_LENGTH);

        HashSet<TrackNode> nodes = new HashSet<TrackNode>();

        // Start at max height, one block forward
        IntVector3 cursor = new IntVector3();
        cursor.y = MAX_HEIGHT;
        cursor.z = 1;

        float heightChangeProb = HEIGHT_CHANGE_PROB * difficulty;
        int remainingChunks = length;
        int currentRotation = 0;
        bool done = false;
        GenerationStatus status = GenerationStatus.OPENING;

        while (!done) {
            int heightDiff = 0;

            switch (status) {
                case GenerationStatus.OPENING:
                    if (cursor.y == 0) {
                        status = GenerationStatus.GENERATING;
                        continue;
                    }

                    heightDiff = -1;

                    break;
                case GenerationStatus.GENERATING:

                    if (rand.NextDouble() < heightChangeProb) {
                        heightDiff = (rand.NextDouble() < 0.5 ? 1 : -1);
                    }

                    break;
                case GenerationStatus.RISING:
                    if (cursor.y == MAX_HEIGHT) {
                        status = GenerationStatus.CLOSING;
                        continue;
                    }

                    heightDiff = 1;
                    
                    break;
                case GenerationStatus.CLOSING:
                    done = true;
                    break;
            }

            // Decide Direction
            TrackChunk.ChunkType direction = TrackChunk.ChunkType.STRAIGHT;
            if (heightDiff == 0 && rand.NextDouble() < DIRECTION_TURN_PROB * difficulty) {
                if (rand.NextDouble() < 0.5) {
                    direction = TrackChunk.ChunkType.CURVE_LEFT;
                } else {
                    direction = TrackChunk.ChunkType.CURVE_RIGHT;
                }
            }

            int nextRotation = currentRotation;

            // Update rotation
            if (direction == TrackChunk.ChunkType.CURVE_RIGHT) {
                nextRotation += 90;
            } else if (direction == TrackChunk.ChunkType.CURVE_LEFT) {
                nextRotation -= 90;
            }

            while (nextRotation >= 360) { nextRotation -= 360; }
            while (nextRotation < 0) { nextRotation += 360; }

            IntVector3 nextPos = GetNextPoint(cursor, nextRotation, heightDiff);

            if (HasNodeAt(nodes, nextPos)) {
                continue;
            }

            TrackChunk chunk = GetRandomChunk(direction, heightDiff);
            TrackNode node = new TrackNode(cursor, currentRotation, chunk);
            nodes.Add(node);

            currentRotation = nextRotation;
            cursor = nextPos;
            remainingChunks--;
            if (remainingChunks == 0) {
                status = GenerationStatus.RISING;
            }
        }

        foreach (TrackNode node in nodes) {
            GameObject chunkClone = Object.Instantiate(node.trackChunk.gameObject);
            chunkClone.transform.SetParent(trackBase.transform);

            chunkClone.transform.position = new Vector3(
                node.x * CHUNK_SIZE,
                node.y * CHUNK_HEIGHT,
                node.z * CHUNK_SIZE);

            chunkClone.transform.rotation = Quaternion.Euler(0, node.rotation, 0);
        }
    }

    private bool HasNodeAt(HashSet<TrackNode> nodes, IntVector3 pos) {
        return nodes.Contains(new TrackNode(pos));
    }

    private IntVector3 GetNextPoint(IntVector3 currentPos, int rotation, int heightDiff) {
        switch (rotation) {
            case 0:
                currentPos.z++;
                break;
            case 90:
                currentPos.x++;
                break;
            case 180:
                currentPos.z--;
                break;
            case 270:
                currentPos.x--;
                break;
        }

        currentPos.y += heightDiff;
        return currentPos;
    }
}
