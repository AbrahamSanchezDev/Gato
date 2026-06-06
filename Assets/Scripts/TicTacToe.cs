using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_WEBGL
using UnityEngine.SceneManagement;
#endif

public class TicTacToe : MonoBehaviour
{
    [SerializeField] private GameObject buttonsGo;
    private Button[] buttons;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject restartPanel;

    private string currentPlayer;
    private int movesCount;
    private bool gameActive;
    private Color originalButtonColor;

    private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;

    private AudioClip originalAudioClip;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
            originalAudioClip = audioSource.clip;
    }

    void Start()
    {
        buttons = buttonsGo.GetComponentsInChildren<Button>();
        if (buttons != null && buttons.Length > 0)
        {
            originalButtonColor = buttons[0].image.color;
        }
        else
        {
            originalButtonColor = Color.white; // Default color if buttons are not assigned
            originalButtonColor.a = 0.5f; // Semi-transparent to indicate uninitialized state
        }
        StartGame();
    }

    void StartGame()
    {
        ResetButtonsSize();
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
            btn.image.color = originalButtonColor;
            btn.onClick.RemoveAllListeners();
            int capturedIndex = index;
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

    // Define the winning conditions
    private int[][] _winConditions = new int[][]
    {
        new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // Rows
        new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // Columns
        new int[] {0, 4, 8}, new int[] {2, 4, 6} // Diagonals
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
                PlayWinSound();
                statusText.text = "Jugador " + currentPlayer + " gana!";
                restartPanel.SetActive(true);
                return;
            }
        }

        // Check for draw
        if (movesCount == 9)
        {
            gameActive = false;
            statusText.text = "Empate!";
            restartPanel.SetActive(true);
        }
    }

    void HighlightWinningButtons(int[] indices)
    {
        foreach (int index in indices)
        {
            buttons[index].image.color = Color.green;
            StartCoroutine(AnimateButtonPulseCo(buttons[index]));
        }
    }

    #region Visual Effects
    private System.Collections.IEnumerator AnimateButtonPulseCo(Button button, bool continuePulsing = true)
    {
        float pulseDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 originalScale = button.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        while (continuePulsing)
        {
            // Pulsar hacia afuera
            while (elapsedTime < pulseDuration)
            {
                button.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Pulsar hacia adentro
            elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                button.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f; // Reiniciar el tiempo para el siguiente ciclo
        }

    }

    private void PlayWinSound()
    {
        if (audioSource && winSound)
        {
            audioSource.clip = winSound;
            audioSource.Play();
        }
    }
    private void PlayOriginalAudio()
    {
        if (audioSource && originalAudioClip)
        {
            audioSource.clip = originalAudioClip;
            audioSource.Play();
        }
    }
    #endregion

    private void ResetButtonsSize()
    {
        StopAllCoroutines();
        foreach (Button btn in buttons)
        {
            btn.transform.localScale = Vector3.one;
        }
        PlayOriginalAudio();
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
#if UNITY_WEBGL
        // Note: This is a workaround since WebGL does not support Application.Quit().
        // In WebGL, we can't quit the application, so we can reload the scene instead
        // Reloading the scene will reset the game state, effectively "quitting" the current game.
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        return;

#else
        Application.Quit();
#endif
    }
}