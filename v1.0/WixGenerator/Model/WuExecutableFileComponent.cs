namespace WixGenerator.Model {

    public sealed class WuExecutableFileComponent: WuFileComponent {

        public WuExecutableFileComponent(string name, string sourceDir, string targetDir) :
            base(name, sourceDir, targetDir) {
        }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
