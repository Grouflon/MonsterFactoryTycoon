using UnityEngine;
using System.Collections;

public class Den : Room
{
    public Den(int _position) : base(RoomType.Den, Balance.Instance().maxDenStaff, _position)
    {
        
    }
}
