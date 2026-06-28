using NUnit.Framework;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
   public int row;
   public int column;


   private Renderer cellRenderer;
   private Color originalColor;
   public Color hoverColor = Color.cyan;



   public bool IsOccupied { get; private set; }



    private void Awake()
    {
        cellRenderer = GetComponent<Renderer>();
        originalColor = cellRenderer.material.color;
    }

    private void OnMouseEnter()
    {
        if (!IsOccupied)
            cellRenderer.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
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

        GameManager.Instance.SelectCell(this);
        AudioManager.Instance.BoardPlayerSelect();
        IsOccupied = true;
    }


    public void ResetCell()
    {
        IsOccupied = false;
        cellRenderer.material.color = originalColor;
    }

}
