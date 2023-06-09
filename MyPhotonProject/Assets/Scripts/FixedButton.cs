using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour,IPointerClickHandler
{
    private PlayerControl player;

    public void SetPlayer(PlayerControl _player)
    {
        player = _player;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        player.Jump();
    }
}
