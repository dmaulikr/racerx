using UnityEngine;
using System.Collections;

public class TrackController : MonoBehaviour {
    public GameObject[] trackChunks;
    public int trackChunkCount = 5;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GenerateTrack() {
        if (trackChunks.Length == 0) {
            Debug.LogError("No track chunks are avaiable.");
            return;
        }

        GameObject trackBase = new GameObject("RacingTrack" + Random.Range(0, 10000).ToString());
        GameObject lastChunk = null;

        for (int i = 0; i < trackChunkCount; i++) {
            GameObject chunkTemplate = trackChunks[Random.Range(0, trackChunks.Length)];

            GameObject newChunk = Instantiate(chunkTemplate);
            newChunk.transform.SetParent(trackBase.transform);

            if (lastChunk != null) {
                MountPoint lastMp = lastChunk.GetComponentInChildren<MountPoint>();
                if (lastMp == null) {
                    Debug.LogError("Unable to find mount point in track chunk.");
                    return;
                }

                newChunk.transform.position = lastMp.transform.position;
                Debug.Log(lastMp.transform.position);
                newChunk.transform.rotation = lastMp.transform.rotation;
            }

            lastChunk = newChunk;
        }
    }
}
