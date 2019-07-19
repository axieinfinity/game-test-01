using UnityEngine;

public class Stage : MonoBehaviour,
    IView<StageModel>,
    IModel
{
    [SerializeField] private Transform _spawnPlayerPos;
    [SerializeField] private Transform _spawnEnemyPos;
    [SerializeField] private Transform _ground;

    public Vector3 PlayerPos => _spawnPlayerPos
        .position;

    public Vector3 EnemyPos => _spawnEnemyPos
        .position;

    public void UpdateView(StageModel model)
    {
        _ground.transform.rotation = Quaternion.Euler(model.GroundRotationX,
            0,
            0);
        var localPos = new Vector3(_ground.transform.localPosition.x,
            model.GroundPositionY,
            _ground.transform.localPosition.z);
        _ground.transform.localPosition = localPos;
        _ground.GetComponent<SpriteRenderer>()
            .size = new Vector2(model.SpriteRendererWidth,
            model.SpriteRendererHeight);
    }
}