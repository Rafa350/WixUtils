namespace WixGenerator.Model {

    public sealed class WuVariable: WuEntity, IWuChildOf<WuProject> {

        public required string Name { get; set; }
        public required string Value { get; set; }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
