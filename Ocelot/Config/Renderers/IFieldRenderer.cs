using System.Reflection;
using Ocelot.Config.Fields;
using Ocelot.Services.Translation;

namespace Ocelot.Config.Renderers;

public interface IFieldRenderer<in TAttr> where TAttr : UIFieldAttribute
{
    bool Render(object target, PropertyInfo prop, TAttr attr, Type owner, ITranslator translator);
}
