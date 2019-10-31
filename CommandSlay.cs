﻿using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace fr34kyn01535.GlobalBan
{
    public class CommandSlay : IRocketCommand
    {
        public string Help => "Banns a player for a year";

        public string Name => "slay";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string>() {"globalban.slay"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerID = null;

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var isOnline = false;
            CSteamID steamid;
            string charactername = null;
            if (!PlayerTool.tryGetSteamPlayer(command[0], out otherSteamPlayer))
            {
                var player = GlobalBan.GetPlayer(command[0]);
                if (player.Key != null)
                {
                    steamid = player.Key;
                    charactername = player.Value;
                }
                else
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                    return;
                }
            }
            else
            {
                isOnline = true;
                steamid = otherSteamPlayer.playerID.steamID;
                charactername = otherSteamPlayer.playerID.characterName;
            }

            if (command.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.DisplayName, command[1],
                    31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                if (isOnline)
                    Provider.kick(steamPlayerID.steamID, command[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.DisplayName, "",
                    31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Provider.kick(steamPlayerID.steamID,
                        GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}