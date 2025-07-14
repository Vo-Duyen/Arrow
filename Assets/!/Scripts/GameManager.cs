// Author: DanlangA

using DesignPattern;
using UnityEngine;

public enum EventID
{
    PlayerGetHit,
    EnemyGetHit,
    
}
namespace _.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject player;
    }
}