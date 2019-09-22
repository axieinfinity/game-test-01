using System;

namespace Gameplay
{

    public abstract class Event
	{

	}

    public class EventTest : Event
    {

    }

    public class EventUnitMoveToTile : Event
    {
        public int unitIndex;
        public int tileIndex;
    }

    public class EventUnitAttackUnit : Event
	{
		public int sourceIndex;
		public int targetIndex;
		public int damage;
        public int remainingHp;
        public bool counterAttack;
	}

    public class EventUnitDead : Event
    {
        public int index;
    }
}