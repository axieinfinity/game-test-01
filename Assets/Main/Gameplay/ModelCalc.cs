using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
    public partial class Model : MonoBehaviour
    {

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

        Vector3Int AxialToCube(Vector2Int axial)
        {
            return new Vector3Int(axial.x, -axial.x - axial.y, axial.y);
        }

        Vector2Int CubeToAxial(Vector3Int cube)
        {
            return new Vector2Int(cube.x, cube.z);
        }

        Vector2Int CubeToAxial(Vector3 cube)
        {
            return new Vector2Int(Mathf.RoundToInt(cube.x), Mathf.RoundToInt(cube.z));
        }

        int Distance(Tile t1, Tile t2)
        {
            var pos1 = t1.position;
            var pos2 = t2.position;

            //convert axial to cube format
            var cubePos1 = AxialToCube(pos1);
            var cubePos2 = AxialToCube(pos2);

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

            Vector3 cubePos1 = AxialToCube(pos1) + epsilonCube;
            Vector3 cubePos2 = AxialToCube(pos2) + epsilonCube;

            foreach (var offset in cubeOffsets)
            {
                var targetTilePos = CubeToAxial(Vector3.Lerp(cubePos1 + offset, cubePos2, 1f / dist));
                var targetTile = GetTileAtPos(targetTilePos.x, targetTilePos.y);

                if (targetTile == null || mapTileToUnit.ContainsKey(targetTile)) continue;
                return targetTile;
            }

            return tileFrom;
        }
    }
}
