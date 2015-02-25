using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiLight.Net.Api;
using MiLight.Net.Commands;
using MiLightAdapter.MiLight;

namespace MiLightAdapter
{
    public class Controller
    {
        public List<WifiController> Controllers { get; set; }

        public async Task Send(string ipAddress, string command, string zone = null, decimal level = 0)
        {
            var controller = GetController(ipAddress);
            if (controller != null)
            {
                Zone z;
                byte[] bCommand = null;
                Enum.TryParse(zone, out z);
                switch (command)
                {
                    case "On":
                        if(zone!=null) bCommand = Colour.On(z);
                        break;
                    case "AllOff":
                        bCommand = Colour.AllOff();
                        break;
                    case "AllOn":
                        bCommand = Colour.AllOn();
                        break;
                    case "EffectDown":
                        bCommand = Colour.EffectDown();
                        break;
                    case "EffectUp":
                        bCommand = Colour.EffectUp();
                        break;
                    case "Hue":
                        bCommand = Colour.Hue(level);
                        break;
                    case "Off":
                        if (zone != null) bCommand = Colour.Off(z);
                        break;
                    case "SetBrightness":
                        bCommand = Colour.SetBrightness((int)level);
                        break;
                    case "SpeedDown":
                        bCommand = Colour.SpeedDown();
                        break;
                    case "SpeedUp":
                        bCommand = Colour.SpeedUp();
                        break;
                }
                if (bCommand != null) await controller.Send(bCommand);
            }
        }

        private WifiController GetController(string ipAddress)
        {
            return (from x in Controllers where x.IpAddress == ipAddress select x).FirstOrDefault();
        }

        public void RemoveController(string ipAddress)
        {
            var c = GetController(ipAddress);
            if (c != null)
            {
                Controllers.Remove(c);
            }
        }
        public void AddController(string ipAddress)
        {
            var c = GetController(ipAddress);
            if (c == null)
            {                
                Controllers.Add(new WifiController(ipAddress));
            }
        }
        public Controller()
        {
            Controllers = new List<WifiController>();
        }

    }
}
