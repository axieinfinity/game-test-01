using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle minimap logic
/// </summary>
public class MiniMap : MonoBehaviour
{
    [SerializeField] SpriteRenderer miniMapBG;
    [SerializeField] Camera miniMapCamera;
    [SerializeField] MiniMapDot prefabDot;
    [SerializeField] Transform dotZone;

    Dictionary<int, MiniMapDot> dots = new Dictionary<int, MiniMapDot>();
    private void OnEnable()
    {
        prefabDot.gameObject.SetActive(false);
        //OrthographicSize relate to height. Fix height to design resolution and calculate the width
        float newWidth = (Screen.width * GameConfig.DesignResolution.y) / Screen.height;
        Vector2 screenUnitSize = new Vector2(newWidth / GameConfig.PixelPerUnit, GameConfig.DesignResolution.y / GameConfig.PixelPerUnit);
        dotZone.transform.position = new Vector3((screenUnitSize.x - miniMapBG.size.x) / 2f, (screenUnitSize.y - miniMapBG.size.y) / 2f);
    }
    public void UpdateMapSize(Vector2 mapSize)
    {
        //Change minimap camera size base on mapSize to fit the fixed minimap background's size
        float miniMapScale = mapSize.y / (0.9f*miniMapBG.size.y);
        miniMapCamera.orthographicSize = GameConfig.InitialCameraSize * miniMapScale;
        miniMapCamera.transform.position = new Vector3(-dotZone.transform.position.x * miniMapScale, -dotZone.transform.position.y * miniMapScale, -1);
    }
    public void AddCharacter(Character character)
    {
        var dot = Instantiate(prefabDot, dotZone);
        dot.gameObject.SetActive(true);
        dot.SetCharacter(character);
        dots.Add(character.SpawnId, dot);
    }
    public void RemoveCharacter(Character character)
    {
        if (dots.ContainsKey(character.SpawnId))
        {
            Destroy(dots[character.SpawnId].gameObject);
            dots.Remove(character.SpawnId);
        }
    }
    private void Update()
    {
        foreach(var dot in dots.Values)
        {
            dot.transform.position = dot.Character.transform.position;
        }
    }
}
