namespace WixGenerator.Model {

    public sealed class WuFolder: WuCompositeEntity, IWuChildOf<WuProject>, IWuChildOf<WuSpecialFolder>, IWuChildOf<WuFolder> {

        public WuFolder() {
        }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
