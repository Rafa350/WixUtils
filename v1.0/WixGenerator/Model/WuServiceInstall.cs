namespace WixGenerator.Model {

    public sealed class WuServiceInstall: WuEntity, IWuChildOf<WuComponentBase> {

        public enum StartModeValue {
            Auto,
            Demand,
            Disabled
        }

        public required string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public StartModeValue StartMode { get; set; } = StartModeValue.Auto;

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
