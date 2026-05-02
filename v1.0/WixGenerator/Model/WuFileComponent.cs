namespace WixGenerator.Model {

    public class WuFileComponent: WuComponentBase {

        private readonly string _name;
        private readonly string _sourceDir;
        private readonly string _installDir;

        public WuFileComponent(string name, string sourceDir, string installDir) {

            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
            ArgumentNullException.ThrowIfNullOrEmpty(sourceDir, nameof(sourceDir));
            ArgumentNullException.ThrowIfNull(installDir, nameof(installDir));

            _name = name;
            _sourceDir = sourceDir;
            _installDir = installDir;
        }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);

        public string Name => _name;
        public string SourceDir => _sourceDir;
        public string InstallDir => _installDir;
        public WuFileShortcut? Shortcut { get; set; }
    }
}
