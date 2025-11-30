using Robust.Shared.GameStates;

namespace Content.Shared.Sich.Eye;
[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class SichEyeBlinkingComponent : Component
{
    [DataField]
    public float BlinkDuration = 2f;
    [DataField]
    public float BlinkInterval = 15f;
    [AutoNetworkedField]
    public float NextBlinkTimer { get; set; }
    [AutoNetworkedField]
    public float BlinkDurationTimer { get; set; }
    [AutoNetworkedField]
    public bool IsSleeping { get; set; }

}
