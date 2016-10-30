using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    public SpriteRenderer tileSprite;

    void OnDisable()
    {
        Color32 col = tileSprite.color;
        col.a = 255;
        tileSprite.color = col;
    }

    public void SetColorFromHeight (float height)
    {
        Color32 col = tileSprite.color;
        col.a = (byte) (Mathf.Clamp(height * 255, 120, 255));
        tileSprite.color = col;

    }
}
