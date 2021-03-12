using UnityEngine;

[CreateAssetMenu(menuName = "Item/Trinkets/Nocimonollerom")]
public class Nocimonollerom : Trinket
{
    public PCombatant revivedCombatantPrefab;
    
    public void Resurrect(Combatant killedCombatant, GridPos spawnPos)
    {
        Instantiate(revivedCombatantPrefab, spawnPos.TxPos, Quaternion.identity);
        revivedCombatantPrefab.Point = spawnPos;
        
        revivedCombatantPrefab.manuallySetMaxEnergy = 25;
    }
}