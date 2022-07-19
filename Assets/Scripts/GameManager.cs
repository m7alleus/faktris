using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    Block currentBlock;
    Block previewBlock;

    [SerializeField]
    bool withGhost;
    float ghostTransparency = .3f;
    [SerializeField]
    Block ghost;

    [SerializeField]
    bool canHold;
    [SerializeField]
    bool canSpawn;
    Vector3 fallingBlockPosition = new Vector3(5, 18, 0);

    Spawner spawner;
    Holder holder;
    [SerializeField]
    Block holdedBlock;

    void Start() {
        canHold = true;
        canSpawn = true;
        spawner = FindObjectOfType<Spawner>();
        holder = FindObjectOfType<Holder>();
    }

    void Update() {
        if (canSpawn) {
            Spawn();
        }

        if (withGhost && ghost != null) {
            ghost.transform.position = currentBlock.FindClosestFloorPosition();
            ghost.transform.rotation = currentBlock.transform.rotation;
        }

        if (Input.GetKey(KeyCode.C) && canHold && currentBlock != null) {
            Hold();
        }
    }

    void Spawn() {
        if (previewBlock == null) {
            previewBlock = spawner.SpawnRandomBlock();
        }

        if (currentBlock == null) {
            TriggerBlock(previewBlock);
            previewBlock = spawner.SpawnRandomBlock();
        }

        canSpawn = false;
    }

    void Hold() {
        if (holdedBlock == null) {
            holdedBlock = currentBlock;
            holder.PlaceBlockOnHold(currentBlock);
        } else {
            Block previousHoldedBlock = holdedBlock;
            holdedBlock = currentBlock;
            holder.PlaceBlockOnHold(currentBlock);
            TriggerBlock(previousHoldedBlock);
        }
        canHold = false;
    }

    public void TriggerBlock(Block block) {
        currentBlock = block;
        currentBlock.currentState = Block.State.Falling;
        currentBlock.transform.position = fallingBlockPosition;
        currentBlock.IsStill += MarkAsReadyToSpawn;
        currentBlock.IsStill += MarkAsReadyToHold;
        CreateGhost();
    }

    void CreateGhost() {
        if (withGhost) {
            if (ghost != null) {
                Destroy(ghost.gameObject);
            }
            ghost = spawner.InstantiateBlock(currentBlock);
            ghost.currentState = Block.State.Inactive;
            ghost.SetTransparency(ghostTransparency);
        }
    }

    public void MarkAsReadyToSpawn() {
        currentBlock = null;
        canSpawn = true;
    }

    void MarkAsReadyToHold() {
        canHold = true;
    }
}