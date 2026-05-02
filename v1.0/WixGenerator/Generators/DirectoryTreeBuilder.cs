namespace WixGenerator.Generators {

    internal sealed class DirectoryTreeBuilder {

        private readonly DirectoryNode _root = new DirectoryNode("/");

        public void Append(string path) {

            ArgumentNullException.ThrowIfNullOrEmpty(path, nameof(path));

            var currentNode = _root;
            foreach (string directory in path.Split('\\')) {

                var node = currentNode.Subdirectories.FirstOrDefault(n => n.Name == directory);
                if (node == null) { 
                    node = new DirectoryNode(directory);
                    currentNode.AddSubdirectory(node);
                }
                currentNode = node;
            }
        }

        public DirectoryNode Root => 
            _root;
    }
}
