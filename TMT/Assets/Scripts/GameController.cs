using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public SceneManager sceneManager = null;

    public GameObject contents = null;

    public GameObject orderErrorPopup = null;

    public Text playerName = null;

    public Text playerLevel = null;

    public Text playerTime = null;

    public Material[] numberMaterials;

    public GameObject numberPrefabs = null;

    public GameObject cubePrefabs = null;

    public GameObject spherePrefabs = null;

    public GameObject tetrahedronPrefabs = null;

    private List<string> orderObject = new List<string>();

    private List<GameObject> objectList = new List<GameObject>();

    private List<GenerateGameType> gameList = new List<GenerateGameType>();

    private float angleTan = 0.0f;
    private float angleTan2 = 0.0f;

    private Animator anim;

    public int numOfParticles = 0;


    struct GenerateGameType
    {
        public GenerateGameType(Type type, string level, bool practice)
        {
            this.type = type;
            this.level = level;
            this.practice = practice;
        }

        public Type type;

        public string level;

        public bool practice;
    }

    enum Type
    {
        TYPE_A,
        TYPE_B,
        TYPE_C
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (numberMaterials == null)
        {
            Debug.Log("Number Material is not available");
        }

        SetGame(AndroidNative.Instance.GameType);

        AndroidNative.Instance.Println();   //   Android Native ID : " + userName + " Day: " + gameType.ToString())
    }

    public void TestDay1()
    {
        SetTestGame(1);
    }

    public void TestDay2()
    {
        SetTestGame(2);
    }

    public void TestDay3()
    {
        SetTestGame(3);
    }

    public void TestDay4()
    {
        SetTestGame(4);
    }

    public void TestDay5()
    {
        SetTestGame(5);
    }

    public void TestDay6()
    {
        SetTestGame(6);
    }

    public void TestDay7()
    {
        SetTestGame(7);
    }

    private void SetTestGame(int gameType)
    {
        // destroy
        foreach (Transform child in contents.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        gameList.Clear();
        orderObject.Clear();
        objectList.Clear();

        SetGame(gameType);
    }

    public void SetGame(int gameType)
    {
        if (gameType == 1) // 1st Practice A and Practice B and Practice C
        {
            Debug.Log("Set Game Day 1");
            gameList.Add(new GenerateGameType(Type.TYPE_A, "1-A 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_B, "1-B 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_C, "1-C 연습", true));
        }
        else if (gameType == 2) // 2st Practice A and Main A
        {
            Debug.Log("Set Game Day 2");
            gameList.Add(new GenerateGameType(Type.TYPE_A, "2-A 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_A, "2-A1", false));
        }
        else if (gameType == 3) // 3st Practice B and Main B
        {
            Debug.Log("Set Game Day 3");
            gameList.Add(new GenerateGameType(Type.TYPE_B, "3-B 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_B, "3-B1", false));
        }
        else if (gameType == 4) // 4st Practice C and Main C
        {
            Debug.Log("Set Game Day 4");
            gameList.Add(new GenerateGameType(Type.TYPE_C, "4-C 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_C, "4-C1", false));
        }
        else if (gameType == 5) // 5st Practice A and Main A
        {
            Debug.Log("Set Game Day 5");
            gameList.Add(new GenerateGameType(Type.TYPE_A, "5-A 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_A, "5-A2", false));
        }
        else if (gameType == 6) // 6st Practice B and Main B
        {
            Debug.Log("Set Game Day 6");
            gameList.Add(new GenerateGameType(Type.TYPE_B, "6-B 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_B, "6-B2", false));
        }
        else if (gameType == 7) // 7st Practice C and Main C
        {
            Debug.Log("Set Game Day 7");
            gameList.Add(new GenerateGameType(Type.TYPE_C, "7-C 연습", true));
            gameList.Add(new GenerateGameType(Type.TYPE_C, "7-C2", false));
        }
        else
        {
            Debug.Log("Game is not available");
        }
    }

    void Start()
    {
        // NextStep();       
        anim = GetComponent<Animator>();
    }

    public bool NextStep()
    {
        Debug.Log("NextStep");

        if (gameList.Count != 0)
        {  // 남은 게임이 있다면, 게임을 만든다
            Generate(gameList[0].type, gameList[0].practice);   //  여기서 진짜 오브젝트가 생성하게됨
            playerName.text = AndroidNative.Instance.UserName + " 님";
            playerLevel.text = gameList[0].level;
            gameList.RemoveAt(0);   //  게임을 만들고, 남은 게임 리스트에서 지운다

            return true;
        }
        else
        {    //남은 게임이 없는 경우
            return false;
        }
    }

    public bool NextIsReady()
    {
        return orderObject.Count == 0 ? true : false;   //  남은 도형이 없는가?
    }

    public void ChangeObjectPosition(float angle)
    {
        angleTan = Mathf.Tan(3.14f / 2.0f - 3.14f / 180.0f * angle / 2.0f);
        angleTan2 = angle / 2.0f * 3.14f / 180.0f ;
        /* 
         *  float range = 5.0f;
            int gap = 2;
            float maxDistance = range * range + range * range + (range + gap) * (range + gap);
            for (int i = 0; i < objectList.Count; i++)
            {
                float x = Random.Range(-range, range);
                float y = Random.Range(-range, range);
                float z = Random.Range(Mathf.Abs(angleTan * x), range);
                if (z < gap) z += gap;
                float distance = x * x + y * y + z * z;
                float localScaleValue = distance / maxDistance;
                objectList[i].transform.localPosition = new Vector3(x, y, z);
                objectList[i].transform.localScale = new Vector3(localScaleValue, localScaleValue, localScaleValue);
                objectList[i].transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                objectList[i].transform.parent = contents.transform;
            }
        */
    }

    private Vector3 generatePosition(float range, float gap)
    {
        float x = 0.0f, y = 0.0f, z = 0.0f, r = 0.0f;

        bool flag = true;
        while(flag)
        {
            x = Random.Range(-range, range);
            //x = Random.Range(gap, range) * Mathf.Sign(Random.Range(-1.0f, 1.0f));
            r = Random.Range(gap, range);
            z = Mathf.Sqrt(Mathf.Abs(r * r - x * x));

            float v1 = Mathf.Abs(Mathf.Tan(angleTan2));
            float v2 = Mathf.Abs(x / z);
            if(v1 > v2)
            //if (Mathf.Abs(Mathf.Tan(angleTan2)) >  Mathf.Abs(x / z) )
            {
                Debug.Log("2020::03::01::" + v1 + ", " + v2);
                flag = false;
            }
        }

        y = Random.Range(-range, range);
        
        return new Vector3(x,y,z);
    }

    private void Generate(Type type, bool practice)
    { // type A, B, C
        int max = practice ? 8 : 15;
        float range = 7.0f; //float range = practice ? 3.0f : 5.0f;
        float gap = 5.0f;
        float maxDistance = 3 * range * range;  //range * range + range * range + (range + gap) * (range + gap);

        if (type == Type.TYPE_A)
        { // A: only number
            for (int i = 0; i < max; i++)
            {
                GameObject obj = Instantiate(numberPrefabs);

                obj.GetComponent<Renderer>().material = numberMaterials[i];

                Vector3 po = generatePosition(range, gap);
                float x = po.x;
                float y = po.y;
                float z = po.z;
                //float x = Random.Range(-range, range);
                //float y = Random.Range(-range, range);
                //float z = Random.Range(Mathf.Abs(angleTan * x), range);
                //if (z < gap) z += gap;

                //float x = Random.Range(-range, range);
                //float y = Random.Range(-range, range);
                //float z = Random.Range(Mathf.Abs(angleTan * x), range);
                //if (z < gap) z += gap;
                float distance = x * x + y * y + z * z;
                float localScaleValue = distance / maxDistance;

                obj.transform.localPosition = new Vector3(x, y, z);
                obj.transform.localScale = new Vector3(localScaleValue, localScaleValue, localScaleValue);

                obj.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                obj.name = string.Format("{0:00}", i);
                obj.GetComponent<Renderer>().material.color
                    = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                if (i == 0)
                {
                    obj.GetComponent<Renderer>().material.shader = Shader.Find("Custom/OutlineShader"); // 이 쉐이더는 빨간색 테두리를 추가함
                    obj.name = "first";
                }

                obj.transform.parent = contents.transform;
                orderObject.Add(obj.name);

                objectList.Add(obj);
            }
        }
        else if (type == Type.TYPE_B)
        { // B: shpere and cude
            for (int i = 0; i < max; i++)
            {
                int res = i % 2;
                GameObject obj = null;
                if (res == 0)
                {
                    obj = Instantiate(tetrahedronPrefabs);
                    //obj = Instantiate(spherePrefabs);
                }
                else
                {
                    obj = Instantiate(cubePrefabs);
                }

                Vector3 po = generatePosition(range, gap);
                float x = po.x;
                float y = po.y;
                float z = po.z;
                //float x = Random.Range(-range, range);
                //float y = Random.Range(-range, range);
                //float z = Random.Range(Mathf.Abs(angleTan * x), range);
                //if (z < gap) z += gap;
                float distance = x * x + y * y + z * z;
                float localScaleValue = distance / maxDistance;

                obj.transform.localPosition = new Vector3(x, y, z);
                obj.transform.localScale = new Vector3(localScaleValue, localScaleValue, localScaleValue);

                obj.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
                obj.name = string.Format("{0:000}", res);

                obj.GetComponent<Renderer>().material.color
                    = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                if (i == 0)
                {
                    obj.GetComponent<Renderer>().material.shader = Shader.Find("Custom/OutlineShader");
                    obj.name = "first";
                }

                obj.transform.parent = contents.transform;
                orderObject.Add(obj.name);

                objectList.Add(obj);
            }
        }
        else if (type == Type.TYPE_C)
        { // C: number and sphere and number and cube
            int matNum = 0;
            for (int i = 0; i < max; i++)
            {
                GameObject obj = null;
                int res = i % 3;
                if (res == 0)
                {
                    obj = Instantiate(numberPrefabs);
                    obj.GetComponent<Renderer>().material = numberMaterials[matNum];
                    obj.name = string.Format("{0:00}", matNum);
                    matNum++;
                }
                else if (res == 1)
                {
                    obj = Instantiate(tetrahedronPrefabs); 
                    //obj = Instantiate(cubePrefabs);
                    obj.name = string.Format("{0:000}", res);
                }
                else
                {
                    obj = Instantiate(spherePrefabs);
                    obj.name = string.Format("{0:000}", res);
                }

                Vector3 po = generatePosition(range, gap);
                float x = po.x;
                float y = po.y;
                float z = po.z;
                //float x = Random.Range(-range, range);
                //float y = Random.Range(-range, range);
                //float z = Random.Range(Mathf.Abs(angleTan * x), range);
                //if (z < gap) z += gap;

                // 원래쓰던 것
                //float x = Random.Range(-range, range);
                //float y = Random.Range(-range, range);
                //float z = Random.Range(Mathf.Abs(angleTan * x), range);
                //if (z < gap) z += gap;
                float distance = x * x + y * y + z * z;
                float localScaleValue = distance / maxDistance;

                obj.transform.localPosition = new Vector3(x, y, z);
                obj.transform.localScale = new Vector3(localScaleValue, localScaleValue, localScaleValue);

                obj.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);

                obj.GetComponent<Renderer>().material.color
                    = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));


                if (i == 0)
                {
                    obj.GetComponent<Renderer>().material.shader = Shader.Find("Custom/OutlineShader");
                    obj.name = "first";
                }

                obj.transform.parent = contents.transform;
                orderObject.Add(obj.name);

                objectList.Add(obj);
            }
        }
                
        sceneManager.SetBaseUI2(max);
    }

    public void updateNumOfParticles(int num)
    {
        numOfParticles = num;
    }

    public float time = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }

        time += Time.deltaTime;
        playerTime.text = string.Format("{0:000.00}", time);
        

        if (Input.GetMouseButtonDown(0))
        {
            //  GetMouseButtonDown(0) == 좌클릭
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                string index0 = orderObject[0];
                if (hit.transform.gameObject.name.Equals(index0))
                {
                    // Debug.Log("Hit " + index0);
                    orderObject.RemoveAt(0);

                    //DestroyImmediate(hit.transform.gameObject);
                    //objectList.RemoveAt(0);
                    Debug.Log("20200218:: hit");
                    int j;

                    for (j = 0; j < objectList.Count; j++)
                    {
                        //Debug.Log("20200218:: FOr" + j);
                        if(objectList[j] == null)
                        {
                            Debug.Log("20200218:: null for" + j);
                        }
                        if (objectList[j].Equals(hit.transform.gameObject))
                        {
                            Debug.Log("20200218:: Hit Equals , " + j);

                            objectList.RemoveAt(j);
                            break;
                        }
                    }

                    DestroyImmediate(hit.transform.gameObject);
                    
                    sceneManager.SetBaseUI2(objectList.Count);
                }
                else
                {
                    orderErrorPopup.SetActive(true);
                }
            }
        }

        if ((sceneManager.gameover == false) && (sceneManager.isInGame == true))
        {
            checkHit();
            checkGuide();
            //checkARPoint();
        }

    }

    private bool AP_setVisible = false; //  AP = AR Point
    private bool AP_isDecting = false;
    private float AP_startTime = 0.0f;
    public void checkARPoint()
    {
        float T_LIMIT = 5.0f;
        int N_LIMIT = 50;

        if(AP_setVisible == false)
        {
            if(AP_isDecting == true)
            {
                if(T_LIMIT < (time - AP_startTime))
                {
                    AP_setVisible = true;
                    sceneManager.SetWarningPopup2(AP_setVisible);
                    //
                    AP_isDecting = false;
                    AP_startTime = time;
                }

                if(N_LIMIT < numOfParticles)
                {
                    AP_setVisible = false;
                    sceneManager.SetWarningPopup2(AP_setVisible);

                    AP_isDecting = false;
                }
            }
            else
            {
                if(N_LIMIT > numOfParticles)
                {
                    AP_isDecting = true;
                    AP_startTime = time;
                }
            }
        }
        else
        {
            // setVisible가 true일 때
            if(N_LIMIT < numOfParticles)
            {
                AP_setVisible = false;
                sceneManager.SetWarningPopup2(AP_setVisible);
            }
        }
        /*
         * 
         * If setVisible = false
         *      If isDecting
         *
         *           AddTimeCount() //  경과시간을 체크함
         *           If T_LIMIT < time
         *               setVisible = true
         *               SceneManager.warningUI.SetVisible(true)
         *               stopCounting()  //  isDecting = false, time = 0
         *           
         *           If Num > N_LIMIT
         *               setVisible = false
         *               stopCounting()  //  isDecting = false, time = 0
         *           
         *           
         *      Else
         *       If Num < N_LIMIT
         *           StartCounting()
         *           isDecting = true
         *
         *Else  //  setVisible = true
         *      If Num > N_LIMIT
         *          setVisible = false
         * 
         */
    }

    private bool isHit = false;
    private GameObject hitObject = null;
    private void checkHit()
    {
        RaycastHit hit;
        var screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "testGuide")
            {
                return;
            }

            if (isHit)
            {
                if (hitObject != null)
                {
                    if (!hit.transform.gameObject.Equals(hitObject))
                    {
                        Debug.Log("AnimationTest:: second hit");
                        //temp원래대로
                        hitObject.GetComponent<Animator>().SetBool("isHited", false);
                        hitObject = hit.transform.gameObject;
                        //temp크기를키운다
                        hit.transform.GetComponent<Animator>().SetBool("isHited", true);
                    }
                }
            }
            else
            {
                Debug.Log("AnimationTest:: first hit");
                hitObject = hit.transform.gameObject;
                isHit = true;
                //temp크기를키운다
                hit.transform.GetComponent<Animator>().SetBool("isHited", true);
            }
        }
        else
        {
            if (isHit == true)
            {
                //temp원래대로
                try
                {
                    if (hitObject != null) hitObject.GetComponent<Animator>().SetBool("isHited", false);
                    hitObject = null;
                    isHit = false;
                    Debug.Log("AnimationTest:: SetBool false");
                }
                catch (System.Exception e)
                {
                    Debug.Log("AnimationTest:: try-catch error");
                }
            }
        }
    }

    public bool left = false, right = false, top = false, bottom = false;
    private void checkGuide()
    {
        left = right = top = bottom = false;
        Vector3 camVec = Camera.main.transform.forward;

        if (objectList == null)
        {
            return;
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            if ((left && right && top && bottom) == true)
            {
                break;
            }
            if (objectList[i] == null)
            {
                Debug.Log("20200218:: 널?");
                break;
            }
            Vector3 objVec = objectList[i].transform.localPosition;
            objVec.Normalize();
            Vector3 pos = Camera.main.WorldToViewportPoint(objectList[i].transform.position);

            if (pos.x < 0f || pos.x > 1f)
            {
                //  물체가 x범위에 없다
                //  없으면, 각도 계산을 한다. 그리고 그 방향을 표시한다.
                float diff = objVec.x - camVec.x;
                if (diff > 0) right = true;
                else left = true;
            }

            if (pos.y < 0f || pos.y > 1f)
            {
                //  물체가 y범위에 없다
                //  없으면, 각도 계산을 한다. 그리고 그 방향을 표시한다.
                float diff = objVec.y - camVec.y;
                if (diff > 0) top = true;
                else bottom = true;
            }
        }
        
        //Debug.Log("20200217:: lrtb = " + left + ", " + right + ", " + top + ", " + bottom );

        sceneManager.updateBaseGuide(left, right, top, bottom);
    }
    
    public void OrderErrorPopup()
    {
        orderErrorPopup.SetActive(false);
    }
}
