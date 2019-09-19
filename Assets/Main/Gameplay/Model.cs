using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public class Model : MonoBehaviour
	{
        public int ringCount = 1;

        [Space(32)]
		public UnityEngine.Events.UnityEvent onReady;
        private List<Tile> tiles;
        private Tile[,] tileMap;


        void Start()
		{
            RespawnTiles();
			onReady?.Invoke();
        }

        void RespawnTiles()
        {
            var size = 1 + ringCount * 4 + 2;

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
        }


        public IEnumerable<Tile> Tiles
        {
            get
            {
                foreach (var tile in tiles) yield return tile;
            }
        }


    }
}