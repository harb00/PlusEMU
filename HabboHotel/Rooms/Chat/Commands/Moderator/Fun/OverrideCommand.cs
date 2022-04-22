﻿namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class OverrideCommand : IChatCommand
    {
        public string PermissionRequired => "command_override";

        public string Parameters => "";

        public string Description => "Gives you the ability to walk over anything.";

        public void Execute(GameClients.GameClient session, Room room, string[] @params)
        {
            var user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            user.AllowOverride = !user.AllowOverride;
            session.SendWhisper("Override mode updated.");
        }
    }
}
