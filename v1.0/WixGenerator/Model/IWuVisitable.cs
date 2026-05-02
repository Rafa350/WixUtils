namespace WixGenerator.Model {

    public interface IWuVisitable<Visitor> {

        void AcceptVisitor(Visitor visitor);
    }
}
