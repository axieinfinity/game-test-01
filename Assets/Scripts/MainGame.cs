using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainGame : MonoBehaviour
{
    public const float CAMERA_DEFAULT_SIZE = 4f;
    public const float CAMERA_WIN_SIZE = 3f;
    
    const float TIME_MOVE = 1f;
    const float TIME_DEFENCE = 0.5f;
    const float TIME_ATTACK = 0.5f;
    const float TIME_DIE = 0.5f;
    
    public int MaxDefendeRadius = 2;
    public float MinFps = 30f;
    
    [SerializeField] HUDFPS hudFps;
    [SerializeField] View viewTemplate;
    [SerializeField] Text textCounter;

    [SerializeField] PanAndZoom panAndZoom;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera cameraMiniMap;
    [SerializeField] RawImage miniMapCamera;
    [SerializeField] Image miniMapRect;

    [SerializeField] Text textPowerDefender;
    [SerializeField] Text textPowerAttacker;
    [SerializeField] Slider powerBarSlider; // defender in left
    
    protected List2<List2<List2Null<Data>>> datas = new List2<List2<List2Null<Data>>>();
    protected List2<List2<List2Null<View>>> views = new List2<List2<List2Null<View>>>();
    protected Dictionary<int, List<Data>> dataByRadius = new Dictionary<int, List<Data>>();

    //list to easy foreach
    protected Dictionary<CellType, List<View>> dictViews = new Dictionary<CellType, List<View>>()
    {
        {CellType.Empty, new List<View>()},
        {CellType.Attacker, new List<View>()},
        {CellType.Defender, new List<View>()},
    };

    protected Stack<View> pools = new Stack<View>();
    protected int totalCharacter;
    protected int defenceRadius;
    protected float totalMapHeight;
    protected int attackedRadius = 0;
    protected int powerDefender;
    protected int powerAttacker;
    protected bool paused = false;

    public void Pause()
    {
        paused = true;
    }

    public void Resume()
    {
        paused = false;
    }

    #region Initialize
    
    void Awake()
    {
        #if !UNITY_EDITOR
        MaxDefendeRadius = int.MaxValue;
        MinFps = 30;
        #endif
        
        viewTemplate.gameObject.SetActive(false);
        
        panAndZoom.onPinchedCamera += delegate { PanAndZoomOnChange(); };
        panAndZoom.onSwipedCamera += delegate { PanAndZoomOnChange(); };

        miniMapRect.GetComponent<Draggable>().onDragged += OnMiniMapDragged;
    }

    public void NewGame ()
    {
        StopAllCoroutines();
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitForSeconds(2f);
        ResetData();
        Resume();
        CreateMinimap();
        yield return null;
        yield return GenerateUnlimited();
        yield return null;
        StartCoroutine(GameLoop());
    }
    
    void ResetData()
    {
        foreach (var pair in dictViews)
        {
            foreach (View view in pair.Value)
            {
                view.gameObject.SetActive(false);
                pools.Push(view);
            }
        }
        
        defenceRadius = 0;
        totalCharacter = 0;
        powerDefender = 0;
        powerAttacker = 0;
        datas = new List2<List2<List2Null<Data>>>();
        dataByRadius = new Dictionary<int, List<Data>>();
        views = new List2<List2<List2Null<View>>>();
        dictViews = new Dictionary<CellType, List<View>>()
        {
            {CellType.Empty, new List<View>()},
            {CellType.Attacker, new List<View>()},
            {CellType.Defender, new List<View>()},
        };
    }

    public IEnumerator WaitExit()
    {
        //yield return new WaitForSeconds(1f);
        //ResetData();
        mainCamera.orthographicSize = CAMERA_DEFAULT_SIZE;
        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.localPosition.z);
        gameObject.SetActive(false);
        yield break;
    }

    IEnumerator GenerateUnlimited()
    {
        var data000 = CreateData(0, 0, 0, CellType.Defender);
        AddView(data000);

        while (true)
        {
            if (paused)
            {
                yield return null;
                continue;
            }
            
            defenceRadius++;

            UpdateCameraSize();

            List<Data> listAdd = new List<Data>();
            List<Data> listRemove = new List<Data>();

            //remove attacker from circle defenceRadius + 1
            listRemove.AddRange(RemoveDataInRadius(defenceRadius + 1));

            //add defender to circle defenceRadius
            var listDefender = CreateDataInRadius(defenceRadius, CellType.Defender); 
            listAdd.AddRange(listDefender);
            
            //add attacker to circle defenceRadius + 1 + defenceRadius - 1
            if (defenceRadius > 1)
            {
                listAdd.AddRange(CreateDataInRadius(2 * defenceRadius, CellType.Attacker));
            }

            //add attacker to circle defenceRadius + 1 + defenceRadius
            listAdd.AddRange(CreateDataInRadius(2 * defenceRadius + 1, CellType.Attacker));

            // remove view
            foreach (Data data in listRemove)
            {
                var view = GetViewAt(data.pos.x, data.pos.y, data.pos.z);
                RemoveView(data, view);
                yield return null;
            }
            
            // add view
            foreach (Data data in listAdd)
            {
                if (data != null && data.type != CellType.Empty)
                {
                    AddView(data);
                    yield return null;
                }
            }
            
            // check to finish
            yield return new WaitForSeconds(1f);
            
            if (hudFps.fps < MinFps || defenceRadius >= MaxDefendeRadius)
            {
                Debug.LogError("Finish! Total Character: " + totalCharacter);
                break;
            }
        }
    }

    #endregion

    
    #region Game Loop
    
    IEnumerator GameLoop()
    {
        attackedRadius = defenceRadius + 1;
        while (true)
        {
            if (paused)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(0.5f);
            var list = LoopUpdateData();
            LoopUpdateRender(list);
            yield return new WaitForSeconds(1.2f); // time to animation
            LoopReUpdateData(list);

            var result = GetWinner();
            if (result != CellType.Empty)
            {
                yield return new WaitForSeconds(1f);
                ShowConfetti(result);
                break;
            }
        }
    }

    void LoopReUpdateData(HashSet<Data> changedDatas)
    {
        foreach (Data data in changedDatas)
        {
            if (data.type != CellType.Empty)
            {
                var view = GetViewAt(data.pos.x, data.pos.y, data.pos.z);

                if (data.hp <= 0)
                {
                    RemoveData(data.pos.x, data.pos.y, data.pos.z);
                    
                    if (view != null)
                    {
                        view.AnimationDie(TIME_DIE, RemoveView);
                    }
                }
                else if (data.activity == Activity.Move)
                {
                    //clear current position
                    datas[data.pos.x][data.pos.y][data.pos.z] = null;
                    views[data.pos.x][data.pos.y][data.pos.z] = null;
                    
                    //change
                    data.pos = data.target;
                    view.UpdatePosition();
                    
                    //move in array of datas + views
                    datas[data.target.x][data.target.y][data.target.z] = data;
                    views[data.target.x][data.target.y][data.target.z] = view;
                    
                    //clear used data
                    data.target = null;
                    data.activity = Activity.Idle;
                }

                data.activity = Activity.Idle;
                data.target = null;
            }
        }
    }

    void LoopUpdateRender(HashSet<Data> changedDatas)
    {
        foreach (Data data in changedDatas)
        {
            if (data.type != CellType.Empty)
            {
                var view = GetViewAt(data.pos.x, data.pos.y, data.pos.z);
                
                if (data.activity == Activity.Attack)
                {
                    view.AnimationAttack(TIME_ATTACK);
                } 
                else if (data.activity == Activity.Defence)
                {
                    view.AnimationDefende(TIME_DEFENCE);
                }
                else if (data.activity == Activity.Move)
                {
                    view.AnimationMove(TIME_MOVE);
                }
            }
        }
    }

    HashSet<Data> LoopUpdateData()
    {
        HashSet<Data> changedDatas = new HashSet<Data>();
        //update data

        bool randx = Random.Range(0, 2) == 0;
        int fromX = randx ? datas.From + 1 : datas.To - 1;
        int toX = randx ? datas.To - 1 : datas.From + 1;
        
        for (int x = fromX; randx ? x <= toX : x >= toX; x += randx ? 1 : -1)
        {
            var randy = Random.Range(0, 2) == 0;
            int fromY = randy ? datas[x].From + 1 : datas[x].To - 1;
            int toY = randy ? datas[x].To - 1 : datas[x].From + 1;
            
            for (int y = fromY; randy ? y <= toY : y >= toY; y += randy ? 1 : -1)
            {
                var randz = Random.Range(0, 2) == 0;
                int fromZ = randz ? datas[x][y].From + 1 : datas[x][y].To - 1;
                int toZ = randz ? datas[x][y].To - 1 : datas[x][y].From + 1;

                for (int z = fromZ; randz ? z <= toZ : z >= toZ; z += randz ? 1 : -1)
                {
                    if (datas[x][y][z] != null && datas[x][y][z].type == CellType.Attacker)
                    {
                        bool changed = false;
                        var data = datas[x][y][z];
                        var pos = new Int3(x, y, z);
                        //if (data.type == CellType.Attacker)
                        {
                            var posNeighbors = GetNeighborPositions(pos);
                            
                            List<Int3> nullNeighbors = new List<Int3>();
                            foreach (Int3 posNeighbor in posNeighbors)
                            {
                                if (data.hp <= 0 || data.activity != Activity.Idle)
                                    break;
                                
                                //list neighbors
                                var neighbor = GetDataAt(posNeighbor.x, posNeighbor.y, posNeighbor.z);
                                if (neighbor != null)
                                {
                                    if (neighbor.type == CellType.Defender && neighbor.hp > 0)
                                    {
                                        Attack(data, neighbor);
                                        changedDatas.Add(neighbor);
                                        changed = true;
                                    }
                                }
                                else //null
                                {
                                    nullNeighbors.Add(posNeighbor);
                                }
                            }

                            if (data.activity == Activity.Idle)
                            {
                                if (nullNeighbors.Count > 0)
                                {
                                    changed = changed || CheckMove(data, pos, nullNeighbors, changedDatas);
                                }
                            }
                        }

                        if (changed)
                        {
                            changedDatas.Add(data);
                        }
                    }
                }
            }
        }

        return changedDatas;
    }

    #endregion

    
    #region Logic Support
    
    // return true means something changed
    public bool CheckMove(Data myData, Int3 myPos, List<Int3> nullNeighbors, HashSet<Data> changedDatas)
    {
        int shortestDistance = int.MaxValue;
        Int3 shortestNeighbor = null;
        
        int radiusFrom = Math.Max(attackedRadius - 1, 0);
        for (int radius = radiusFrom; radius <= defenceRadius; radius++)
        {
            foreach (Data target in dataByRadius[radius])
            {
                int distance = Distance(myPos, target.pos);
                float N = distance;
                Vector3 A = myPos.ToVector3();
                Vector3 B = target.pos.ToVector3();
                float i = 1f;
                Int3 neighbor = Int3.Round(A + (B - A) * (i / N));
                if (nullNeighbors.Contains(neighbor))
                {
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        shortestNeighbor = neighbor;
                    }
                }
            }
        }

        if (shortestNeighbor != null)
        {
            myData.activity = Activity.Move;
            myData.target = shortestNeighbor;
            
            //add empty place hoder to target
            var empty = CreateData(shortestNeighbor.x, shortestNeighbor.y, shortestNeighbor.z, CellType.Empty);
            changedDatas.Add(empty);
            
            return true;
        }

        return false;
    }

    void Attack(Data attacker, Data defender)
    {
        var radiusOfDefender = GetRadius(defender.pos);
        if (attackedRadius > radiusOfDefender)
            attackedRadius = radiusOfDefender;
        
        if (attacker.hp <= 0 || defender.hp <= 0)
            return;
        
        int attackNumber = Random.Range(0, 3);
        int defendNumber = Random.Range(0, 3);
        int attachDamage = GetDamage(attackNumber, defendNumber);
        int defendDamage = GetDamage(defendNumber, attackNumber);

        if (attachDamage > defender.hp)
            attachDamage = defender.hp;
        if (defendDamage > attacker.hp)
            defendDamage = attacker.hp;
        
        defender.hp -= attachDamage;
        attacker.hp -= defendDamage;

        if (defender.hp < 0)
            defender.hp = 0;
        if (attacker.hp < 0)
            attacker.hp = 0;

        //update status after 
        attacker.activity = Activity.Attack;
        attacker.target = defender.pos;

        defender.activity = Activity.Defence;
        defender.target = attacker.pos;
        
        //update powerbar
        UpdatePowerbar(CellType.Attacker, -defendDamage);
        UpdatePowerbar(CellType.Defender, -attachDamage);
    }

    int GetDamage(int attachNumber, int targetNumber)
    {
        var number = (3 + attachNumber - targetNumber) % 3;
        if (number == 0)
            return 4;
        else if (number == 1)
            return 5;
        else
            return 3;
    }
    
    void ShowConfetti(CellType result)
    {
        foreach (View view in dictViews[result])
        {
            view.AnimationVictory();
        }

        //zoom camera to show detail
        if (panAndZoom.cam.orthographicSize > 3)
        {
            DOTween.To(() => panAndZoom.cam.orthographicSize, f => { panAndZoom.cam.orthographicSize = f; }, CAMERA_WIN_SIZE, 0.5f);
        }

        //move camera to center
        var center = new Vector3(0, 0, panAndZoom.cam.transform.localPosition.z);
        panAndZoom.cam.transform.DOLocalMove(center, 0.5f);
    }

    CellType GetWinner()
    {
        if (powerAttacker <= 0)
            return CellType.Defender;
        if (powerDefender <= 0)
            return CellType.Attacker;
        return CellType.Empty;
    }

    #endregion

    
    #region MiniMap And Camera

    void CreateMinimap()
    {
        if (cameraMiniMap != null)
        {
            int h = 256;
            int w = (int) (Screen.width * h / (float) Screen.height);
            RenderTexture renderTexture = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32);
            cameraMiniMap.targetTexture = renderTexture;
            miniMapCamera.texture = renderTexture;
            
            miniMapCamera.rectTransform.sizeDelta = new Vector2(w, h);
        }
    }

    void UpdateCameraSize()
    {
        // calculate camera properties
        var hUnit = 2f * View.HEXAGON_RADIUS;
        var maxRadius = 2f * defenceRadius + 1f;
            
        var spaceVeritalUnit = 3f / 4f * hUnit;
        totalMapHeight = spaceVeritalUnit * maxRadius * 2f + hUnit;
        cameraMiniMap.orthographicSize = totalMapHeight / 2f;
        
        var boundY = totalMapHeight / 2f;
        panAndZoom.boundMaxY = boundY;
        panAndZoom.boundMinY = -boundY;

        var boundX = boundY / Screen.height * Screen.width;
        panAndZoom.boundMaxX = boundX;
        panAndZoom.boundMinX = -boundX;

        PanAndZoomOnChange();
    }
    
    void OnMiniMapDragged()
    {
        var totalWidth = totalMapHeight / Screen.height * Screen.width;
        
        var posX = miniMapRect.rectTransform.anchoredPosition.x * 2f;
        var posY = miniMapRect.rectTransform.anchoredPosition.y * 2f;

        var ratioX = posX / miniMapCamera.rectTransform.sizeDelta.x;
        var mainCamera_localPositionX = ratioX * (totalWidth / 2f);

        var ratioY = posY / miniMapCamera.rectTransform.sizeDelta.y;
        var mainCamera_localPositionY = ratioY * (totalMapHeight / 2f);
        
        mainCamera.transform.localPosition = new Vector3(mainCamera_localPositionX, mainCamera_localPositionY, mainCamera.transform.localPosition.z);
    }

    private void PanAndZoomOnChange()
    {
        var ratioY = mainCamera.transform.localPosition.y / (totalMapHeight / 2f);
        var posY = ratioY * miniMapCamera.rectTransform.sizeDelta.y;

        var totalWidth = totalMapHeight / Screen.height * Screen.width;
        
        var ratioX = mainCamera.transform.localPosition.x / (totalWidth / 2f);
        var posX = ratioX * miniMapCamera.rectTransform.sizeDelta.x;
        miniMapRect.rectTransform.anchoredPosition = new Vector2(posX, posY) / 2f;
        
        //update rect size
        var sizeY = 2f * mainCamera.orthographicSize / totalMapHeight * miniMapCamera.rectTransform.sizeDelta.y;
        var sizeX = sizeY / Screen.height * Screen.width;
        miniMapRect.rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
    }


    #endregion
    
    
    #region View

    void AddView(Data data)
    {
        View view = null;
        if (pools.Count > 0)
            view = pools.Pop();
        else
            view = Instantiate(viewTemplate, viewTemplate.transform.parent);

        view.transform.localScale = Vector3.one;
        view.gameObject.SetActive(true);
        view.Init(data);
        
        views[data.pos.x][data.pos.y][data.pos.z] = view;
        dictViews[data.type].Add(view);
    }
    
    void RemoveView(Data data, View view)
    {
        views[data.pos.x][data.pos.y][data.pos.z] = null;
        dictViews[data.type].Remove(view);
        
        view.gameObject.SetActive(false);
        pools.Push(view);
    }

    void UpdatePowerbar(CellType type, int addHp)
    {
        if (type == CellType.Attacker)
            powerAttacker += addHp;
        else if (type == CellType.Defender)
            powerDefender += addHp;
        
        var newValue = (float)powerDefender / (powerDefender + powerAttacker);
        //powerBarSlider.DOValue(newValue, 0.2f);
        powerBarSlider.value = newValue;
        textPowerDefender.text = powerDefender.ToString();
        textPowerAttacker.text = powerAttacker.ToString();
    }

    #endregion
    
    
    #region Data
    
    static readonly List<Int3> neighborsPositionDelta = new List<Int3>
    {
        new Int3(+1, -1, 0), new Int3(+1, 0, -1), new Int3(0, +1, -1),
        new Int3(-1, +1, 0), new Int3(-1, 0, +1), new Int3(0, -1, +1),
    };

    List<Int3> GetNeighborPositions(Int3 pos)
    {
        return neighborsPositionDelta.Select(x => x + pos).ToList();
    }

    int GetRadius(Int3 pos)
    {
        return Math.Max(Math.Max(Math.Abs(pos.x), Math.Abs(pos.y)), Math.Abs(pos.z));
    }

    int Distance(Int3 pos1, Int3 pos2)
    {
        return (Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y) + Math.Abs(pos1.z - pos2.z)) / 2;
    }

    View GetViewAt(int x, int y, int z)
    {
        return views[x][y][z];
    }

    Data GetDataAt(int x, int y, int z)
    {
        return datas[x][y][z];
    }

    void AddDefenders(int radius, Data data)
    {
        if (data.type == CellType.Defender)
        {
            if (dataByRadius.ContainsKey(radius))
            {
                dataByRadius[radius].Add(data);
            }
            else
            {
                dataByRadius.Add(radius, new List<Data>() {data});
            }
        }
    }

    void RemoveDefenders(int radius, Data data)
    {
        if (data.type == CellType.Defender)
        {
            if (dataByRadius.ContainsKey(radius))
            {
                dataByRadius[radius].Remove(data);
            }
        }
    }

    Data CreateData(int x, int y, int z, CellType type)
    {
        int hp = 0;
        if (type == CellType.Attacker)
            hp = Constants.HpAttacker;
        else if (type == CellType.Defender)
            hp = Constants.HpDefender;
        
        var data = new Data
        {
            pos = new Int3(x, y, z),
            type = type,
            hp = hp,
            activity = Activity.Idle,
            target = null
        };

        datas[x][y][z] = data;
        
        // add defenders
        int radius = GetRadius(data.pos);
        AddDefenders(radius, data);

        // update powerbar
        UpdatePowerbar(data.type, data.hp);
        
        // update counter
        if (data.type != CellType.Empty)
        {
            totalCharacter++;
            textCounter.text = totalCharacter + " characters";
        }

        return data;
    }

    Data RemoveData(int x, int y, int z)
    {
        var data = datas[x][y][z];
        if (data != null)
        {
            datas[x][y][z] = null;

            // remove defenders
            int radius = GetRadius(data.pos);
            RemoveDefenders(radius, data);

            // update powerbar
            UpdatePowerbar(data.type, -data.hp);

            // update counter
            if (data.type != CellType.Empty)
            {
                totalCharacter--;
                textCounter.text = totalCharacter + " characters";
            }
        }

        return data;
    }
    
    List<Data> CreateDataInRadius(int radius, CellType type)
    {
        List<Data> listAdd = new List<Data>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int z = 0 - x - y;
                if (Math.Abs(x) + Math.Abs(y) + Math.Abs(z) == 2 * radius)
                {
                    var data = CreateData(x, y, z, type);
                    listAdd.Add(data);
                }
            }
        }

        return listAdd;
    }

    List<Data> RemoveDataInRadius(int radius)
    {
        List<Data> listRemove = new List<Data>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int z = 0 - x - y;
                if (Math.Abs(x) + Math.Abs(y) + Math.Abs(z) == 2 * radius)
                {
                    var data = RemoveData(x, y, z);
                    if (data != null)
                    {
                        listRemove.Add(data);
                    }
                }
            }
        }

        return listRemove;
    }
    
    #endregion
    

}
