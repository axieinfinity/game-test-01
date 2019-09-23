using UnityEngine;
using System.Collections;

namespace Gameplay.Display
{
    public class TileDisplay : Instantiable<Tile>
    {
        public Vector2 tileOffset;
        public override void OnCreate(Tile data)
        {
            var pos = data.displayPosition;
            pos.y += pos.x / 2;
            transform.localPosition = pos * tileOffset;
        }
    }

}