using System.Text;
using Ocelot.Ipc.BossMod;

namespace Ocelot.Rotation.Services.BossMod;

public class BossModRotationService(IBossModIpc ipc) : IRotationService
{
    private const string SINGLE_TARGET_PRESET_NAME = "Ocelot Single Target";

    /// <summary>
    ///     Built-in AI autorotation preset shipped by both BossMod and BossMod Reborn
    ///     (not legacy <c>/vbmai</c> / <c>/bmrai</c>).
    /// </summary>
    private const string AiPresetName = "VBM AI";

    private const string SINGLE_TARGET_PRESET =
        "eyJOYW1lIjoiT2NlbG90IFNpbmdsZSBUYXJnZXQiLCJNb2R1bGVzIjp7IkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SRE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QQ1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TTU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUkciOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NTksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5OSU4iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5SUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQU0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5WUFIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5BU1QiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5XSE0iOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5TR0UiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5ETkMiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5NQ0giOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5EUksiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5HTkIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5QTEQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLnhhbi5CTFUiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5CUkQiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV0sIkJvc3NNb2QuQXV0b3JvdGF0aW9uLlZleW5XQVIiOlt7IlRyYWNrIjoiQU9FIiwiT3B0aW9uIjoiU1QifV19fQ==";

    public void Load()
    {
        var data = Convert.FromBase64String(SINGLE_TARGET_PRESET);
        ipc.Create(Encoding.UTF8.GetString(data));
    }

    public void EnableAutoRotation()
    {
        ipc.SetActive(AiPresetName);
    }

    public void DisableAutoRotation()
    {
        ipc.ClearActive();
    }

    public void EnableSingleTarget()
    {
        ipc.Activate(SINGLE_TARGET_PRESET_NAME);
    }

    public void DisableSingleTarget()
    {
        ipc.Deactivate(SINGLE_TARGET_PRESET_NAME);
    }
}
