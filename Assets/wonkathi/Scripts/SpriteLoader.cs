using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Autoload sprite from SpriteAtlas
/// </summary>
public class SpriteLoader : MonoBehaviour
{
    [SerializeField] private string spriteName;
    Image image;
    SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (string.IsNullOrEmpty(spriteName))
        {
            if (image != null && image.sprite != null)
                spriteName = image.sprite.name;
            else if (spriteRenderer != null && spriteRenderer.sprite != null)
                spriteName = spriteRenderer.sprite.name;
        }
        Load(spriteName);
    }

    private void Load(string sName)
    {
        if(image != null)
        {
            image.sprite = SpriteManager.Inst.GetSprite(sName);
            return;
        }
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = SpriteManager.Inst.GetSprite(sName);
        }
    }
}
