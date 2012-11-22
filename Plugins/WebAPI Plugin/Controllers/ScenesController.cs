using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers
{
    public class ScenesController : zvsControllerBase<Scene>
    {

    //    ILog log = LogManager.GetLogger<DevicesController>();
    //    public object Get()
    //    {
    //        base.Log(log);
    //        using (zvsContext context = new zvsContext())
    //        {
    //            List<object> scenes = new List<object>();
    //            foreach (Scene scene in context.Scenes)
    //            {
    //                bool show = false;
    //                string prop = ScenePropertyValue.GetPropertyValue(context, scene, "HTTPAPI_SHOW");
    //                bool.TryParse(prop, out show);

    //                if (show)
    //                {
    //                    scenes.Add(new
    //                    {
    //                        id = scene.Id,
    //                        name = scene.Name,
    //                        is_running = scene.isRunning,
    //                        cmd_count = scene.Commands.Count()
    //                    });
    //                }
    //            }

    //            return new { success = true, scenes = scenes.ToArray() };
    //        }

    //    }
    //    public object Get(string name)
    //    {
    //        base.Log(log, "name=", name);
    //        using (zvsContext context = new zvsContext())
    //        {
    //            Scene scene = context.Scenes.FirstOrDefault(s => s.Name == name);

    //            if (scene != null)
    //            {
    //                return Get(scene.Id);
    //            }
    //        }
    //        return new { success = false, reason = "Scene not found." };
    //    }
    //    public object Get(int id)
    //    {
    //        base.Log(log, "id=", id);

    //        using (zvsContext context = new zvsContext())
    //        {
    //            Scene scene = context.Scenes.FirstOrDefault(s => s.Id == id);

    //            if (scene != null)
    //            {
    //                List<object> s_cmds = new List<object>();
    //                foreach (SceneCommand sc in scene.Commands.OrderBy(o => o.SortOrder))
    //                {
    //                    s_cmds.Add(new
    //                    {
    //                        device = sc.StoredCommand.ActionableObject,
    //                        action = sc.StoredCommand.ActionDescription,
    //                        order = (sc.SortOrder + 1)
    //                    });
    //                }
    //                var s = new
    //                {
    //                    id = scene.Id,
    //                    name = scene.Name,
    //                    is_running = scene.isRunning,
    //                    cmd_count = scene.Commands.Count(),
    //                    cmds = s_cmds.ToArray()
    //                };
    //                return new { success = true, scene = s };
    //            }
    //        }
    //        return new { success = false, reason = "Scene not found." };

    //    }

    //    [AcceptVerbs("POST")]
    //    public object Post(int id)
    //    {
    //        base.Log(log, "id=", id);

    //        using (zvsContext context = new zvsContext())
    //        {
    //            Scene scene = context.Scenes.FirstOrDefault(s => s.Id == id);

    //            if (scene != null)
    //            {
    //                return Post(scene.Name);
    //            }
    //        }
    //        return new { success = false, reason = "Scene not found." };
    //    }
    //    [AcceptVerbs("POST")]
    //    public object Post(string name)
    //    {
    //        base.Log(log, "name=", name);

    //        using (zvsContext context = new zvsContext())
    //        {
    //            Scene scene = context.Scenes.FirstOrDefault(s => s.Name == name);

    //            if (scene != null)
    //            {
    //                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
    //                if (cmd != null)
    //                {
    //                    CommandProcessor cp = new CommandProcessor(Core);
    //                    cp.RunBuiltinCommand(context, cmd, scene.Id.ToString());
    //                }
    //                return new { success = true, desc = "Scene Started." };
    //            }
    //        }

    //        return new { success = false, reason = "Scene not found." };
    //    }
        protected override System.Data.Entity.DbSet DBSet
        {
            get { return db.Scenes; }
        }
    }

}