﻿using System.Threading.Tasks;

namespace MiLightAdapter.MiLight
 {
     public interface IController
     {
         Task Send(byte[] command);
     }
 }