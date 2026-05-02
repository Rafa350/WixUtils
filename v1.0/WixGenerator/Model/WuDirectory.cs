namespace WixGenerator.Model {

    public sealed class WuDirectory: WuEntity, IWuChildOf<WuProject> {

        public required string Name { get; init; }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
