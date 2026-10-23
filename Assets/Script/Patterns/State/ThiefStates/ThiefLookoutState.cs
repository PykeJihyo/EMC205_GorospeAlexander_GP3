using UnityEngine;

public class ThiefLookoutState : INPCState
{
    private const float StealChanceWhenDistracted = 0.5f;

    private float patience;
    private float boldTimer;
    private float checkTimer;

    public void Enter(NPC npc)
    {
        patience = Random.Range(15f, 25f);
        boldTimer = Random.Range(4f, 10f);
        checkTimer = 1f;
    }

    public void Execute(NPC npc)
    {
        Thief thief = (Thief)npc;

        patience -= Time.deltaTime;
        if (patience <= 0f)
        {
            thief.Say("Just looking."); // same line as a real browsing customer
            thief.LeaveShop();          // gave up, leaves like a normal customer
            return;
        }

        if (thief.IsBold)
        {
            boldTimer -= Time.deltaTime;
            if (boldTimer <= 0f && thief.TrySteal()) return; // right in front of your face
        }

        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f) return;
        checkTimer = 1f;

        if (GameManager.Instance.IsPlayerDistracted && Random.value < StealChanceWhenDistracted)
        {
            thief.TrySteal();
        }
    }

    public void Exit(NPC npc) { }
}