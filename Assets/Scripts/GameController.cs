using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameController : MonoBehaviour
{
    public List<GuessBundleScriptableObject> bundles; // Bundles with different packs of values (e.g. letters, digits)
    public static event Action<string> onTaskChange;

    private enum GAME_TYPE { LETTERS, DIGITS };
    private enum DIFFICULTY { EASY = 1, MEDIUM, HARD } // Difficulty shows number of rows

    [SerializeField] private GameObject cell;
    [SerializeField] private GAME_TYPE gameType;
    [SerializeField] private DIFFICULTY diffuculty;

    private Transform gamePanel;
    private GameObject restartPanel;
    private Image loadingScreen;
    private List<string> wasAsked = new List<string>();
    private string currentTask;

    void Start()
    {
        CellTapScript.onCellTapped += onCellTapped;
        RestartBtnScript.onRestart += onRestart;
        gamePanel = GameObject.FindGameObjectWithTag("Panel").transform;
        restartPanel = GameObject.FindGameObjectWithTag("Restart");
        restartPanel.GetComponentInChildren<RestartBtnScript>().enabled = false;
        loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<Image>();
        SpawnCells(diffuculty);
    }

    public void TaskChanged(string task)
    {
        onTaskChange?.Invoke(task);
    }

    void SpawnCellsRow(int rowIndex, List<string> symbPool, GuessBundleScriptableObject bundle, bool anim = false)
    {
        float scaleFactor = gamePanel.GetComponentInParent<Canvas>().scaleFactor; // UI Scale Factor
        float panelSize = gamePanel.gameObject.GetComponent<RectTransform>().rect.width * scaleFactor;

        if (scaleFactor > 1) scaleFactor = 1;
        float cellSize = cell.GetComponent<RectTransform>().rect.width * scaleFactor;
        
        float margin = (panelSize - 3*cellSize) / 3;
        for (int c = -1; c <= 1; c++)  // c = column value
        {
            Vector3 shiftValue = new Vector3(c * (margin + cellSize), rowIndex * (margin + cellSize));
            GameObject cellGO = Instantiate(cell, gamePanel.position + shiftValue, Quaternion.identity, gamePanel);
            Transform cellTransform = cellGO.transform;
            // Choosing a symbol from pool
            int symbolIndex = UnityEngine.Random.Range(0, symbPool.Count);
            string symbol = symbPool[symbolIndex];
            symbPool.RemoveAt(symbolIndex);

            cellTransform.GetChild(0).GetComponent<Image>().sprite = bundle.cellSprites[symbol];
            cellTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            cellGO.GetComponent<CellTapScript>().symbol = symbol;
            
            if (anim)
            {
                cellTransform.localScale = Vector3.zero;
                cellTransform.DOScale(scaleFactor, 1).SetEase(Ease.OutBounce);
            }
        }
    }


    void SpawnCells(DIFFICULTY diff, bool anim = true)
    {
        int rowsToSpawn = (int)diff;
        GuessBundleScriptableObject bundle = bundles[UnityEngine.Random.Range(0, bundles.Count)];
        List<string> symbolsPool = bundle.getSample((int)diff * 3);
        // Choosing Task
        List<string> potentialTasks = new List<string>(symbolsPool.Where(x => !wasAsked.Contains(x)));
        currentTask = potentialTasks[UnityEngine.Random.Range(0, potentialTasks.Count)];
        TaskChanged(currentTask);

        // Spawn Cells
        for (int i = 0, r = 1 ; i < rowsToSpawn; i++, r--) // r = row index
        {
            SpawnCellsRow(r, symbolsPool, bundle, anim);
        }
        
        
    }

    void NextLevel()
    {
        if (diffuculty == DIFFICULTY.HARD) // If You passed Hard then You completed the game; 
        { 
            ShowRestart(); 
        }
        else
        {
            wasAsked.Add(currentTask);
            diffuculty += 1;
            DeleteCells();
            SpawnCells(diffuculty, false);
        }
    }
    void ShowRestart()
    {
        foreach (CellTapScript script in FindObjectsOfType<CellTapScript>())
        {
            script.enabled = false;
        }
        restartPanel.transform.parent.GetComponent<Image>().DOFade(0.8f, 0.4f).OnComplete(() => restartPanel.GetComponentInChildren<RestartBtnScript>().enabled = true);
        restartPanel.transform.DOScaleX(1, 0.5f);
    }

    void onRestart()
    {
        restartPanel.transform.parent.GetComponent<Image>().DOFade(0f, 0f);
        restartPanel.transform.DOScaleX(0, 0);
        restartPanel.GetComponentInChildren<RestartBtnScript>().enabled = false;
        // Show "loading"
        loadingScreen.DOFade(1, 1f).OnComplete(() =>
        {
            // Setting starting settings
            diffuculty = DIFFICULTY.EASY;
            DeleteCells();
            wasAsked.Clear();
            SpawnCells(diffuculty);
            loadingScreen.DOFade(0, 1f);
        });
    }
    void DeleteCells()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in gamePanel) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }
    
    public void onCellTapped(string symbol, GameObject cell)
    {
        Transform childTransform = cell.transform.GetChild(0).transform;
        bool rightAnswer = currentTask.Equals(symbol);

        if (rightAnswer)
        {
            // Make Bounce animation and go to next level
            childTransform.DOScale(0.6f, 1f).SetEase(Ease.OutBounce).OnComplete(NextLevel); 
        } 
        else
        {
            Vector3 currPos = childTransform.localPosition;
            childTransform.DOShakePosition(1, new Vector3(6, 0, 0));
            
        }
    }

}
