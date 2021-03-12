using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    private GameManager _gm;
    
    public string characterName;
    public Sprite sprite;
    
    // stats
    [Range(1,5)] public int EndLvl = 1;
    [Range(1,5)] public int StrLvl = 1;
    [Range(1,5)] public int DexLvl = 1;
    [Range(1,5)] public int WisLvl = 1;

    public List<AbilityCommand> learnedAbilities;
}