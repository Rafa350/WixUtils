namespace WixGenerator.Generators {

    internal sealed class DirectoryNode {

        private readonly string _name;
        private readonly List<DirectoryNode> _nodes = new List<DirectoryNode>();

        public DirectoryNode(string name) {

            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

            _name = name;
        }

        public void AddSubdirectory(DirectoryNode subdirectory) {

            ArgumentNullException.ThrowIfNull(subdirectory, nameof(subdirectory));

            _nodes.Add(subdirectory);
        }

        public override string ToString() => 
            _name;

        public string Name =>
            _name;

        public IEnumerable<DirectoryNode> Subdirectories =>
            _nodes;
    }
}
