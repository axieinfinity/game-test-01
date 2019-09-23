using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public class Runner : MonoBehaviour
	{

        [SerializeField] private Model model;
        [SerializeField] private FPSCounter fpsCounter;
        [SerializeField] private UI.Report uiReport;
        [SerializeField] private Display.TileDisplay baseTileDisplay;
        [SerializeField] private Display.UnitDisplay baseUnitDisplay;
        private List<Display.TileDisplay> tiles = new List<Display.TileDisplay>();
        private List<Display.UnitDisplay> units = new List<Display.UnitDisplay>();
        private bool gameover;

        public OnMaxViewportSizeChanged onMaxViewportSizeChanged;
        [System.Serializable] public class OnMaxViewportSizeChanged : UnityEngine.Events.UnityEvent<Rect> { }

        [SerializeField] private DisplayAction sampleAttackDisplayAction;


        private Dictionary<System.Type, System.Func<Event, IEnumerator>> mapEventToCoroutineFunction;

        private void Awake()
        {
            Application.targetFrameRate = 60;

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

            if (Settings.benchmarkMode) benchmarking = true;
        }

        bool benchmarking = false;

        public void FunctionModelOnReady()
        {
            gameover = false;

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

            onMaxViewportSizeChanged?.Invoke(CalculateViewportSize());

            StopAllCoroutines();

            if (benchmarking)
                StartCoroutine(BenchmarkDelay());
            else
                StartCoroutine(CoGameplay());
		}

        IEnumerator BenchmarkDelay()
        {
			uiReport.ReportBenchmarking(model.ringCount);

			yield return new WaitForSecondsRealtime(3);
            bool aboveFPS = fpsCounter.FPS >= Settings.minBenchmarkFPS;

            if (aboveFPS)
            {
                model.ringCount++;
            }
            else {
                model.ringCount--;
                benchmarking = false;
				uiReport.ReportBenchmarkingCompleted(model.ringCount);
			}
            model.RestartModel();
        }

		public Rect CalculateViewportSize()
		{
			Rect result = Rect.zero;
			foreach (var tile in tiles)
			{
				var pos = tile.transform.position;
                result.xMin = Mathf.Min(result.xMin, pos.x);
                result.xMax = Mathf.Max(result.xMax, pos.x);
                result.yMin = Mathf.Min(result.yMin, pos.y);
                result.yMax = Mathf.Max(result.yMax, pos.y);
            }
            return result;
		}

		IEnumerator CoGameplay()
        {
            while (true)
            {
                yield return new WaitForSeconds(3.5f);

                if (gameover) yield break;
                model.PlayNextRound();
            }
        }

        public void FunctionSetGameover()
        {
            gameover = true;
            StartCoroutine(CoGameover());
        }

        IEnumerator CoGameover()
        {
            yield return new WaitForSeconds(5);
            foreach (var unit in units)
                if (unit.gameObject.activeSelf) unit.PlayVictory();
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
            var unit = units[e.sourceIndex];
            var target = units[e.targetIndex];
            yield return unit.CoPlayAttack(target, sampleAttackDisplayAction, e);
        }

        IEnumerator OnEvent(EventUnitDead e)
        {
            var unit = units[e.index];
            yield return unit.CoPlayDead();

        }
	}
}
