using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace zvs.Processor
{
    public static class DetectVCRedist
    {
        public static bool IsVCRedistInstalled()
        {
            string[] strCodes = new string[]
            {
                "{E824E81C-80A4-3DFF-B5F9-4842A9FF5F7F}"  //C++ 2012 x86  //{E824E81C-80A4-3DFF-B5F9-4842A9FF5F7F}
              //  "{0B497B28-5243-3329-9F10-DBB18E0963E6}"//C++ 2012 x64
                
               ////vcredist_x86 - ProductCode 2008
               //"{9A25302D-30C0-39D9-BD6F-21E6EC160475}",
               //"{86CE1746-9EFF-3C9C-8755-81EA8903AC34}",
               //"{CA8A885F-E95B-3FC6-BB91-F4D9377C7686}",
               //"{820B6609-4C97-3A2B-B644-573B06A0F0CC}",
               //"{6AFCA4E1-9B78-3640-8F72-A7BF33448200}",
               //"{F03CB3EF-DC16-35CE-B3C1-C68EA09E5E97}",
               //"{402ED4A1-8F5B-387A-8688-997ABF58B8F2}",
               //"{887868A2-D6DE-3255-AA92-AA0B5A59B874}",
               //"{527BBE2F-1FED-3D8B-91CB-4DB0F838E69E}",
               //"{57660847-B1F7-35BD-9118-F62EB863A598}",
               

               // //Visual C++ 2010 redistributable package product codes:
               // "{196BB40D-1578-3D01-B289-BEFC77A11A1E}",
               // "{DA5E371C-6333-3D8A-93A4-6FD5B20BCC6E}",
               // "{C1A35166-4301-38E9-BA67-02823AD72A1B}",
               // //Visual C++ 2010 SP1 redistributable package product codes
               // "{F0C3E5D1-1ADE-321E-8167-68EF0DE699A5}",
               // "{1D8E6291-B0D5-35EC-8441-6616F567A0F7}",
               // "{88C73C1C-2DE5-3B01-AFB8-B46EF4AB41CD}"
 
               ////vcredist_x64 - ProductCode 2008
               //"{8220EEFE-38CD-377E-8595-13398D740ACE}",
               //"{56F27690-F6EA-3356-980A-02BA379506EE}",
               //"{14297226-E0A0-3781-8911-E9D529552663}",
               //"{9B3F0A88-790D-3AD9-9F96-B19CF2746452}",
               //"{D285FC5F-3021-32E9-9C59-24CA325BDC5C}",
               //"{092EE08C-60DE-3FE6-B113-90076EC06D0D}",
               //"{A96702F7-EFC8-3EED-BE46-22C809D4EBE5}",
               //"{92B8FD1F-C1AE-3750-8577-631B0AA85DF5}",
               //"{2DFD8316-9EF1-3210-908C-4CB61961C1AC}",
               //"{E34002C7-8CE7-3F76-B36C-09FA973BC4F6}",
 
               ////vcredist_IA64 - ProductCode 2008
               //"{5827ECE1-AEB0-328E-B813-6FC68622C1F9}",
               //"{9363B366-8370-34F7-8164-25052EBF35FD}",
               //"{4EC84186-70BB-3121-9C1B-C63512D7126E}",
               //"{1F7B9797-A3C8-3B98-85C4-00620F221CE8}",
               //"{6BE0A7C7-3462-30EE-8B77-D21D7848D967}",
               //"{BF58DC07-38AB-3887-8000-70173F9650EA}",
               //"{D289009A-2728-3D0A-833E-F08E0E58934C}",
               //"{9476DC14-00C3-3C36-A435-00D714CF77B8}",
               //"{678835D7-D524-3C0E-9C33-1D3767FDA6BF}"
           };

            NativeMethods.INSTALLSTATE state;
            for (int i = 0; i < strCodes.Length; i++)
            {
                state = NativeMethods.MsiQueryProductState(strCodes[i]);
                if (state == NativeMethods.INSTALLSTATE.INSTALLSTATE_LOCAL ||
                    state == NativeMethods.INSTALLSTATE.INSTALLSTATE_DEFAULT)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
