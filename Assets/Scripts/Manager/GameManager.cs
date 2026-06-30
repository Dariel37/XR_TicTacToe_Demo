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

   [Header("Enemy")]
   private bool aiEnabled = true;
   private bool aiThinking = false;
   




    private void Awake()
    {
        //Singleton
        Instance = this;
        mainCamera = Camera.main;


        // Restart button starts hidden and only appears after win/draw.
        restartButton.gameObject.SetActive(false);
    }



     private void Update()
    {
         // Stop input when game is over or while Enemy is waiting.
        if (gameOver || aiThinking) return;

        // Desktop input click a board cell with the mouse.
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
    // Stop immediately if the game is over.

    if (gameOver) return;


    // Determine whose turn it currently is.
    // If xTurn is true -> current player is X
    // Otherwise -> current player is O
    string currentPlayer = xTurn ? "X" : "O";

    Debug.Log($"Selected cell: Row {cell.row}, Column {cell.column}");
    
    
    // Choose which prefab to spawn based on current turn.
    GameObject prefabToSpawn = xTurn ? xPrefab : oPrefab;
    // Spawn the correct piece based on whose turn it is.
    GameObject piece = Instantiate(prefabToSpawn, cell.transform);

    // X and O use different transforms because the prefabs are built differently.
    if (xTurn)
    {
        // Adjust X piece
        piece.transform.localPosition = new Vector3(0, 0.5f, 0.05f);
        piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        piece.transform.localScale = new Vector3(4f, 4f, 4f);
    }
    else
    {
        // Adjust O piece
        piece.transform.localPosition = new Vector3(0, 0, 0.05f);
        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    // Save the move into the board array.
    board[cell.row, cell.column] = currentPlayer;

     // Increase move count for draw detection.
    movesCount++;


    // Check if the current move won the game.
    if (CheckWinner(currentPlayer))
    {
        Debug.Log(currentPlayer + " wins!");
        // Update UI
        winnerText.text = currentPlayer + " Wins!";
        turnText.text = "";
        // If player (X) wins, attack enemy.
         if (currentPlayer == "X")
        {
              LaunchMissile();
        }
        else
        {
            // If Enemy (O) wins, enemy attacks player.
            LaunchEnemyMissile();
        }
             // Wait 2 seconds before resetting board.
              StartCoroutine(ResetBoardAfterDelay());

       // Mark game as over so no more moves happen.
        gameOver = true;
        return;
    }


    // If all 9 spaces are used and nobody won, it is a draw.
    if (movesCount >= 9)
    {
        Debug.Log("Draw!");
        // Update UI
        winnerText.text = "Draw!";
        turnText.text = "";

        // Reset board after short delay.
        StartCoroutine(ResetBoardAfterDelay());
        
        // Lock input until reset.
        gameOver = true;
        return;
    }
    // If nobody won and no draw,
    // Switch turns.
    xTurn = !xTurn;

    // Update turn UI
    turnText.text = xTurn ? "X Turn" : "O Turn";

    // If AI is enabled and it is O's turn, start AI move after a short delay.
    // let AI make its move after a short delay.
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
        // Check row i
        // Example when i = 0:
        // board[0,0], board[0,1], board[0,2]
        // (top row)
        if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
            return true;

        // Check column i
        // Example when i = 0:
        // board[0,0], board[1,0], board[2,0]
        // (left column)

        if (board[0, i] == player && board[1, i] == player && board[2, i] == player)
            return true;
    }
    // Check diagonal from top-left to bottom-right
    // [0,0] -> [1,1] -> [2,2]
    if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
        return true;

    // Check diagonal from top-right to bottom-left
    // [0,2] -> [1,1] -> [2,0]

    if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
        return true;

// No winning combination found.
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
    // Mark enemy as thinking.
    // This prevents player input during enemy turn.
       aiThinking = true;

    // Wait 0.5 seconds before enemy makes a move.
    // Makes enemy feel more natural instead of instant.
    yield return new WaitForSeconds(0.5f);

    // Ask enemy logic to choose the best available move.
    BoardCell aiCell = FindBestMove();

    
    // Make sure enemy found a valid cell.
    if (aiCell != null)
    {
        // Enemy selects that cell.
        // This calls BoardCell.Select(),
        // which eventually calls GameManager.SelectCell().
        aiCell.Select();
    }
    // enemy finished its move.
    // Player can interact again.
    aiThinking = false;
}



private BoardCell FindBestMove()
{
    // First priority:
    // First priority: block the player if they are about to win.
    // If yes, return the cell that blocks the player.
    BoardCell blockMove = FindBlockingMove();

    if (blockMove != null)
    {
        return blockMove;
    }
    // Get all BoardCell objects in the scene.
    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);

    // Second priority: take center if available.
    foreach (BoardCell cell in cells)
    {
        if (cell.row == 1 && cell.column == 1 && !cell.IsOccupied)
        {
            return cell;
        }
    }

    // Third priority:
    // If no block is needed and center is taken,
    // choose the first empty cell found.
    foreach (BoardCell cell in cells)
    {
        if (!cell.IsOccupied)
        {
            return cell;
        }
    }
    // If no empty cell exists, return null.
    return null;
}




private BoardCell FindBlockingMove()
{
    // Get all board cells in the scene.
    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);
    
    // Check every cell one by one.
    foreach (BoardCell cell in cells)
    {
        // Only consider empty cells.
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
    // Create (spawn) a missile in the scene.
    // Spawn it at missileSpawnPoint position and rotation.
    GameObject missileObj = Instantiate(
        missilePrefab,
        missileSpawnPoint.position,
        missileSpawnPoint.rotation
    );

    // Play missile launch sound effect.
    AudioManager.Instance.PlayMissileLaunch();
    // Get the Missile script attached to the spawned missile object.
    Missile missile = missileObj.GetComponent<Missile>();
    // Safety check:
    // Make sure missile prefab has a Missile script attached.
    if (missile != null)
    {
        // Tell missile where to go.
        // Enemy.transform = target
        // false = missile was NOT launched by enemy (launched by player)
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
    // Do NOT reset board if the game already ended for real.
    if (enemyDefeated || playerDefeated) return;

   // Reset internal board data.
   // This clears all X and O memory.
    board = new string[3, 3];
    
    // Reset move counter.
    movesCount = 0;
    // Allow gameplay again.
    gameOver = false;
    // New round always starts with X.
    xTurn = true;

    // Enemy is no longer thinking.
    aiThinking = false;

    // Reset UI text.
    winnerText.text = "Game Start";
    turnText.text = "X Turn";

    // Hide restart button during normal gameplay.
    restartButton.gameObject.SetActive(false);

    // Find all board cells in the scene.
    BoardCell[] cells = FindObjectsByType<BoardCell>(FindObjectsSortMode.None);
    // Reset every cell.
    foreach (BoardCell cell in cells)
    {
         // Reset cell state.
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
    //enemy dead
    enemyDefeated = true;
    // Stop gameplay.
    // Prevents any more board moves.
    gameOver = true;

    // Update UI to show Victory. 
    winnerText.text = "Victory!";
    // Clear turn text since turns no longer matter.
    turnText.text = "";
    // Show restart button so player can play again.
    restartButton.gameObject.SetActive(true);
}





void LaunchEnemyMissile()
{
    // Spawn a missile at the enemy's current position.
    // Quaternion.identity means default rotation (no rotation).
    GameObject missileObj = Instantiate(
        missilePrefab,
        Enemy.transform.position,
        Quaternion.identity
    );
    // Play missile launch sound effect.
    AudioManager.Instance.PlayMissileLaunch();
    // Get the Missile script attached to the spawned missile.
    Missile missile = missileObj.GetComponent<Missile>();

    if (missile != null)
    {
        // Set missile target to player.
        // true means this missile was launched by the enemy.
        missile.SetTarget(playerTarget, true);
    }
}



public void PlayerDefeated()
{
    // Mark player as defeated.
    // This tells the game the player lost completely.
    playerDefeated = true;
    // Stop gameplay.
    // Prevents any more board moves.
    gameOver = true;

    // Update UI to show Game Over.
    winnerText.text = "Game Over";
    // Clear turn text since turns no longer matter.
    turnText.text = "";
    // Show restart button so player can play again.
    restartButton.gameObject.SetActive(true);
}



}
