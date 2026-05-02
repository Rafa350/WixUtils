namespace WixGenerator.Model {

    public abstract class WuEntity: IWuEntity {

        public abstract void AcceptVisitor(WuVisitor visitor);
    }
}
