namespace WixGenerator.Model {

    public sealed class WuLaunch: WuEntity, IWuChildOf<WuProject> {

        public required string ConditionExpr { get; init; }
        public required string Message { get; init; }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
