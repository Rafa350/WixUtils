namespace WixGenerator.Model {

    public sealed class WuRegisterAppPath: WuEntity, IWuChildOf<WuComponentBase> {

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
