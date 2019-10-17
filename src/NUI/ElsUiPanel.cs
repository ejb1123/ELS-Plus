using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Dynamic;
using CitizenFX.Core.Native;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using ELS.configuration;

namespace ELS.NUI
{
    internal static class ElsUiPanel
    {
        internal static int _enabled { get; set; }
        internal static bool _runPattern { get; set; }

        //Sent current resource name to the ui
        internal static void InitData()
        {
            string name = API.GetCurrentResourceName();
            Utils.DebugWriteLine($"Sending Current resouce name {name}");
            //API.SendNuiMessage($"{{\"type\":\"initdata\", \"name\":\"{name}\"}}");
            JObject message = new JObject();
            message["type"] = "initdata";
            message["name"] = name;
            message["currentUI"] = ELS.userSettings.uiSettings.currentUI.ToString().ToLower();
            API.SendNuiMessage(message.ToString());
        }

        internal static async void ReloadUI()
        {
            JObject message = new JObject();
            message["type"] = "reload";
            API.SendNuiMessage(message.ToString());
            await ELS.Delay(10000);
            Utils.ReleaseWriteLine("Reloaded UI");
            InitData();
            if (ELS.userSettings.uiSettings.enabled)
            {
                ShowUI();
            }
            else
            {
                DisableUI();
            }
        }

        //Enable full ui control and cursor
        internal static void EnableUI()
        {
            Utils.DebugWriteLine("Enabling UI");
            API.SendNuiMessage("{\"type\":\"enableui\", \"enable\":true}");
            API.SetNuiFocus(true, true);
            _enabled = 2;
        }


        //Disable the UI and cursor
        internal static void DisableUI()
        {
            Utils.DebugWriteLine("Disabling Ui");
            API.SendNuiMessage("{\"type\":\"enableui\", \"enable\":false}");
            if (_enabled == 2)
                API.SetNuiFocus(false, false);
            _enabled = 0;
        }


        //Show only the UI without focus and cursor
        internal static void ShowUI()
        {
            Utils.DebugWriteLine("Showing Ui");
            API.SendNuiMessage("{\"type\":\"enableui\", \"enable\":true}");
            if (_enabled == 2)
                API.SetNuiFocus(false, false);
            _enabled = 1;
        }

        internal static void SetEuro(bool euro)
        {
            Utils.DebugWriteLine("Got Euro");
            API.SendNuiMessage($"{{\"type\":\"seteuro\", \"euro\":{euro.ToString().ToLower()}}}");
        }

        static internal void PlayUiSound(string sound)
        {
            API.SendNuiMessage($"{{\"type\":\"playsound\", \"volume\":{Global.SoundVolume}, \"sound\":\"{sound}\"}}");
        }

        /// <summary>
        /// Send lighting data to display 
        /// </summary>
        /// <param name="state">True or false if light is on</param>
        /// <param name="light">Corresponding light on NUI display</param>
        /// <param name="color">Color of light</param>
        internal static void SendLightData(bool state, string light, string color)
        {
            //CitizenFX.Core.Debug.WriteLine("Sending Light Data");
            API.SendNuiMessage("{\"type\":\"lightControl\", \"state\":" + state.ToString().ToLower() + ", \"light\": \"" + light + "\", \"color\":\"" + color + "\" }");
        }

        internal static void SetUiDesc(string desc, string uielement)
        {
            API.SendNuiMessage($"{{\"type\":\"setuidesc\", \"uielement\":\"{uielement}\", \"desc\":\"{desc}\" }}");
        }

        internal static void ToggleUiBtnState(bool state, string which)
        {
            Utils.DebugWriteLine($"Setting {which} to {state}");
            API.SendNuiMessage($"{{\"type\":\"togglestate\", \"which\":\"{which}\", \"state\":{state.ToString().ToLower()} }}");
        }

        internal static void ToggleStages(int stage)
        {
            API.SendNuiMessage($"{{\"type\":\"togglestage\", \"stage\":{stage}}}");
        }

        static ElsUiPanel()
        {
            _enabled = 0;
            _runPattern = false;
        }

        internal static CallbackDelegate EscapeUI(IDictionary<string, Object> data, CallbackDelegate cb)
        {
            Utils.DebugWriteLine("Escape Executed");
            ShowUI();
            return cb;
        }

        internal static CallbackDelegate TooglePrimary(IDictionary<string, Object> data, CallbackDelegate cb)
        {
            Utils.DebugWriteLine("Toggle Primary Executed");
            return cb;
        }

        internal static CallbackDelegate KeyPress(IDictionary<string, Object> data, CallbackDelegate cb)
        {
            Utils.DebugWriteLine("J key pressed");
            return cb;
        }
    }
}
