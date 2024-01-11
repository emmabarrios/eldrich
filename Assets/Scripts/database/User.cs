using System.Collections.Generic;

[System.Serializable]
public class User 
{
    public string userId;
    public List<string> weaponItems;
    public List<string> quickItems;
    public int exp;
    public Stats stats;
    public double totalTraveledDistance;
    public int totalDaysLogged;
    public string lastLoggedDay;
}
