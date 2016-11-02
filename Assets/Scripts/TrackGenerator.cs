using System.Collections.Generic;
using UnityEngine;

class TrackGenerator {

    public class InvalidTrackException : System.Exception {
        public InvalidTrackException(string msg) : base(msg) {
        }
    }

    private class SearchNode {
        public SearchNode parent;
        public TrackChunk.ChunkType parentType = TrackChunk.ChunkType.STRAIGHT;
        public int parentHeightDiff = 0;
        public IntVector3 pos;
        public int rotation;

        public int g;
        public float h;

        public SearchNode(SearchNode parent, int rotation, IntVector3 pos) {
            this.parent = parent;
            this.pos = pos;
            this.rotation = rotation;
        }

        public override int GetHashCode() {
            unchecked {
                return pos.GetHashCode() * 31 + rotation.GetHashCode();
            }
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            SearchNode searchNode = obj as SearchNode;
            return searchNode.pos.Equals(pos) && searchNode.rotation == rotation;
        }
    }

    private class TrackNode {
        public int rotation;
        public IntVector3 pos;

        public TrackChunk.ChunkType chunkType;
        public int heightDiff;

        public TrackNode(IntVector3 pos) {
            this.pos = pos;
        }

        public TrackNode(IntVector3 pos, int rotation) : this(pos) {
            this.rotation = rotation;
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
               
            TrackNode trackNode = obj as TrackNode;
            return trackNode.pos.Equals(pos);
        }
    }

    public const int CHUNK_SIZE = 40;
    public const int CHUNK_HEIGHT = 10;

    public const int MIN_LENGTH = 20;
    public const int MAX_LENGTH = 500;

    public const float HEIGHT_CHANGE_PROB = 0.2f;
    public const float DIRECTION_TURN_PROB = 0.9f;

    public const int INITIAL_GENERATION_TIMEOUT = 50;
    public const int A_STAR_TIMEOUT = 50;

    public const int FIRST_STRETCH_LEN = 3;

    private GameObject trackBase;
    private System.Random rand;
    private TrackController.TrackChunkCollection chunkPrefabs;

    public float difficulty;
    public int length;

    public TrackGenerator(GameObject trackBase, System.Random rand, TrackController.TrackChunkCollection chunkPrefabs) {
        this.trackBase = trackBase;
        this.rand = rand;
        this.chunkPrefabs = chunkPrefabs;
    }

    private GameObject GetChunkPrefab(TrackChunk.ChunkType direction, int heightDiff) {
        GameObject result = null;

        if (direction == TrackChunk.ChunkType.STRAIGHT) {
            if (heightDiff == 0) {
                result = chunkPrefabs.chunkStraight;
            } else if (heightDiff == -1) {
                result = chunkPrefabs.chunkDown;
            } else if (heightDiff == 1) {
                result = chunkPrefabs.chunkUp;
            }
        } else if (direction == TrackChunk.ChunkType.CURVE_LEFT) {
            result = chunkPrefabs.chunkTurnLeft;
        } else if (direction == TrackChunk.ChunkType.CURVE_RIGHT) {
            result = chunkPrefabs.chunkTurnRight;
        }

        return result;
    }

    public void GenerateTrack() {
        difficulty = Mathf.Clamp01(difficulty);
        length = Mathf.Clamp(length, MIN_LENGTH, MAX_LENGTH);

        HashSet<TrackNode> nodes = new HashSet<TrackNode>();

        // Start one block forward
        IntVector3 cursor = new IntVector3();
        cursor.z = 1;

        float heightChangeProb = HEIGHT_CHANGE_PROB * difficulty;
        int remainingChunks = length;
        int currentRotation = 0;
        bool done = false;

        int firstStretchLen = 0;
        int countdown = INITIAL_GENERATION_TIMEOUT * length;

        while (!done) {
            if (countdown-- < 0) {
                throw new InvalidTrackException("Initial track generation timed out.");
            }

            int heightDiff = 0;
            TrackChunk.ChunkType direction = TrackChunk.ChunkType.STRAIGHT;

            if (firstStretchLen > FIRST_STRETCH_LEN) {
                if (rand.NextDouble() < heightChangeProb) {
                    heightDiff = (rand.NextDouble() < 0.5 ? 1 : -1);
                }

                // Decide Direction
                if (heightDiff == 0 && rand.NextDouble() < DIRECTION_TURN_PROB * difficulty) {
                    if (rand.NextDouble() < 0.5) {
                        direction = TrackChunk.ChunkType.CURVE_LEFT;
                    } else {
                        direction = TrackChunk.ChunkType.CURVE_RIGHT;
                    }
                }
            }

            firstStretchLen++;

            int nextRotation = currentRotation;

            // Update rotation
            if (direction == TrackChunk.ChunkType.CURVE_RIGHT) {
                nextRotation += 90;
            } else if (direction == TrackChunk.ChunkType.CURVE_LEFT) {
                nextRotation -= 90;
            }

            nextRotation = ClampRotation(nextRotation);

            IntVector3 nextPos = GetNextPoint(cursor, nextRotation, 0);

            if (HasNodeAt(nodes, nextPos)) {
                continue;
            }

            if (heightDiff == 0) {
                IntVector3 nextPosUp = GetNextPoint(cursor, nextRotation, 1);
                IntVector3 nextPosDown = GetNextPoint(cursor, nextRotation, -1);

                if (HasNodeAt(nodes, nextPosUp) || HasNodeAt(nodes, nextPosDown)) {
                    continue;
                }
            } else {
                IntVector3 nextPosHDiff = GetNextPoint(cursor, nextRotation, heightDiff);
                if (HasNodeAt(nodes, nextPosHDiff)) {
                    continue;
                }
            }

            nextPos.y += heightDiff;

            TrackNode node = new TrackNode(cursor, currentRotation);
            node.chunkType = direction;
            node.heightDiff = heightDiff;
            nodes.Add(node);

            currentRotation = nextRotation;
            cursor = nextPos;

            if (firstStretchLen > FIRST_STRETCH_LEN) {
                remainingChunks--;
            }

            if (remainingChunks == 0) {
                done = true;
            }
        }

        // Now, close the circuit

        bool ok = CloseCircuit(nodes, cursor, currentRotation);

        // Instance prefabs
        InstanceChunkClones(nodes);

        if (!ok) {
            throw new InvalidTrackException("A* pathfinding timed out.");
        }
    }

    private bool CloseCircuit(HashSet<TrackNode> nodes, IntVector3 start, int startRotation) {
        // use A* to close track
        // Track ends at (0, 0, 0) rotation 0
        // Start from where cursor left off
        int endRotation = 0;
        IntVector3 end = new IntVector3();

        HashSet<SearchNode> closed = new HashSet<SearchNode>();
        LinkedList<SearchNode> open = new LinkedList<SearchNode>();

        SearchNode first = new SearchNode(null, startRotation, start);
        first.g = 0;
        first.h = start.dst(ref end);
        open.AddLast(first);

        int countdown = A_STAR_TIMEOUT * length;
        HashSet<IntVector3> currentPathPositions = new HashSet<IntVector3>();

        while (open.Count > 0) {

            if (countdown-- < 0) {
                return false;
            }

            SearchNode current = open.First.Value;
            open.RemoveFirst();

            if (closed.Contains(current)) {
                continue;
            }

            closed.Add(current);

            if (current.pos.Equals(end) && current.rotation == endRotation) {
                FinalizeCloseCircuit(nodes, current);
                return true;
            }

            currentPathPositions.Clear();
            SearchNode tmp = current;
            while (tmp != null) {
                currentPathPositions.Add(tmp.pos);
                tmp = tmp.parent;
            }

            List<SearchNode> neighbours = GetNeighboursFor(nodes, current);
            foreach (SearchNode neighbour in neighbours) {
                if (closed.Contains(neighbour) || currentPathPositions.Contains(neighbour.pos)) {
                    continue;
                }

                neighbour.g = current.g; //+ 1;
                neighbour.h = neighbour.pos.dst(ref end);
                AddOrdered(open, neighbour);
            }
        }

        return false;
    }

    private void AddOrdered(LinkedList<SearchNode> nodes, SearchNode node) {
        if (nodes.Count == 0) {
            nodes.AddFirst(node);
            return;
        }

        LinkedListNode<SearchNode> listNode = nodes.First;
        while (listNode.Value.g + listNode.Value.h < node.g + node.h) {
            listNode = listNode.Next;
            if (listNode == null) {
                break;
            }
        }

        if (listNode != null) {
            nodes.AddBefore(listNode, node);
        } else {
            nodes.AddLast(node);
        }
    }

    private List<SearchNode> GetNeighboursFor(HashSet<TrackNode> track, SearchNode node) {
        List<SearchNode> neighbours = new List<SearchNode>();

        // Forward flat
        IntVector3 nextPos = GetNextPoint(node.pos, node.rotation, 0);
        IntVector3 nextPosUp = GetNextPoint(node.pos, node.rotation, 1);
        IntVector3 nextPosDown = GetNextPoint(node.pos, node.rotation, -1);

        if (!HasNodeAt(track, nextPos) && !HasNodeAt(track, nextPosUp) && !HasNodeAt(track, nextPosDown)) {
            SearchNode n = new SearchNode(node, node.rotation, nextPos);
            neighbours.Add(n);
        }

        // Forward up
        if (!HasNodeAt(track, nextPos) && !HasNodeAt(track, nextPosUp)) {
            SearchNode n = new SearchNode(node, node.rotation, nextPosUp);
            n.parentHeightDiff = 1;
            neighbours.Add(n);
        }

        // Forward Down
        if (!HasNodeAt(track, nextPos) && !HasNodeAt(track, nextPosDown)) {
            SearchNode n = new SearchNode(node, node.rotation, nextPosDown);
            n.parentHeightDiff = -1;
            neighbours.Add(n);
        }

        IntVector3 nextPosLeft = GetNextPoint(node.pos, ClampRotation(node.rotation - 90), 0);
        IntVector3 nextPosRight = GetNextPoint(node.pos, ClampRotation(node.rotation + 90), 0);

        // Left
        if (!HasNodeAt(track, nextPosLeft)) {
            SearchNode n = new SearchNode(node, ClampRotation(node.rotation - 90), nextPosLeft);
            n.parentType = TrackChunk.ChunkType.CURVE_LEFT;
            neighbours.Add(n);
        }

        // Right
        if (!HasNodeAt(track, nextPosRight)) {
            SearchNode n = new SearchNode(node, ClampRotation(node.rotation + 90), nextPosRight);
            n.parentType = TrackChunk.ChunkType.CURVE_RIGHT;
            neighbours.Add(n);
        }

        return neighbours;
    }

    private void FinalizeCloseCircuit(HashSet<TrackNode> track, SearchNode endNode) {
        Stack<SearchNode> path = new Stack<SearchNode>();
        SearchNode current = endNode;

        while (current != null) {
            path.Push(current);
            current = current.parent;
        }

        while (path.Count > 0) {
            SearchNode node = path.Pop();
            SearchNode next = null;
            if (path.Count > 0) {
                next = path.Peek();
            }

            int heightDiff = 0;
            TrackChunk.ChunkType chunkType = TrackChunk.ChunkType.STRAIGHT;

            if (next != null) {
                heightDiff = next.parentHeightDiff;
                chunkType = next.parentType;
            }

            TrackNode trackNode = new TrackNode(node.pos, node.rotation);
            trackNode.heightDiff = heightDiff;
            trackNode.chunkType = chunkType;
            track.Add(trackNode);
        }
    }

    private static int ClampRotation(int r) {
        while (r >= 360) { r -= 360; }
        while (r < 0) { r += 360; }
        return r;
    }

    private void InstanceChunkClones(HashSet<TrackNode> nodes) {
        foreach (TrackNode node in nodes) {
            GameObject chunkPrefab = GetChunkPrefab(node.chunkType, node.heightDiff);
            GameObject chunkClone = Object.Instantiate(chunkPrefab);
            chunkClone.transform.SetParent(trackBase.transform);

            chunkClone.transform.position = new Vector3(
                node.pos.x * CHUNK_SIZE,
                node.pos.y * CHUNK_HEIGHT,
                node.pos.z * CHUNK_SIZE);

            chunkClone.transform.rotation = Quaternion.Euler(0, node.rotation, 0);

            TrackChunk tc = chunkClone.AddComponent<TrackChunk>();
            tc.heightDiff = node.heightDiff;
            tc.chunkType = node.chunkType;
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
