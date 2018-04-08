﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ELS.configuration;
using ELS.NUI;
using ELS.Light.Patterns;
using ELS.Light;

namespace ELS.Extra
{

    internal enum LightType
    {
        PRML,
        SECL,
        WRNL,
        SCL,
        SBRN,
        TDL
    }

    internal partial class Extra
    {

        Vector3 _posistion;
        Entity _vehicle;
        int _Id;
        configuration.Extra _extraInfo;
        private bool _state;
        private bool _pattRunning;
        private int _pattnum;
        private LightType LightType { get; set; }
        private string _pattern;
        private string _pattType;
        private SpotLight spotLight;

        internal string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                if (count > value.Length - 1)
                {
                    count = 0;
                }
                _pattern = value;

            }
        }

        internal string PatternType
        {
            get { return _pattType; }
            set
            {
                _pattType = value;
            }
        }

        internal int PatternNum
        {
            get { return _pattnum; }
            set
            {
                _pattnum = value;
            }
        }

        internal int Id
        {
            get
            {
                return _Id;
            }
        }

        bool _on;
        internal bool TurnedOn
        {
            get { return _on; }
            set
            {
                _on = value;
                if (TurnedOn)
                {
                    SetState(true);
                }
                else
                {
                    SetState(false);
                    if (spotLight != null)
                    {
                        spotLight.SpotLightReset();
                    }
                }
            }
        }
        internal bool IsPatternRunning
        {
            get { return _pattRunning; }
            set
            {
                _pattRunning = value;
                if (!IsPatternRunning)
                {
                    CleanUp();
                    count = 0;
                    flashrate = 0;
                }
                else
                {
                    flashrate = Game.GameTime;
                }
            }

        }

        internal Vector3 GetBone()
        {
            if (_Id == 10)
            {
                return ((Vehicle)_vehicle).Bones[$"extra_ten"].Position;
            }
            return ((Vehicle)_vehicle).Bones[$"extra_{_Id}"].Position;
        }

        internal int Delay { get; set; }
        internal bool State
        {
            private set
            {
                _state = value;
                if (value)
                {
                    SetTrue();
                }
                else
                {
                    SetFalse();
                }
            }
            get
            {
                return _state;
            }
        }

        internal Extra(Entity entity, int id, configuration.Extra ex, string format = "", bool state = false)
        {
            _vehicle = entity;
            _Id = id;
            _extraInfo = ex;
            CleanUp();
            SetInfo();
            PatternType = format;
            TurnedOn = false;
            Utils.DebugWriteLine($"Registered extra_{_Id} successfully");
        }

        internal void SetState(bool state)
        {
            this.State = state;
        }

        private void SetTrue()
        {
            API.SetVehicleExtra(_vehicle.Handle, _Id, false);
            if (Game.PlayerPed.IsInPoliceVehicle && Game.PlayerPed.CurrentVehicle.GetNetworkId() == _vehicle.GetNetworkId())
            {
                ElsUiPanel.SendLightData(true, $"#extra{_Id}", _extraInfo.Color);
            }
        }

        private void SetFalse()
        {
            API.SetVehicleExtra(_vehicle.Handle, _Id, true);
            ElsUiPanel.SendLightData(false, $"#extra{_Id}", "");
        }

        int count = 0;
        int flashrate = 0;
        int allowflash = 1;
        bool firstTick = true;
        internal async void ExtraTicker()
        {
            if (flashrate != 0 && Game.GameTime - flashrate >= Delay)
            {
                allowflash = 1;
                if (IsPatternRunning)
                {
                    if (!IsPatternRunning)
                    {
                        CleanUp();
                        return;
                    }
                    if (Pattern.ToCharArray()[count].Equals('0'))
                    {
                        SetState(false);
                        if (!IsPatternRunning)
                        {
                            CleanUp();
                            return;
                        }
                    }
                    else
                    {
                        SetState(true);
                        if (!IsPatternRunning)
                        {
                            CleanUp();
                            return;
                        }

                    }
                    count++;
                    if (count == Pattern.Length - 1)
                    {
                        count = 0;
                    }
                    if (!IsPatternRunning)
                    {
                        CleanUp();
                        return;
                    }
                }
                flashrate = Game.GameTime;

            }
            if (TurnedOn && spotLight != null)
            {
                spotLight.RunTick();
            }

            if (IsPatternRunning)
            {
                if (Pattern[count].Equals('0'))
                {
                }
                else
                {
                    if (_extraInfo.AllowEnvLight)
                    {
                        DrawEnvLight();
                    }
                }
            }
        }

        private Vector3 dirVector;
        private float anglehorizontal = 0f;
        private float anngleVirtical = 0f;

        internal void DrawEnvLight()
        {
            if (!IsPatternRunning)
            {
                return;
            }
            if (_vehicle == null)
            {
                CitizenFX.Core.Debug.WriteLine("Vehicle is null!!!");
                return;
            }
            var off = _vehicle.GetPositionOffset(GetBone());
            if (off == null)
            {
                CitizenFX.Core.Debug.WriteLine("Bone is null for some reason!!!");
                return;
            }
            var extraoffset = _vehicle.GetOffsetPosition(off + new Vector3(_extraInfo.OffsetX, _extraInfo.OffsetY, _extraInfo.OffsetZ));
            API.DrawLightWithRangeAndShadow(extraoffset.X, extraoffset.Y, extraoffset.Z, Color['r'], Color['g'], Color['b'], Global.EnvLightRng, Global.EnvLightInt, 0.01f);
        }

        internal Dictionary<char, int> Color;

        internal void SetInfo()
        {
            Delay = Global.PrimDelay;
            switch (_Id)
            {
                case 1:
                    LightType = LightType.PRML;
                    Pattern = "";
                    break;
                case 2:
                    LightType = LightType.PRML;
                    Pattern = "";
                    break;
                case 3:
                    LightType = LightType.PRML;
                    Pattern = "";
                    break;
                case 4:
                    LightType = LightType.PRML;
                    Pattern = "";
                    break;
                case 5:
                    LightType = LightType.WRNL;
                    Pattern = "";
                    break;
                case 6:
                    LightType = LightType.WRNL;
                    Pattern = "";
                    break;
                case 7:
                    LightType = LightType.SECL;
                    Pattern = "";
                    break;
                case 8:
                    LightType = LightType.SECL;
                    Pattern = "";
                    break;
                case 9:
                    LightType = LightType.SECL;
                    Pattern = "";
                    break;
                case 10:
                    LightType = LightType.SBRN;
                    Pattern = "";
                    IsPatternRunning = false;
                    break;
                case 11:
                    LightType = LightType.SCL;
                    Pattern = "";
                    IsPatternRunning = false;
                    spotLight = new SpotLight(_Id, _vehicle.GetNetworkId());
#if DEBUG
                    CitizenFX.Core.Debug.WriteLine("Scene lights setup");
#endif
                    break;
                case 12:
                    LightType = LightType.TDL;
                    Delay = Global.PrimDelay;
                    Pattern = "";
                    IsPatternRunning = false;
                    spotLight = new SpotLight(_Id, _vehicle.GetNetworkId());
#if DEBUG
                    CitizenFX.Core.Debug.WriteLine("Takedown lights setup");
#endif
                    break;
            }


            string hex = "0xFFFFFF";
            switch (_extraInfo.Color)
            {
                case "red":
                    hex = "0xFF0300";
                    break;
                case "blue":
                    hex = "0x000AFF";
                    break;
                case "amber":
                    hex = "0xFF7E00";
                    break;
                case "white":
                    hex = "0xFFFFFF";
                    break;
                case "green":
                    hex = "0x06ff00";
                    break;
            }
            int r = Convert.ToInt32(hex.Substring(2, 2), 16);
            int g = Convert.ToInt32(hex.Substring(4, 2), 16);
            int b = Convert.ToInt32(hex.Substring(6, 2), 16);
            Color = new Dictionary<char, int>
            {
                { 'r', r },
                { 'g', g },
                { 'b', b },
            };
        }

        internal void CleanUp()
        {
            SetState(false);
        }
    }
}
