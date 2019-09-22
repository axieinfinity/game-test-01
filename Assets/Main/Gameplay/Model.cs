using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public partial class Model : MonoBehaviour
	{
        [SerializeField] private int ringCount = 1;
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


        void Start()
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

        public IEnumerable<Tile> Tiles
        {
            get
            {
                foreach (var tile in tiles) yield return tile;
            }
        }

        public IEnumerable<Unit> Units
        {
            get
            {
                foreach (var unit in units) yield return unit;
            }
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

            if (attackerCount == 0 || defenderCount == 0) onGameover?.Invoke();
        }

        Tile GetTileAtPos(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) return null;
            return tileMap[x, y];
        }

        IEnumerable<Tile> IterateTileFromCenter()
        {
            var dia = ringCount * 2 + 1;
            foreach (var tile in IterateFromTile(tileMap[size / 2, size / 2], dia))
                yield return tile;
        }

        IEnumerable<Tile> IterateFromTile(Tile tile, int diameter)
        {
            if (tiles[tile.index] != tile) yield break;
            var x = tile.position.x;
            var y = tile.position.y;

            yield return tile;
            for (var i = 1; i <= diameter; i++)
            {
                y += 1;

                //ITERATE CLOCKWISE
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    x++; y--;
                }
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    y--;
                }
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    x--;
                }
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    x--; y++;
                }
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    y++;
                }
                for (var d = 0; d < i; d++)
                {
                    var t = GetTileAtPos(x, y); if (t != null) yield return t;
                    x++;
                }
            }
        }

        Vector3Int AxielToCube(Vector2Int axiel)
        {
            return new Vector3Int(axiel.x, -axiel.x - axiel.y, axiel.y);
        }

        Vector2Int CubeToAxiel(Vector3Int cube)
        {
            return new Vector2Int(cube.x, cube.z);
        }

        Vector2Int CubeToAxiel(Vector3 cube)
        {
            return new Vector2Int(Mathf.RoundToInt(cube.x), Mathf.RoundToInt(cube.z));
        }

        int Distance(Tile t1, Tile t2)
        {
            var pos1 = t1.position;
            var pos2 = t2.position;

            //convert axiel to cube format
            var cubePos1 = AxielToCube(pos1);
            var cubePos2 = AxielToCube(pos2);

            //calculate cube distance
            return Mathf.Max(
                Mathf.Abs(cubePos2.x - cubePos1.x),
                Mathf.Abs(cubePos2.y - cubePos1.y),
                Mathf.Abs(cubePos2.z - cubePos1.z));
        }

        static Vector3 epsilonCube = new Vector3(float.Epsilon, 2 * float.Epsilon, -3 * float.Epsilon);
        static Vector3[] cubeOffsets = {
            new Vector3(0, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(0, -1, 1),
            new Vector3(1, 0, -1),
            new Vector3(1, -1, 0),
            new Vector3(0, 1, -1),
            new Vector3(-1, 0, 1)
        };

        Tile GetNextFreeTileTowardTile(Tile tileFrom, Tile tileTo)
        {
            if (tileFrom == tileTo) return tileFrom;

            var dist = Distance(tileFrom, tileTo);
            Vector2Int pos1 = tileFrom.position;
            Vector2Int pos2 = tileTo.position;

            Vector3 cubePos1 = AxielToCube(pos1) + epsilonCube;
            Vector3 cubePos2 = AxielToCube(pos2) + epsilonCube;

            foreach (var offset in cubeOffsets)
            {
                var targetTilePos = CubeToAxiel(Vector3.Lerp(cubePos1 + offset, cubePos2, 1f / dist));
                var targetTile = GetTileAtPos(targetTilePos.x, targetTilePos.y);

                if (targetTile == null || mapTileToUnit.ContainsKey(targetTile)) continue;
                return targetTile;
            }

            return tileFrom;
        }

    }
}