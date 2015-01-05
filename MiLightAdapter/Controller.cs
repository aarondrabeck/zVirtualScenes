using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiLight.Net.Commands;
using MiLight.Net.WifiCintroller;

namespace MiLightAdapter
{
    public class Controller
    {
        public List<MiLight.Net.WifiCintroller.WifiController> Controllers { get; set; }

        public async Task Send(string ipAddress, string command, string zone = null, decimal level = 0)
        {
            var controller = GetController(ipAddress);
            if (controller != null)
            {
                var z = Zone.One;
                byte[] bCommand = null;
                System.Enum.TryParse(zone, out z);
                switch (command)
                {
                    case "On":
                        if(zone!=null) bCommand = MiLight.Net.Api.Colour.On(z);
                        break;
                    case "AllOff":
                        bCommand = MiLight.Net.Api.Colour.AllOff();
                        break;
                    case "AllOn":
                        bCommand = MiLight.Net.Api.Colour.AllOn();
                        break;
                    case "EffectDown":
                        bCommand = MiLight.Net.Api.Colour.EffectDown();
                        break;
                    case "EffectUp":
                        bCommand = MiLight.Net.Api.Colour.EffectUp();
                        break;
                    case "Hue":
                        bCommand = MiLight.Net.Api.Colour.Hue(level);
                        break;
                    case "Off":
                        if (zone != null) bCommand = MiLight.Net.Api.Colour.Off(z);
                        break;
                    case "SetBrightness":
                        bCommand = MiLight.Net.Api.Colour.SetBrightness((int)level);
                        break;
                    case "SpeedDown":
                        bCommand = MiLight.Net.Api.Colour.SpeedDown();
                        break;
                    case "SpeedUp":
                        bCommand = MiLight.Net.Api.Colour.SpeedUp();
                        break;
                    default:
                        break;
                }
                if (bCommand != null) await controller.Send(bCommand);
            }
        }

        private WifiController GetController(string ipAddress)
        {
            return (from x in Controllers where x.IPAddress == ipAddress select x).FirstOrDefault();
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
