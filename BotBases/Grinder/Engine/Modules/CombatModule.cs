using System.Linq;
using ZzukBot.Core.Framework;
using ZzukBot.Core.Game.Objects;
using ZzukBot.Core.Game.Statics;
using System.Collections.Generic;

namespace Grinder.Engine.Modules
{
    public class CombatModule
    {
        CustomClasses CustomClasses { get; }
        ObjectManager ObjectManager { get; }
        PathModule PathModule { get; }

        public CombatModule(
            CustomClasses customClasses,
            ObjectManager objectManager,
            PathModule pathModule)
        {
            CustomClasses = customClasses;
            ObjectManager = objectManager;
            PathModule = pathModule;
        }

        public void Fight()
        {
            var unit = PartyMembers.Where(x => x != null && x.IsInCombat);
            if (unit != null)
            {
                CustomClasses.Current.Fight(unit);
            }
            else if (ObjectManager.Units.Count() > 0)
                CustomClasses.Current.Fight(ObjectManager.Units.Where(x => x.IsInCombat || x.GotDebuff("Polymorph")));
        }

        public bool IsBuffRequired()
        {
            return CustomClasses.Current.IsBuffRequired();
        }

        public bool IsReadyToFight()
        {
            return CustomClasses.Current.IsReadyToFight(ObjectManager.Units);
        }

        public void PrepareForFight()
        {
            CustomClasses.Current.PrepareForFight();
        }

        public void Pull(WoWUnit target)
        {
            if (target != null)
                CustomClasses.Current.Pull(target);
        }

        public void Rebuff()
        {
            CustomClasses.Current.Rebuff();
        }


        List<WoWUnit> PartyMembers =>
            new List<WoWUnit>()
            {
                ObjectManager.Instance.PartyLeader,
                ObjectManager.Instance.Party1,
                ObjectManager.Instance.Party2,
                ObjectManager.Instance.Party3,
                ObjectManager.Instance.Party4,
                ObjectManager.Instance.Player
            };

    }
}
