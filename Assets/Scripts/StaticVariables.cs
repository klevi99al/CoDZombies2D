using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : MonoBehaviour
{
    public static bool gameIsOver = false;
    public static bool isCoopGame = false;
    public static bool gameIsPaused = false;
    public static bool powerTurnedOn = false;
    public static bool canPowerupsDrop = true;
    public static bool nukedImageActive = false;
    public static bool isAnnouncerSpeaking = false;
    public static bool levelInBetweenRounds = false;
    public static int connectedPlayersNumber = 0;
    public static int selectedCharacterIndex = 0;
    public static int roundNumber = 1;
    public static int zombiesNumber = 0;
    public static int damagePoints = 10;
    public static int mysteryBoxCost = 950;
    public static int normalKillPoints = 60;
    public static int knifeKillPoints = 130;
    public static int maxDropsForPowerup = 3;
    public static int zombieHealthRatio = 50;
    public static int nukeRewardPoints = 400;
    public static int wonderWeaponPoints = 50;
    public static int granadeKillPoints = 100;
    public static int powerupDropPercentage = 2;
    public static int zombiesKilledThisRound = 0;
    public static int zombiesSpawnedThisRound = 0;
    public static float zombieHealthIncrease = 100;
    public static float zombieStartingHealth = 150;
    public static float zombieHealthMultiplier = 0.1f;
    public static float timeToWaitForNextZombieSpawn = 2;
    public static float mysteryBoxSpinningAnimationTime = 2f;
    public static GameObject mysteryBox = null;
    public static List<int> mysteryBoxIndexesBeforeFiresale = new();

    //powerups
    public static int totalPowerupsSpawnedThisRound = 0;
    public static int powerupDuration = 30;
    public static int nextPowerupFreeSlot = 0;
    public static int totalMaxAmmosSpawned = 0;
    public static int totalNukesSpawned = 0;

    public static int totalInstakillSpawned = 0;
    public static int instakillDuration = 30;
    public static bool isInstakillActive = false;
    public static GameObject instakillLastSlot = null;

    public static int totaldoublePointsSpawned = 0;
    public static int doublePointsDuration = 30;
    public static bool isDoublePointsActive = false;
    public static GameObject doublePointsLastSlot = null;

    public static int totalFiresalesSpawned = 0;
    public static int firesaleDuration = 30;
    public static bool isFiresaleActive = false;
    public static GameObject firesaleLastSlot = null;
}
