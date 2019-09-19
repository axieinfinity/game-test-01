using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public class Runner : MonoBehaviour
	{

        [SerializeField] private Model model;
		[SerializeField] private Display.TileDisplay baseTileDisplay;
        private List<Display.TileDisplay> tiles = new List<Display.TileDisplay>();

        public void FunctionModelOnReady()
        {
            baseTileDisplay.gameObject.SetActive(false);
            baseTileDisplay.Reinstantiate(model.Tiles);

            tiles = new List<Display.TileDisplay>();
            foreach (Display.TileDisplay tile in baseTileDisplay.Instances)
                tiles.Add(tile);
        }
	}
}