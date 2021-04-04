using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Challenge", menuName = "ScriptableObjects/Challenge", order = 0 )]
public class Challenge : ScriptableObject
{
    public string UniqueChallengeID = "CHANGE_ME";
    public int Reward = 0;
    public bool CannotTakeDamage = false;
    public bool CannotUseCrystals = false;
    public bool CannotMuddySaw = false;
    public int MustKillXZappedEnemys = int.MinValue;
    public int MustKillXEnemysWithFire = int.MinValue;
    public float LevelTimeLimit = float.MaxValue;
    [Multiline(5)]
    public string ChallengeDescription;
}
