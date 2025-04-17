using System;
using UnityEngine;

public class FactionHandler : MonoBehaviour
{
    public enum Faction
    {
        Player,
        PlayerAllied,
        Neutral,
        TargetsEverything,
        Enemy1,
        Enemy2,

    }

    //The current faction of the object this script is attached to
    [SerializeField] private Faction faction = Faction.Enemy1;

    //Property to get the current faction
    public Faction CurrentFaction
    {
        get { return faction; }
    }

    //Arrays to store enemies and allies for the current faction
    [SerializeField] private Faction[] enemies;
    [SerializeField] private Faction[] allies;

    void Start()
    {
        //Set default enemies and allies based on the current faction at start
        SetDefaultEnemiesAndAllies();
    }

    //Method to set default enemies and allies based on the current faction
    void SetDefaultEnemiesAndAllies()
    {
        switch (faction)
        {
            case Faction.Player:
                enemies = new Faction[] { Faction.TargetsEverything, Faction.Enemy1, Faction.Enemy2 };
                allies = new Faction[] { Faction.PlayerAllied };
                break;

            case Faction.PlayerAllied:
                enemies = new Faction[] { Faction.TargetsEverything, Faction.Enemy1, Faction.Enemy2 };
                allies = new Faction[] { Faction.Player };
                break;

            case Faction.Enemy1:
                enemies = new Faction[] { Faction.TargetsEverything, Faction.Player, Faction.PlayerAllied };
                allies = new Faction[] { };
                break;

            case Faction.Enemy2:
                enemies = new Faction[] { Faction.TargetsEverything, Faction.Player, Faction.PlayerAllied };
                allies = new Faction[] { };
                break;

            //Default case for factions not explicitly handled, guess ill leave it all empty for now
            default:
                enemies = new Faction[] { };
                allies = new Faction[] { };
                break;
        }
    }

    public bool IsEnemy(Faction otherFaction)
    {
        return Array.IndexOf(enemies, otherFaction) != -1;
    }

    public bool IsAlly(Faction otherFaction)
    {
        return Array.IndexOf(allies, otherFaction) != -1;
    }

    public bool IsOwnFaction(Faction otherFaction)
    {
        return otherFaction == faction;
    }

    public Faction[] Enemies
    {
        get { return enemies; }
    }

    public Faction[] Allies
    {
        get { return allies; }
    }
}