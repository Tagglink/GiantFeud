using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    public static GameObject[][] tiles;

	// Use this for initialization
	void Start () {

        tiles = CreateTileMatrix();

    }

    GameObject[][] CreateTileMatrix()
    {
        GameObject[][] ret = new GameObject[15][] {
            new GameObject[1],
            new GameObject[2],
            new GameObject[3],
            new GameObject[4],
            new GameObject[5],
            new GameObject[6],
            new GameObject[7],
            new GameObject[8],
            new GameObject[7],
            new GameObject[6],
            new GameObject[5],
            new GameObject[4],
            new GameObject[3],
            new GameObject[2],
            new GameObject[1]
        };

        List<Transform> tileTransforms = new List<Transform>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            tileTransforms.Add(transform.GetChild(i));
        }

        tileTransforms = tileTransforms.OrderByDescending(t => t.position.y).ThenBy(t => t.position.x).ToList();

        int totalLoops = 0;

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < (i < 7 ? i + 1 : 15 - i); j++)
            {
                Debug.Log(totalLoops);
                ret[i][j] = tileTransforms[totalLoops].gameObject;
                totalLoops++;
            }
        }

        return ret;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
