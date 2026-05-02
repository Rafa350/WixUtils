namespace WixGenerator.Model {

    public sealed class WuComponent: WuComponentBase {

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
