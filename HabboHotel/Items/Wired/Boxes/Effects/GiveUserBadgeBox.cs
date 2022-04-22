﻿using System;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserBadgeBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectGiveUserBadge;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserBadgeBox(Room instance, Item item)
        {
            this.Instance = instance;
            this.Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            var unknown = packet.PopInt();
            var badge = packet.PopString();

            StringData = badge;
        }

        public bool Execute(params object[] @params)
        {
            if (@params == null || @params.Length == 0)
                return false;

            var owner = PlusEnvironment.GetHabboById(Item.UserId);
            if (owner == null || !owner.GetPermissions().HasRight("room_item_wired_rewards"))
                return false;

            var player = (Habbo)@params[0];
            if (player == null || player.GetClient() == null)
                return false;

            var user = player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(player.Username);
            if (user == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            if (player.GetBadgeComponent().HasBadge(StringData))
                player.GetClient().SendPacket(new WhisperComposer(user.VirtualId, "Oops, it appears you have already recieved this badge!", 0, user.LastBubble));
            else
            {
                player.GetBadgeComponent().GiveBadge(StringData, true, player.GetClient());
                player.GetClient().SendNotification("You have recieved a badge!");
            }

            return true;
        }
    }
}