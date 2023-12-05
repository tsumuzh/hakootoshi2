using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject prefab_StageSelectBlock, prefab_Ball, prefab_RetryBlock, prefab_NextBlock, prefab_StartStageSelectBlock;
    [SerializeField] GameObject prefab_NormalBlock, prefab_LightBlock, prefab_HeavyBlock, prefab_WallBlock, prefab_UpGravityBlock, prefab_LeftGravityBlock, prefab_RightGravityBlock, prefab_DownGravityBlock, prefab_BigBlock, prefab_SlowBlock, prefab_TweetBlock;
    [SerializeField] GameObject mainCamera, ball_scene, obj_PlayGuide;
    [SerializeField] TextMeshPro stageLabel, clearLabel;
    public bool isPhaseTransitioning;
    public int phase; //0:default, 1:stage select, 2:in game, 3:failed, 4:clear
    public int blockCount, blockCountInFalling, currentStageNumber;
    private Vector3 ballPos;
    private Coroutine coroutine_PlayGuide;
    const int MAX_STAGE_NUM = 12;
    void Start()
    {
        InitializeField();
        CheckSaveData();
        isPhaseTransitioning = false;
        stageLabel.text = "";
        coroutine_PlayGuide = StartCoroutine(PlayGuide());
    }
    void Update()
    {
        if (!isPhaseTransitioning)
        {
            switch (phase)
            {
                case 0:
                    if (Mathf.Abs(ball_scene.transform.position.x) > 14 || Mathf.Abs(ball_scene.transform.position.y) > 5.5f)
                    {
                        Destroy(ball_scene);
                        ball_scene = Instantiate(prefab_Ball, new Vector3(0, -2, 0), Quaternion.Euler(90, 0, 0));
                    }
                    break;

                case 1:
                    if (Mathf.Abs(ball_scene.transform.position.x) > 14 || ball_scene.transform.position.y < 4.5f || ball_scene.transform.position.y > 15.5f)
                    {
                        Destroy(ball_scene);
                        ball_scene = Instantiate(prefab_Ball, new Vector3(0, 10, 0), Quaternion.Euler(90, 0, 0));
                    }
                    break;

                case 2:
                    if (blockCountInFalling == 0 && blockCount > 0)
                    {
                        if ((Physics.gravity.y < 0 && (Mathf.Abs(ball_scene.transform.position.x) > 14 || ball_scene.transform.position.y < -7))
                        || (Physics.gravity.y > 0 && (Mathf.Abs(ball_scene.transform.position.x) > 14 || ball_scene.transform.position.y > 7))
                        || (Physics.gravity.x < 0 && (ball_scene.transform.position.x < -14 || Mathf.Abs(ball_scene.transform.position.y) > 7))
                        || (Physics.gravity.x > 0 && (ball_scene.transform.position.x > 14 || Mathf.Abs(ball_scene.transform.position.y) > 7)))
                        {
                            StartCoroutine(MoveCameraAndTransitionPhase(new Vector3(0, 0, -10), new Vector3(-25, 0, -10), 3, 1));
                            ball_scene.GetComponent<Collider>().isTrigger = true;
                            Instantiate(prefab_RetryBlock, new Vector3(-29, -1, 0), Quaternion.identity);
                            Instantiate(prefab_StartStageSelectBlock, new Vector3(-21, -1, 0), Quaternion.identity);
                        }
                    }
                    break;

                case 3:
                    if (Mathf.Abs(ball_scene.transform.position.x + 25) > 14 || Mathf.Abs(ball_scene.transform.position.y) > 5.5f)
                    {
                        Destroy(ball_scene);
                        ball_scene = Instantiate(prefab_Ball, new Vector3(-25, 0, 0), Quaternion.Euler(90, 0, 0));
                    }
                    break;

                case 4:
                    if (Mathf.Abs(ball_scene.transform.position.x - 25) > 14 || Mathf.Abs(ball_scene.transform.position.y) > 5.5f)
                    {
                        Destroy(ball_scene);
                        ball_scene = Instantiate(prefab_Ball, new Vector3(25, 0, 0), Quaternion.Euler(90, 0, 0));
                    }
                    break;
            }
        }
    }
    private void InitializeField()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject obj in objs) Destroy(obj);
        stageLabel.text = "";
        blockCount = 0;
    }
    public void DecrementAndCheckBlockCount()
    {
        blockCount--;
        blockCountInFalling--;
        if (blockCount == 0)
        {
            phase = 4;
            StartCoroutine(MoveCameraAndTransitionPhase(new Vector3(0, 0, -10), new Vector3(25, 0, -10), 4, 1));
            /* Destroy(ball_scene);
             ball_scene = Instantiate(prefab_Ball, new Vector3(25, 0, 0), Quaternion.Euler(90, 0, 0));*/
            ball_scene.GetComponent<Collider>().isTrigger = true;
            if (currentStageNumber != MAX_STAGE_NUM)
            {
                clearLabel.text = "成功";
                Instantiate(prefab_NextBlock, new Vector3(21, -1, 0), Quaternion.identity);
            }
            else
            {
                clearLabel.text = "勝利";
                Instantiate(prefab_TweetBlock, new Vector3(21, -1, 0), Quaternion.identity);
            }
            Instantiate(prefab_StartStageSelectBlock, new Vector3(29, -1, 0), Quaternion.identity);
            if (PlayerPrefs.GetInt("MaxReachedStage") == currentStageNumber) PlayerPrefs.SetInt("MaxReachedStage", currentStageNumber + 1);
        }
    }
    private void CheckSaveData()
    {
        if (!PlayerPrefs.HasKey("MaxReachedStage")) InitializeSaveData();
    }
    private void InitializeSaveData()
    {
        PlayerPrefs.SetInt("MaxReachedStage", 1);
    }
    private void GenerateStageSelectBlock()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("StageSelectBlock");
        foreach (GameObject obj in objs) Destroy(obj);
        int maxReachedStage = PlayerPrefs.GetInt("MaxReachedStage");
        for (int i = 0; i < MAX_STAGE_NUM; i++)
        {
            GameObject obj = Instantiate(prefab_StageSelectBlock, new Vector3(3 * Mathf.Sin(i * Mathf.PI * 2 / MAX_STAGE_NUM), 10 + 3 * Mathf.Cos(i * Mathf.PI * 2 / MAX_STAGE_NUM), 0), Quaternion.identity);
            obj.transform.GetChild(0).GetComponent<TextMeshPro>().text = (i + 1).ToString();
            obj.GetComponent<StageSelectBlockManager>().stageNumber = i + 1;
            if (i >= maxReachedStage)
            {
                obj.GetComponent<Collider>().isTrigger = true;
                obj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
    public void OnStartStageSelectBlockFallen()
    {
        stageLabel.text = "";
        GenerateStageSelectBlock();
        StartCoroutine(MoveCameraAndTransitionPhase(mainCamera.transform.position, new Vector3(0, 10, -10), 1, 0));
    }
    private IEnumerator MoveCameraAndTransitionPhase(Vector3 startPos, Vector3 endPos, int targetPhase, float waitTimeBeforeTransition)
    {
        isPhaseTransitioning = true;
        yield return new WaitForSeconds(waitTimeBeforeTransition);
        float t = 0;
        while (t < 1)
        {
            t += Time.fixedDeltaTime;
            mainCamera.transform.position = CubicEase(startPos, endPos, t);
            yield return new WaitForFixedUpdate();
        }
        phase = targetPhase;
        isPhaseTransitioning = false;
        if (phase == 2)
        {
            Destroy(ball_scene);
            ball_scene = Instantiate(prefab_Ball, ballPos, Quaternion.Euler(90, 0, 0));
            GameObject[] objs = GameObject.FindGameObjectsWithTag("StageSelectBlock");
            foreach (GameObject obj in objs) Destroy(obj);
        }
        else
        {
            if (phase == 1)
            {
                StopCoroutine(coroutine_PlayGuide);
                obj_PlayGuide.SetActive(false);
            }
            InitializeField();
            Destroy(ball_scene);
            ball_scene = Instantiate(prefab_Ball, endPos + new Vector3(0, -10, 0), Quaternion.Euler(90, 0, 0));
        }
        yield break;
    }
    public void StartGame(int stageNumber)
    {
        InitializeField();
        ball_scene.GetComponent<Collider>().isTrigger = true;
        currentStageNumber = stageNumber;
        stageLabel.text = "ステージ" + stageNumber.ToString();
        string stageData = Resources.Load<TextAsset>("StageDatas/" + stageNumber.ToString()).text;
        blockCountInFalling = 0;
        int index = 0;
        string posStr = "";
        ballPos = new Vector3(float.MaxValue, float.MaxValue, 0);
        Vector3 blockPos = Vector3.zero;
        while (index < stageData.Length)
        {
            if (stageData[index] == ',')
            {
                if (ballPos.x == float.MaxValue) ballPos.x = float.Parse(posStr);
                else blockPos.x = float.Parse(posStr);
                index++;
                posStr = "";
            }

            switch (stageData[index])
            {
                case ':':
                    if (ballPos.y == float.MaxValue)
                    {
                        ballPos.y = float.Parse(posStr);
                    }
                    else
                    {
                        blockPos.y = float.Parse(posStr);
                        Instantiate(prefab_NormalBlock, blockPos, Quaternion.identity);
                        blockCount++;
                    }
                    index++;
                    posStr = "";
                    break;

                case 'L':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_LightBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'H':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_HeavyBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'W':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_WallBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'U':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_UpGravityBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'E':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_LeftGravityBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'R':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_RightGravityBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'D':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_DownGravityBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'B':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_BigBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                case 'S':
                    blockPos.y = float.Parse(posStr);
                    Instantiate(prefab_SlowBlock, blockPos, Quaternion.identity);
                    blockCount++;
                    index++;
                    posStr = "";
                    break;

                default:
                    break;
            }

            posStr += stageData[index];
            index++;
        }
        StartCoroutine(MoveCameraAndTransitionPhase(mainCamera.transform.position, new Vector3(0, 0, -10), 2, 0));
    }

    private IEnumerator PlayGuide()
    {
        Sprite sprire_mouse = Resources.Load<Sprite>("Textures/mouse");
        Sprite sprire_leftclick = Resources.Load<Sprite>("Textures/leftclick");
        SpriteRenderer spriteRenderer_PlayGuide = obj_PlayGuide.GetComponent<SpriteRenderer>();
        Color transparent = new Color(1, 1, 1, 0);
        float t = 0;
        while (true)
        {
            spriteRenderer_PlayGuide.sprite = sprire_mouse;
            obj_PlayGuide.transform.position = new Vector3(3, 0, 0);
            t = 0;
            yield return new WaitForSeconds(2);
            while (t < 1)
            {
                t += Time.fixedDeltaTime * 2;
                spriteRenderer_PlayGuide.color = Color.Lerp(transparent, Color.white, t);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(1);
            spriteRenderer_PlayGuide.sprite = sprire_leftclick;
            yield return new WaitForSeconds(1);
            t = 0;
            while (t < 1)
            {
                t += Time.fixedDeltaTime;
                obj_PlayGuide.transform.position = CubicEase(new Vector3(3, 0, 0), new Vector3(3, -3, 0), t);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(1);
            spriteRenderer_PlayGuide.sprite = sprire_mouse;
            yield return new WaitForSeconds(1);
            t = 0;
            while (t < 1)
            {
                t += Time.fixedDeltaTime * 2;
                spriteRenderer_PlayGuide.color = Color.Lerp(Color.white, transparent, t);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(2);
        }
    }

    private Vector3 CubicEase(Vector3 start, Vector3 end, float t)
    {
        if (t <= 0) return start;
        else if (t >= 1) return end;
        else return (end - start) * ((3 * t * t) - (2 * t * t * t)) + start;
    }
}