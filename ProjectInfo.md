id: gato
title: Gato
subtitle: Unity Tic-Tac-Toe with responsive UI and win animations
techBadge: Unity 2D
gifUrl: /games/gato.gif
repoUrl: [https://github.com/AbrahamSanchezDev/Gato](https://github.com/AbrahamSanchezDev/Gato)
demoUrl: "https://abrahamsanchezdev.github.io/Gato/"
challenge: "Implemented responsive turn-based logic, win detection, and animated UI feedback in Unity while keeping game state clean and restartable."
architecture: "Event-driven MonoBehaviour flow with UI button listeners and coroutine-based animation."
techStack:
- Unity
- C#
- Universal Render Pipeline
- TextMeshPro
- AudioSource
codeSnippetTitle: "Turn handling and win detection"
codeSnippet: |
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
---