using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audios : MonoBehaviour
{
    [Header("General Audios")]
    public AudioClip purchase;
    public AudioClip noPurchase;
    public AudioClip chaChing;

    [Header("Round Change Music")]
    public List<AudioClip> roundStart;
    public List<AudioClip> roundEnd;

    [Header("Ambience")]
    public List<AudioClip> windSounds;

    [Header("Player Walking Audios")]
    public List<AudioClip> grassWalk;
    public List<AudioClip> grassRun;
    public List<AudioClip> mudWalk;
    public List<AudioClip> woodWalk;
    public List<AudioClip> woodRun;
    public List<AudioClip> concreteRun;
    public List<AudioClip> concreteWalk;

    private LevelManager levelManager;

    // TODO: Complete this function so a player plays a specific dialog
    // Example: IF player is runnning low on ammo: play an audio where the PLAYER SAYS: Oh no, im low on ammo
    public void CreateAndPlayDialog(GameObject player, AudioClip audio)
    {

    }
}
