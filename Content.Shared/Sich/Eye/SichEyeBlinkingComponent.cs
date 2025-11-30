using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Sich.Eye;
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class SichEyeBlinkingComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public TimeSpan BlinkDuration = TimeSpan.FromSeconds(2);
    [DataField]
    [AutoNetworkedField]
    public TimeSpan BlinkInterval = TimeSpan.FromSeconds(15);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan LastBlinkTime;
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan NextBlinkingTime;
    [DataField]
    [AutoNetworkedField]
    public bool IsSleeping;
    [DataField]
    [AutoNetworkedField]
    public bool IsBlinking;

}
