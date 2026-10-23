using UnityEngine;

// While the shop is open: every few seconds, if an NPC slot is free,
// ask the factory for a new visitor and send it to that slot.
public class NPCSpawner : MonoBehaviour
{
    [Tooltip("Drag the object that has the NPCFactory script.")]
    public NPCFactory factory;

    private bool[] spotTaken;
    private float timer;

    void Start()
    {
        if (factory == null) Debug.LogError("NPCSpawner: drag the object with NPCFactory into 'Factory'.");
    }

    public void BeginDay()
    {
        spotTaken = new bool[GameManager.Instance.layout.npcSlots.Length];
        timer = 1f; // first visitor shows up quickly
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.Phase != GamePhase.ShopOpen || factory == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Vector2 interval = gm.Difficulty.SpawnInterval; // Strategy decides how busy the shop is
        timer = Random.Range(interval.x, interval.y);
        TrySpawn(gm);
    }

    private void TrySpawn(GameManager gm)
    {
        int spot = System.Array.IndexOf(spotTaken, false);
        if (spot < 0) return; // every spot is taken, try again next time

        ShopLayout layout = gm.layout;
        NPC npc = factory.CreateNPC(gm.Difficulty, layout.entranceDoor.position);
        if (npc == null) return; // factory already logged what's wrong

        spotTaken[spot] = true;
        npc.AssignSlot(spot, layout.npcSlots[spot].position, layout.exitDoor.position);
        npc.ChangeState(new WalkInState());
    }

    public void FreeSlot(int index)
    {
        if (spotTaken != null && index >= 0 && index < spotTaken.Length) spotTaken[index] = false;
    }

    // Closing time or restart: everyone disappears at once.
    public void RemoveAllNPCs()
    {
        foreach (NPC npc in NPC.Active.ToArray())
        {
            npc.OnLeftShop();
        }
    }
}