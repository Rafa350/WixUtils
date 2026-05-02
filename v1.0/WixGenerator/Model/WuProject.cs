namespace WixGenerator.Model {

    public sealed class WuProject: WuCompositeEntity {

        public string Name { get; set; }
        public Guid ProductCode { get; set; }
        public Guid UpgradeCode { get; set; }
        public string IconFile { get; set; } = String.Empty;

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);

        public WuProject Add<T>(T entity) where T : IWuChildOf<WuProject> {

            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            AddEntity(entity);
            return this;
        }

        public WuComponentGroup FindGroup(string name) {

            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

            foreach (var group in Entities.OfType<WuComponentGroup>())
                if (group.Name == name)
                    return group;

            throw new InvalidOperationException($"No se encontro el grupo '{name}'.");
        }
    }
}
