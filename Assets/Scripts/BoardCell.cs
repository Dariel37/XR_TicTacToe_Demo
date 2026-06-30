using NUnit.Framework;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
   public int row;
   public int column;

// Used to change cell color visually.
   private Renderer cellRenderer;
   private Color originalColor;
   // Color shown when mouse hovers over the cell.
   public Color hoverColor = Color.cyan;


// Tracks whether this cell already has an X or O.
   public bool IsOccupied { get; private set; }



    private void Awake()
    {
        // Get Renderer component attached to this cell.
        cellRenderer = GetComponent<Renderer>();
        // Save original color so we can restore it later.
        originalColor = cellRenderer.material.color;
    }

    private void OnMouseEnter()
    {
        // When mouse hovers over the cell,
        // highlight it only if the cell is empty.
        if (!IsOccupied)
            cellRenderer.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        // When mouse leaves the cell,
        // restore original color.
        cellRenderer.material.color = originalColor;
    }


    public void Select()
    {
        // Prevent placing more than one piece in the same cell.
        if (IsOccupied)
        {
            Debug.Log("Cell already occupied");
            return;
        }
        // Tell GameManager that this cell was selected.
        GameManager.Instance.SelectCell(this);
        // Play board selection sound.
        AudioManager.Instance.BoardPlayerSelect();
        // Mark this cell as occupied.
        IsOccupied = true;
    }


    public void ResetCell()
    {
        // Mark cell as empty again.
        IsOccupied = false;
        // Restore original color.
        cellRenderer.material.color = originalColor;
    }

}
