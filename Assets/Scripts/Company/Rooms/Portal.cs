using UnityEngine;
using System.Collections;

public class Portal : Room
{

	public Portal(int _position) : base(RoomType.Portal, Balance.Instance().maxPortalStaff, _position)
    {

    }
}
