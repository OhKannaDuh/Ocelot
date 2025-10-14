using System.Reflection;
using Ocelot.Config.Fields;

namespace Ocelot.Config.Renderers;

public interface IFieldRenderer<in TAttr> where TAttr : UIFieldAttribute
{
    bool Render(object target, PropertyInfo prop, TAttr attr);
}
