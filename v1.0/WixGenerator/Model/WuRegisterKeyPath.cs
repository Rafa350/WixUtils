namespace WixGenerator.Model {

    public sealed class WuRegisterKeyPath: WuEntity, IWuChildOf<WuComponentBase> {

        public required string Path { get; init; }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
