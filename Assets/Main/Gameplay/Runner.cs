using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public class Runner : MonoBehaviour
	{

        [SerializeField] private Model model;
		[SerializeField] private Display.TileDisplay baseTileDisplay;
        [SerializeField] private Display.UnitDisplay baseUnitDisplay;
        private List<Display.TileDisplay> tiles = new List<Display.TileDisplay>();
        private List<Display.UnitDisplay> units = new List<Display.UnitDisplay>();

        public OnMaxViewportSizeChanged onMaxViewportSizeChanged;
        [System.Serializable] public class OnMaxViewportSizeChanged : UnityEngine.Events.UnityEvent<float> { }

        private static Dictionary<System.Type, System.Func<Event, IEnumerator>> mapEventToCoroutineFunction;

        private void Awake()
        {
            if (mapEventToCoroutineFunction != null) return;
            mapEventToCoroutineFunction = new Dictionary<System.Type, System.Func<Event, IEnumerator>>();
            var type = this.GetType();
            foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                if (method.Name != "OnEvent") continue;
                if (method.ReturnType != typeof(IEnumerator)) continue;
                var parameters = method.GetParameters();
                if (parameters.Length != 1 && parameters[0].ParameterType.IsSubclassOf(typeof(Event)) == false) continue;

                mapEventToCoroutineFunction[parameters[0].ParameterType] = (e) =>
                {
                    return method.Invoke(this, new object[] { e }) as IEnumerator;
                };
            }
        }

        public void FunctionModelOnReady()
        {
            baseTileDisplay.gameObject.SetActive(false);
            baseTileDisplay.Reinstantiate(model.Tiles);

            tiles = new List<Display.TileDisplay>();
            foreach (Display.TileDisplay tile in baseTileDisplay.Instances)
                tiles.Add(tile);

            baseUnitDisplay.gameObject.SetActive(false);
            baseUnitDisplay.Reinstantiate(model.Units);

            units = new List<Display.UnitDisplay>();
            foreach (Display.UnitDisplay unit in baseUnitDisplay.Instances)
                units.Add(unit);

            foreach (var unit in units)
            {
                var tile = tiles[unit.data.tileIndex];
                unit.transform.position = tile.transform.position;
            }

            StopAllCoroutines();
            StartCoroutine(CoGameplay());
            
        }

        IEnumerator CoGameplay()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                model.PlayNextRound();
            }
        }

        public void FunctionPlayEvent(Event e)
        {
            if (mapEventToCoroutineFunction.ContainsKey(e.GetType()))
                StartCoroutine(mapEventToCoroutineFunction[e.GetType()](e));
        }

        IEnumerator OnEvent(EventTest e)
        {
            yield break;
        }

        IEnumerator OnEvent(EventUnitMoveToTile e)
        {
            var unit = units[e.unitIndex];
            var tile = tiles[e.tileIndex];
            yield return unit.CoMoveToTile(tile);
        }

        IEnumerator OnEvent(EventUnitAttackUnit e)
        {
            yield return true;
        }
	}
}
