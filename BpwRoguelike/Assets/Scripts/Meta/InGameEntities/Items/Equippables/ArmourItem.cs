using UnityEngine;

public abstract class ArmourItem : Item
{
    [Header("Stats")]
    [Range(5, 100)] public int coverage;
    [Range(5, 100)] public int sturdiness; // maximum damage reduction value per incoming attack
    
    public abstract float DmgAbsorbRatio();
}