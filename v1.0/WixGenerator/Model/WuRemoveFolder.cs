namespace WixGenerator.Model {

    public sealed class WuRemoveFolder: WuEntity, IWuChildOf<WuComponentBase> {

        public enum PerformOnValue {
            Install,
            Uninstall,
            Both
        }

        public required string Directory { get; init; }
        public PerformOnValue PerformOn { get; init; }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
