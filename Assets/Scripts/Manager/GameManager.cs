using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }
    
    
    // Stores the board state:
    // null = empty, "X" = player, "O" = AI

   private string[,] board = new string[3,3];
   private int movesCount = 0;
   private bool gameOver = false;
   private bool enemyDefeated = false;
   private bool playerDefeated = false;
   
   
   

   [Header("Piece Prefabs")]
   public GameObject xPrefab;
   public GameObject oPrefab;


   [Header("UI")]
   public TMP_Text winnerText;
   public TMP_Text turnText;
   public Button restartButton;

   public Enemy Enemy;
   public GameObject missilePrefab;
   public Transform missileSpawnPoint;
   public Transform playerTarget;

   private bool xTurn = true;
   private Camera mainCamera;

   [Header("AI")]
   private bool aiEnabled = true;
   private bool aiThinking = false;
   




    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;


        // Restart button starts hidden and only appears after win/draw.
        restartButton.gameObject.SetActive(false);
    }



     private void Update()
    {
         // Stop input when game is over or while AI is waiting.
        if (gameOver || aiThinking) return;

        // Desktop testing input: click a board cell with the mouse.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BoardCell cell = hit.collider.GetComponent<BoardCell>();

                if (cell != null)
                {
                    cell.Select();
                }
            }
        }
    }




 public void SelectCell(BoardCell cell)
{
    if (gameOver) return;

    string currentPlayer = xTurn ? "X" : "O";

    Debug.Log($"Selected cell: Row {cell.row}, Column {cell.column}");
    
    // Spawn the correct piece based on whose turn it is.
    GameObject prefabToSpawn = xTurn ? xPrefab : oPrefab;
    GameObject piece = Instantiate(prefabToSpawn, cell.transform);

    // X and O use different transforms because the prefabs are built differently.
    if (xTurn)
    {
        piece.transform.localPosition = new Vector3(0, 0.5f, 0.05f);
        piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        piece.transform.localScale = new Vector3(4f, 4f, 4f);
    }
    else
    {
        piece.transform.localPosition = new Vector3(0, 0, 0.05f);
        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }
    // Save the move into the board array.
    board[cell.row, cell.column] = currentPlayer;
    movesCount++;


    // Check if the current move won the game.
    if (CheckWinner(currentPlayer))
    {
        Debug.Log(currentPlayer + " wins!");
        winnerText.text = currentPlayer + " Wins!";
        turnText.text = "";

         if (currentPlayer == "X")
        {
              LaunchMissile();
        }
        else
        {
            LaunchEnemyMissile();
        }
              StartCoroutine(ResetBoardAfterDelay());

        // restartButton.gameObject.SetActive(true);
        gameOver = true;
        return;
    }


    // If all 9 spaces are used and nobody won, it is a draw.
    if (movesCount >= 9)
    {
        Debug.Log("Draw!");
        winnerText.text = "Draw!";
        turnText.text = "";
        // restartButton.gameObject.SetActive(true);
        StartCoroutine(ResetBoardAfterDelay());
        gameOver = true;
        return;
    }
    // Switch turns.
    xTurn = !xTurn;
    turnText.text = xTurn ? "X Turn" : "O Turn";

    // If AI is enabled and it is O's turn, start AI move after a short delay.
    if (aiEnabled && !xTurn)
    {
        StartCoroutine(AIMoveDelay());
    }


}




private bool CheckWinner(string player)
{
     // Check rows and columns.
    for (int i = 0; i < 3; i++)
    {
        if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
            return true;

        if (board[0, i] == player && board[1, i] == player && board[2, i] == player)
            return true;
    }
    // Check diagonals.
    if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
        return true;

    if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
        return true;

    return false;
}




public void RestartGame()
{
    // Reloads the current scene to reset board, pieces, UI, and game state.
    SceneManager.LoadScene(0);
    winnerText.text = "Game Start";
}



private IEnumerator AIMoveDelay()
{
       aiThinking = true;

    // Small delay so AI feels more natural instead of instant.
    yield return new WaitForSeconds(0.5f);

    BoardCell aiCell = FindBestMove();

    if (aiCell != null)
    {
        aiCell.Select();
    }

    aiThinking = false;
}



private BoardCell FindBestMove()
{
    // First priority: block the player if they are about to win.
    BoardCell blockMove = FindBlockingMove();

    if (blockMove != null)
    {
        return blockMove;
    }

    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);

    // Second priority: take center if available.
    foreach (BoardCell cell in cells)
    {
        if (cell.row == 1 && cell.column == 1 && !cell.IsOccupied)
        {
            return cell;
        }
    }

    // Third priority: take the first empty cell.
    foreach (BoardCell cell in cells)
    {
        if (!cell.IsOccupied)
        {
            return cell;
        }
    }

    return null;
}




private BoardCell FindBlockingMove()
{
    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);

    foreach (BoardCell cell in cells)
    {
        if (!cell.IsOccupied)
        {
            // Temporarily pretend the player placed X here.
            board[cell.row, cell.column] = "X";

            bool playerWouldWin = CheckWinner("X");
            
            // Reset the simulated move.
            board[cell.row, cell.column] = null;
            
            // If this move would let X win, AI should block this cell.
            if (playerWouldWin)
            {
                return cell;
            }
        }
    }

    return null;
}



void LaunchMissile()
{
    GameObject missileObj = Instantiate(
        missilePrefab,
        missileSpawnPoint.position,
        missileSpawnPoint.rotation
    );
    AudioManager.Instance.PlayMissileLaunch();

    Missile missile = missileObj.GetComponent<Missile>();

    if (missile != null)
    {
        missile.SetTarget(Enemy.transform, false);
    }
}



private IEnumerator ResetBoardAfterDelay()
{
    yield return new WaitForSeconds(2f);

    ResetBoardOnly();
}


private void ResetBoardOnly()
{
    if (enemyDefeated || playerDefeated) return;

    // Clear board data
    board = new string[3, 3];
    movesCount = 0;
    gameOver = false;
    xTurn = true;
    aiThinking = false;

    winnerText.text = "Game Start";
    turnText.text = "X Turn";
    restartButton.gameObject.SetActive(false);

    // Reset cells and delete X/O pieces
    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);

    foreach (BoardCell cell in cells)
    {
        cell.ResetCell();

        // Delete spawned X/O pieces under each cell
        for (int i = cell.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cell.transform.GetChild(i).gameObject);
        }
    }
}



public void EnemyDefeated()
{
    enemyDefeated = true;
    gameOver = true;


    winnerText.text = "Victory!";
    turnText.text = "";
    restartButton.gameObject.SetActive(true);
}





void LaunchEnemyMissile()
{
    GameObject missileObj = Instantiate(
        missilePrefab,
        Enemy.transform.position,
        Quaternion.identity
    );

    AudioManager.Instance.PlayMissileLaunch();

    Missile missile = missileObj.GetComponent<Missile>();

    if (missile != null)
    {
        missile.SetTarget(playerTarget, true);
    }
}

public void PlayerDefeated()
{
    playerDefeated = true;
    gameOver = true;

    winnerText.text = "Game Over";
    turnText.text = "";
    restartButton.gameObject.SetActive(true);
}



}
