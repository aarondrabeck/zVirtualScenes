﻿namespace MiLight.Net.Contracts
 {
     using System.Threading.Tasks;

     public interface IController
     {
         Task Send(byte[] command);
     }
 }