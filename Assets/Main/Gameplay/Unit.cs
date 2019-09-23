using System;

namespace Gameplay
{
    public class Unit
    {
        public int index;
        public bool attacker;
        public int tileIndex;

        public int hp, maxHp;
        public bool dead;
        public bool countered;
    }
}