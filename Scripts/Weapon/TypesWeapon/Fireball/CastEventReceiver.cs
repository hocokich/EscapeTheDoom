using UnityEngine;

public class CastEventReceiver : MonoBehaviour
{
    // если хочешь — можешь перетащить ссылку вручную в инспектор
    public Player player;

    void Awake()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    // Этот метод будет вызываться Animation Event'ом (имя должно совпадать)
    public void OnCastComplete()
    {
        if (player != null)
            player.OnCastComplete();
    }
}