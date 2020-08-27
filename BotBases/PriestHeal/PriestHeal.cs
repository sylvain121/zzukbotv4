using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ZzukBot.Core.Constants;
using ZzukBot.Core.Framework.Interfaces;
using ZzukBot.Core.Framework.Loaders;
using ZzukBot.Core.Game.Objects;
using ZzukBot.Core.Game.Statics;
using ZzukBot.Core.Mem;

namespace PriestHeal
{
    [Export(typeof(IBotBase))]
    public class PriestHeal : IBotBase
    {
        private bool running;
        private Action stopCallback;
        readonly MainThread.Updater MainThread;

        public string Name => "Priest Heal";

        public string Author => "sylvain121";

        public Version Version => new Version(0, 9, 9, 9);

        public PriestHeal()
        {
            MainThread = new MainThread.Updater(Pulse, 250);
        }

        public void Dispose()
        {
        }

        public void PauseBotbase(Action onPauseCallback)
        {
        }

        public bool ResumeBotbase()
        {
            return true;
        }

        public void ShowGui()
        {
        }

        public bool Start(Action onStopCallback)
        {
            if (running) return false;
            WoWEventHandler.Instance.OnChatMessage += OnWoWChatMessage;
            if (!ObjectManager.Instance.IsIngame) return false;
            if (ObjectManager.Instance.Player == null) return false;
            if (!CCLoader.Instance.LoadCustomClass(ObjectManager.Instance.Player.Class)) return false;
            running = true;
            this.stopCallback = stopCallback;
            MainThread.Start();
            return running;

        }

        public void Stop()
        {
            running = false;
        }
        void Pulse()
        {
            ObjectManager.Instance.Player.AntiAfk();
            if (ObjectManager.Instance.PartyLeader.DistanceToPlayer > 5f)
            {
                Navigation.Instance.Traverse(ObjectManager.Instance.PartyLeader.Position);
            } else
            {
                ObjectManager.Instance.Player.CtmStopMovement();
            }

            if (running) return;
            stopCallback();
            MainThread.Stop();
        }

        void OnWoWChatMessage(object sender, WoWEventHandler.ChatMessageArgs args)
        {

            /**
             * "Flash Heal" mana : 380  893-1054 1.5s
             * "Great Heal" mana: 556 1977-2207 2.5s
             * "Heal" mana: 259 783-885 2.5s
             * "Renew" mana: 369 1160-1165 overs 15s
             * "Resurrection" mana 1077 10s
             * "Power Word: Fortitude" mana 1525
             * "Power Word: Shield" mana 450
             * "Divine Spirit" mana 873
             * "Inner Fire" mana 283
             */

            if (args.Message.Contains("HEAL"))
            {
                Common.Instance.DebugMessage("HEAL request for " + args.UnitName);
                if (ObjectManager.Instance.Player.Class == Enums.ClassId.Priest)
                {
                    var partyMember = PartyMembers.Where(x => x.Name == args.UnitName).FirstOrDefault();
                    if (partyMember != null)
                    {
                        ObjectManager.Instance.Player.SetTarget(partyMember.Guid);
                        if (partyMember.DistanceToPlayer > 29f)
                            Navigation.Instance.Traverse(partyMember.Position);
                        else
                        {
                            //if (Spell.Instance.IsKnown("Greater Heal") && partyMember.HealthPercent < 50)
                            //    Spell.Instance.Cast("Greater Heal");
                            //else if (Spell.Instance.IsKnown("Heal"))
                            //Spell.Instance.Cast("Heal");
                            //else
                            Spell.Instance.Cast("Heal");
                        }
                        ObjectManager.Instance.Player.SetTarget(null);
                    }

                }
            }
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
