namespace WixGenerator.Model {

    public sealed class WuDotNetCompatibilityCheck: WuEntity, IWuChildOf<WuProject> {

        public enum RuntimeTypeValue {
            Core,
            Desktop,
            ASPNet
        }

        public enum PlatformValue {
            x86,
            x64,
            amd64
        }

        public enum RollForwardValue {
            LatestMajor,
            Major,
            LatestMinor,
            Minor,
            LatestPatch,
            Disabled
        }

        public required string PropertyId { get; set; }
        public required string Version { get; set; }
        public PlatformValue Platform { get; set; } = PlatformValue.x64;
        public RuntimeTypeValue RuntimeType { get; set; } = RuntimeTypeValue.Core;
        public RollForwardValue RollForward { get; set; } = RollForwardValue.Disabled;

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
