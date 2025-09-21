using Ocelot.Graphics;

namespace Ocelot.UI.Services;

public partial interface IUIService
{
    void Text(string text, Color? color = null);

    void Text(object obj, Color? color = null);

    void LabelledValue(string label, string value, Color? labelColor = null, Color? valueColor = null);

    void LabelledValue(string label, object value, Color? labelColor = null, Color? valueColor = null);

    void LabelledValue(object label, string value, Color? labelColor = null, Color? valueColor = null);

    void LabelledValue(object label, object value, Color? labelColor = null, Color? valueColor = null);
}
