using UnityEngine;
using System.Collections;

public enum TileType { NONE, WOODS, STONE, WATER, CROPS, CATTLE, GIANTS, CAMP }
public enum TileState { NONE, READY, HARVESTED }

public class Tile : MonoBehaviour {

    public TileType type; // set in inspector
    public TileState state;
    public bool occupied;

    /*
     * 0: Ready sprite
     * 1: Ready Highlighted sprite
     * 2: Harvested sprite
     * 3: Harvested Highlighted sprite
     */
    public Sprite[] spriteSheet;

    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        state = TileState.READY;
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetHighlighted(bool enable)
    {
        if (state == TileState.HARVESTED)
        {
            if (enable)
                spriteRenderer.sprite = spriteSheet[3];
            else
                spriteRenderer.sprite = spriteSheet[2];
        }
        else
        {
            if (enable)
                spriteRenderer.sprite = spriteSheet[1];
            else
                spriteRenderer.sprite = spriteSheet[0];
        }
    }

    public void SetHarvested(bool enable)
    {
        if (enable)
        {
            state = TileState.HARVESTED;
            spriteRenderer.sprite = spriteSheet[2];
        }
        else
        {
            state = TileState.READY;
            spriteRenderer.sprite = spriteSheet[0];
        }
    }
}
