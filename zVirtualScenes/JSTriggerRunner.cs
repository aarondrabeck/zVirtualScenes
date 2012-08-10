using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zVirtualScenes
{
    public class JSTriggerRunner
    {
        public static string ExecuteScript(string Script)
        {
            Jint.JintEngine engine = new Jint.JintEngine();
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;
            engine.SetFunction("RunScene", new Action<double>(RunScene));
            try
            {
                object result = engine.Run(Script);
                if (result != null) return result.ToString();
            }
            catch (Exception exc)
            {
                return exc.ToString();
            }
            return "";
        }
        public static void RunScene(double SceneID)
        {
            int sid = Convert.ToInt32(SceneID);
            SceneRunner runner = new SceneRunner();
            runner.RunScene(sid);            
        }

    }
}