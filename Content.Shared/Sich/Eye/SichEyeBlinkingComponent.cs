using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared.Sich.Eye;
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SichEyeBlinkingComponent : Component
{
    [DataField]
    public float BlinkDuration = 0.2f;
    [DataField]
    public float BlinkInterval = 1f;
    [AutoNetworkedField]
    public float NextBlinkTimer = 0f;
    [AutoNetworkedField]
    public float BlinkDurationTimer = 0f;

    public bool IsSleeping = false;

}
