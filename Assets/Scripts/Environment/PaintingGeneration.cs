using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingGeneration : MonoBehaviour
{

    // The paiting prefab
    public GameObject paintingPrefab;

    private float maxPaintingWidth = 3.5f;
    private float maxPaintingHeight = 3.5f;
    private float minPaintingWidth = 2.0f;
    private float minPaintingHeight = 2.0f;
    private float paintingDepth = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
      // Get all the paiting objects (locations)
      foreach (Transform child in transform) {

        // Greate a new paintingPrefab
        var newPainting = Instantiate(paintingPrefab, child.position, Quaternion.identity);

        // Set painting parent
        newPainting.transform.parent = child;

        // Set paiting size
        newPainting.transform.localScale = new Vector3(paintingDepth, Random.Range(minPaintingWidth, maxPaintingWidth), Random.Range(minPaintingHeight, maxPaintingHeight));
      }
    }
}
