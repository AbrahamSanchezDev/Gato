using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToe : MonoBehaviour
{
    [SerializeField] private GameObject buttonsGo;
    private Button[] buttons;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject restartPanel;

    private string currentPlayer;
    private int movesCount;
    private bool gameActive;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        currentPlayer = "X";
        movesCount = 0;
        gameActive = true;
        restartPanel.SetActive(false);
        buttons = buttonsGo.GetComponentsInChildren<Button>();
        var index = 0;

        foreach (Button btn in buttons)
        {
            btn.GetComponentInChildren<TMP_Text>().text = "";
            btn.interactable = true;
            btn.image.color = Color.white;
            btn.onClick.RemoveAllListeners();
            int capturedIndex = index; // Capturar el índice para el cierre
            btn.onClick.AddListener(() => OnButtonClick(capturedIndex));
            index++;
        }

        UpdateStatusText();
    }

    public void OnButtonClick(int index)
    {
        if (!gameActive) return;

        buttons[index].GetComponentInChildren<TMP_Text>().text = currentPlayer;
        buttons[index].interactable = false;

        movesCount++;
        CheckWinner();

        if (gameActive)
        {
            currentPlayer = (currentPlayer == "X") ? "O" : "X";
            UpdateStatusText();
        }
    }

    // Combinaciones ganadoras
    private int[][] _winConditions = new int[][]
    {
        new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // Filas
        new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // Columnas
        new int[] {0, 4, 8}, new int[] {2, 4, 6} // Diagonales
    };

    private int[][] winConditions
    {
        get { return _winConditions; }
    }

    void CheckWinner()
    {
        // Verificar combinaciones
        foreach (int[] condition in winConditions)
        {
            if (buttons[condition[0]].GetComponentInChildren<TMP_Text>().text == currentPlayer &&
                buttons[condition[1]].GetComponentInChildren<TMP_Text>().text == currentPlayer &&
                buttons[condition[2]].GetComponentInChildren<TMP_Text>().text == currentPlayer)
            {
                gameActive = false;
                HighlightWinningButtons(condition);
                statusText.text = "¡Jugador " + currentPlayer + " gana!";
                restartPanel.SetActive(true);
                return;
            }
        }

        // Verificar empate
        if (movesCount == 9)
        {
            gameActive = false;
            statusText.text = "¡Empate!";
            restartPanel.SetActive(true);
        }
    }

    void HighlightWinningButtons(int[] indices)
    {
        foreach (int index in indices)
        {
            buttons[index].image.color = Color.green;
        }
    }

    void UpdateStatusText()
    {
        statusText.text = "Turno del jugador: " + currentPlayer;
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}