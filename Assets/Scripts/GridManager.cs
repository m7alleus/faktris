using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public int width = 10;
    public int height = 20;

    public Transform[,] grid;

    public event System.Action IsFull;

    void Start() {
        grid = new Transform[width, height];
    }

    public void Register(Block block) {
        foreach (Transform square in block.transform) {
            int x = Mathf.FloorToInt(square.transform.position.x);
            int y = Mathf.FloorToInt(square.transform.position.y);

            grid[x, y] = square.transform;
        }
        CheckIfBlockMoves(block);
        CheckForCompleteRows();
    }

    void CheckIfBlockMoves(Block block) {
        if(block.moves == 0) {
            if (IsFull != null) {
                IsFull();
            }
        }
    }

    void CheckForCompleteRows() {
        bool completeRowFlag = true;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!completeRowFlag) {
                    continue;
                }
                if (grid[x, y] == null) {
                    completeRowFlag = false;
                }
            }
            if (completeRowFlag) {
                DeleteRow(y);
                ShiftRowsDown(y + 1);
                y--;
            } else {
                completeRowFlag = true;
            }
        }
    }

    void DeleteRow(int rowIndex) {
        for (int x = 0; x < width; x++) {
            Transform squareTransform = grid[x, rowIndex];
            grid[x, rowIndex] = null;
            Transform squareParent = squareTransform.parent;
            Destroy(squareTransform.gameObject);
            if (squareParent.childCount == 0) {
                Destroy(squareParent.gameObject);
            }
        }
    }

    void ShiftRowsDown(int rowIndex) {
        for (int y = rowIndex; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[x, y] != null) {
                    grid[x, y].position += Vector3.down;
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                }
            }
        }
    }
}
