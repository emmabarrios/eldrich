using UnityEngine;

public class Potion : MonoBehaviour, IUsable {

    public float healthPoints;
    Player player;
    
    public void Use() {
        player = GameObject.Find("Player").GetComponent<Player>();
        player.RecoverHealth(healthPoints);
    }

}
