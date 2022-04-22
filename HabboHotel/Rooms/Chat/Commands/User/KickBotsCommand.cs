﻿using System;
using System.Linq;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;


namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class KickBotsCommand : IChatCommand
    {
        public string PermissionRequired => "command_kickbots";

        public string Parameters => "";

        public string Description => "Kick all of the bots from the room.";

        public void Execute(GameClients.GameClient session, Room room, string[] @params)
        {
            if (!room.CheckRights(session, true))
            {
                session.SendWhisper("Oops, only the room owner can run this command!");
                return;
            }

            foreach (var user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsPet || !user.IsBot)
                    continue;

                RoomUser botUser = null;
                if (!room.GetRoomUserManager().TryGetBot(user.BotData.Id, out botUser))
                    return;

                using (var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", user.BotData.Id);
                    dbClient.RunQuery();
                }

                session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(botUser.BotData.Id), Convert.ToInt32(botUser.BotData.OwnerId), botUser.BotData.Name, botUser.BotData.Motto, botUser.BotData.Look, botUser.BotData.Gender));
                session.SendPacket(new BotInventoryComposer(session.GetHabbo().GetInventoryComponent().GetBots()));
                room.GetRoomUserManager().RemoveBot(botUser.VirtualId, false);
            }

            session.SendWhisper("Success, removed all bots.");
        }
    }
}
