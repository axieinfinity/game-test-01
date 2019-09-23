using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public partial class Model : MonoBehaviour
	{
        [SerializeField] public int ringCount = 1;
        [SerializeField] private UnitPreset presetAttacker, presetDefender;
        private int size;

        [Space(32)]
		public UnityEngine.Events.UnityEvent onReady;
        public UnityEngine.Events.UnityEvent onGameover;

        [System.Serializable] public class OnGameplayEvent : UnityEngine.Events.UnityEvent<Event> { }
		public OnGameplayEvent onGameplayEvent;

        private Tile centerTile;
		private List<Tile> tiles;
        private Tile[,] tileMap;

        private List<Unit> units;

        private Dictionary<Unit, Tile> mapUnitToTile;
        private Dictionary<Tile, Unit> mapTileToUnit;

        private void Awake()
        {
            if (Settings.enabled)
                ringCount = Settings.ringCount;
        }

        void Start()
        {
            SetupReport();
            RestartModel();
        }

        public void RestartModel()
        {
            RespawnTiles();
            RespawnUnits();
            onReady?.Invoke();
        }

        void RespawnTiles()
        {
            size = 1 + ringCount * 4 + 2;

            tiles = new List<Tile>();
            tileMap = new Tile[size, size];
            int index = 0;

            for (var row = 0; row < size; row++)
            {
                for (var col = 0; col < size; col++)
                {
                    var pos_x = (size - 1f) / 2 - col;
                    var pos_y = (size - 1f) / 2 - row;
                    bool tile_enabled = Mathf.Abs(pos_x + pos_y) <= size / 2;
                    if (tile_enabled == false) continue;

                    var newTile = new Tile()
                    {
                        index = index++,
                        position = new Vector2Int(col, row),
                        displayPosition = new Vector2(pos_x, pos_y),
                    };

                    tiles.Add(newTile);
                    tileMap[col, row] = newTile;
                }
            }

            centerTile = tileMap[size / 2, size / 2];
        }

        void RespawnUnits()
        {
            units = new List<Unit>();
            mapTileToUnit = new Dictionary<Tile, Unit>();
            mapUnitToTile = new Dictionary<Unit, Tile>();

            var cx = size / 2;
            var cy = size / 2;
            
            for (var y = -ringCount; y <= ringCount; y++)
                for (var x = -ringCount; x <= ringCount; x++)
                {
                    if (Mathf.Abs(x + y) >= (ringCount + 1)) continue;
                    var unit = SpawnUnitAtTile(cx + x, cy + y);
                }

            for (var y = -size / 2; y <= size / 2; y++)
                for (var x = -size / 2; x <= size / 2; x++)
                {
                    var isInnerRect = Mathf.Abs(x) <= ringCount + 1 && Mathf.Abs(y) <= ringCount + 1;
                    var isOuterRing = Mathf.Abs(x + y) >= (ringCount + 2);

                    if (isInnerRect && isOuterRing == false) continue;

                    var unit = SpawnUnitAtTile(cx + x, cy + y);
                    if (unit == null) continue;
                    unit.attacker = true;
                }

            foreach (var unit in units)
                unit.hp = unit.maxHp = (unit.attacker ? presetAttacker : presetDefender).hp;
        }

        Unit SpawnUnitAtTile(int x, int y)
        {
            if (tileMap[x, y] == null) return null;
            var index = units.Count;
            var tile = tileMap[x, y];

            if (mapTileToUnit.ContainsKey(tile)) return null;

            var unit = new Unit()
            {
                index = index,
                tileIndex = tile.index
            };
            units.Add(unit);

            mapTileToUnit[tile] = unit;
            mapUnitToTile[unit] = tile;

            return unit;
        }

        List<Unit> turnOrder = new List<Unit>();
        public void PlayNextRound()
        {
            onGameplayEvent?.Invoke(new EventTest());

            turnOrder.Clear();
            //iterate though tiles and capture units order
            foreach (var tile in IterateTileFromCenter())
            {
                if (mapTileToUnit.ContainsKey(tile) == false) continue;
                var unit = mapTileToUnit[tile];
                if (unit != null && unit.dead == false) turnOrder.Add(unit);
            }

            foreach (var unit in turnOrder)
                unit.countered = false;

            //foreach unit in the unit order
            foreach (var unit in turnOrder)
                PlayUnitTurn(unit);

            int attackerCount = 0;
            int defenderCount = 0;

            foreach (var unit in turnOrder)
            {
                RemoveDeadUnit(unit);
                if (unit.dead) continue;

                if (unit.attacker) attackerCount++;
                else defenderCount++;
            }

			UpdateReport();
			if (attackerCount == 0 || defenderCount == 0) onGameover?.Invoke();
        }

    }
}