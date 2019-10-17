using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var screenUnitSize = new Vector2(Screen.width / GameConfig.PixelPerUnit, Screen.height / GameConfig.PixelPerUnit);
        dotZone.transform.position = new Vector3((screenUnitSize.x - miniMapBG.size.x) / 2f, (screenUnitSize.y - miniMapBG.size.y) / 2f);
    }
    public void UpdateMapSize(Vector2 mapSize)
    {
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
