using System.Collections.Generic;
using CitizenFX.Core;
using Red.Common.Client;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;

namespace Red.ShowId.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Character currentCharacter;
        protected CharacterFeatures currentCharacterFeatures;
        #endregion

        #region Constructor
        public ClientMain() => RegisterNUICallback("closeNUI", CloseNUI);
        #endregion

        #region Commands
        [Command("showlicense")]
        private void ShowLicenseCommand() => GiveLicense();
        #endregion

        #region NUI Callbacks
        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);
            result(new { success = true, message = "success" });
        }
        #endregion

        #region Methods
        private void GiveLicense()
        {
            Player closestPlayer = GetClosestPlayer(2.5f);

            if (closestPlayer is null)
            {
                ErrorNotification("You need to be closer to a player to show them your Id.");
                return;
            }

            TriggerServerEvent("ShowId:Server:showId", closestPlayer.ServerId);
        }

        private bool DisplayNUI(bool display)
        {
            if (display)
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_NUI",
                    firstName = currentCharacter.FirstName,
                    lastName = currentCharacter.LastName,
                    dateOfBirth = currentCharacter.DoB,
                    gender = currentCharacter.Gender,
                    weight = "160",
                    height = "5'11",
                    hair = "Black",
                    eyes = "Blue"
                }));

                SetNUIFocus(true, false, true);
                TriggerEvent("");
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);

            return false;
        }
        #endregion

        #region Event Handlers
        [EventHandler("ShowId:Client:showId")]
        private void OnShowId() => TriggerServerEvent("ShowId:Server:showId");

        [EventHandler("ShowId:Client:returnId")]
        private void OnReturnId()
        {
        }
        #endregion
    }

    public class CharacterFeatures
    {
        public string Weight { get; set; }
        public string Height { get; set; }
        public string Hair { get; set; }
        public string EyeColor { get; set; }
    }
}